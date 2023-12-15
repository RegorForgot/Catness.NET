using Catness.Enums;
using Catness.Persistence.Models;
using Catness.Services.EFServices;
using Microsoft.Extensions.Caching.Memory;

namespace Catness.Handlers;

public class ReminderHandler
{
    private readonly ReminderService _reminderService;
    private readonly IMemoryCache _memoryCache;

    public ReminderHandler(
        ReminderService reminderService,
        IMemoryCache memoryCache)
    {
        _reminderService = reminderService;
        _memoryCache = memoryCache;
    }

    public async Task PrepareExpiry()
    {
        IEnumerable<Reminder> reminders = await _reminderService.GetUpcomingActiveReminders();

        ParallelOptions options = new ParallelOptions
        {
            MaxDegreeOfParallelism = 50
        };

        await Parallel.ForEachAsync(reminders, options, (reminder, _) =>
        {
            Task.Run(() => ConsumeReminder(reminder), CancellationToken.None);
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

    private async Task SendReminder(Reminder reminder) { }

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