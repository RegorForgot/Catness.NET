using Catness.Persistence.Models;
using Catness.Services.EFServices;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Caching.Memory;

namespace Catness.Handlers;

public class ReminderHandler
{
    private readonly DiscordSocketClient _client;
    private readonly ReminderService _reminderService;
    private readonly IMemoryCache _memoryCache;

    public ReminderHandler(
        DiscordSocketClient client,
        ReminderService reminderService,
        IMemoryCache memoryCache)
    {
        _client = client;
        _reminderService = reminderService;
        _memoryCache = memoryCache;
    }

    public async Task PrepareExpiry(CancellationToken token)
    {
        IEnumerable<Reminder> reminders = await _reminderService.GetUpcomingActiveReminders();

        ParallelOptions options = new ParallelOptions
        {
            MaxDegreeOfParallelism = 50,
            CancellationToken = token
        };

        await Parallel.ForEachAsync(reminders, options, (reminder, _) =>
        {
            Task.Run(() => ConsumeReminder(reminder), token);
            return ValueTask.CompletedTask;
        }).ConfigureAwait(false);
    }

    private async Task ConsumeReminder(Reminder reminder)
    {
        string key = GetCancellationTokenCacheKey(reminder.ReminderId);

        bool reminding = _memoryCache.TryGetValue(key, out _);

        if (reminding)
        {
            return;
        }

        TimeSpan timeToReminder = reminder.ReminderTime - DateTimeOffset.Now;

        if (timeToReminder < TimeSpan.Zero)
        {
            await SendReminder(reminder);
            reminder.Reminded = RemindedType.Reminded;
        }
        else
        {
            CancellationTokenSource cancellationToken = new CancellationTokenSource();

            _memoryCache.Set(key, cancellationToken, timeToReminder);
            try
            {
                await Task.Delay(timeToReminder, cancellationToken.Token);
                await SendReminder(reminder);
                reminder.Reminded = RemindedType.Reminded;
            }
            catch (OperationCanceledException)
            {
                reminder.Reminded = RemindedType.Cancelled;
                _memoryCache.Remove(key);
            }
        }

        await _reminderService.UpdateReminder(reminder);
    }

    private async Task SendReminder(Reminder reminder)
    {
        Embed embed = new EmbedBuilder()
            .WithTitle($"Reminder <@{reminder.UserId}!")
            .WithDescription(reminder.Body)
            .WithCurrentTimestamp()
            .Build();

        if (reminder.PrivateReminder)
        {
            if (_client.GetUser(reminder.UserId) is IUser user)
            {
                try
                {
                    await user.SendMessageAsync(embed: embed, allowedMentions: new AllowedMentions(AllowedMentionTypes.Users));
                }
                catch (HttpException) { }
            }
        }
        else
        {
            if (_client.GetChannel(reminder.ChannelId) is ITextChannel channel)
            {

                try
                {
                    await channel.SendMessageAsync(embed: embed, allowedMentions: new AllowedMentions(AllowedMentionTypes.Users));
                }
                catch (HttpException) { }
            }
        }
    }

    public async Task StopReminding(Reminder reminder)
    {
        if (reminder.Reminded != RemindedType.None)
        {
            return;
        }

        string key = GetCancellationTokenCacheKey(reminder.ReminderId);
        bool reminding = _memoryCache.TryGetValue(key, out CancellationTokenSource? cancellationToken);

        if (!reminding)
        {
            reminder.Reminded = RemindedType.Cancelled;
            await _reminderService.UpdateReminder(reminder);
        }
        else
        {
            await cancellationToken!.CancelAsync();
        }
    }

    public static string GetCancellationTokenCacheKey(ulong reminderId)
    {
        return $"reminder-token-{reminderId}";
    }
}