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
    private readonly ReminderService.ReminderRemoverService _removerService;
    private readonly IMemoryCache _memoryCache;

    public ReminderHandler(
        DiscordSocketClient client,
        ReminderService.ReminderRemoverService removerService, 
        IMemoryCache memoryCache)
    {
        _client = client;
        _removerService = removerService;
        _memoryCache = memoryCache;
    }

    public async Task SendReminder(Reminder reminder, bool late = false)
    {
        EmbedBuilder embedBuilder = new EmbedBuilder()
            .WithTitle($"Reminder!")
            .WithDescription($"{reminder.Body}")
            .WithCurrentTimestamp();

        if (late)
        {
            embedBuilder = embedBuilder.WithFooter("Sorry for the late reminder!");
        }

        Embed embed = embedBuilder.Build();

        if (reminder.PrivateReminder)
        {
            if (_client.GetUser(reminder.UserId) is IUser user)
            {
                try
                {
                    await user.SendMessageAsync($"<@{reminder.UserId}>", embed: embed, allowedMentions: new AllowedMentions(AllowedMentionTypes.Users));
                }
                catch (HttpException) { }
            }
        }
        else
        {
            if (_client.GetChannel(reminder.ChannelId ?? 0) is ITextChannel channel)
            {

                try
                {
                    await channel.SendMessageAsync($"<@{reminder.UserId}>", embed: embed, allowedMentions: new AllowedMentions(AllowedMentionTypes.Users));
                }
                catch (HttpException) { }
            }
        }
    }
    
    public async Task ConsumeReminder(Reminder reminder)
    {
        string key = GetReminderCancellationTokenCacheKey(reminder.ReminderGuid);

        bool reminding = _memoryCache.TryGetValue(key, out _);

        if (reminding)
        {
            return;
        }

        TimeSpan timeToReminder = reminder.ReminderTime - DateTime.UtcNow;

        if (timeToReminder < TimeSpan.Zero)
        {
            await SendReminder(reminder, true);
        }
        else
        {
            CancellationTokenSource cancellationToken = new CancellationTokenSource();

            _memoryCache.Set(key, cancellationToken, timeToReminder);
            try
            {
                await Task.Delay(timeToReminder, cancellationToken.Token);
                await SendReminder(reminder);
            }
            catch (OperationCanceledException)
            {
                _memoryCache.Remove(key);
            }
        }

        await _removerService.RemoveReminder(reminder);
    }
    
    
    public static string GetReminderCancellationTokenCacheKey(Guid reminderId)
    {
        return $"reminder-token-{reminderId}";
    }
}