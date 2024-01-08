using Catness.Handlers;
using Catness.Persistence.Models;
using Catness.Services.EntityFramework;
using Microsoft.Extensions.Caching.Memory;

namespace Catness.Services.Timed;

public class ReminderDispatchService : ITimedService
{
    public CancellationTokenSource TokenSource { get; private set; }
    private readonly ReminderService _reminderService;
    private readonly ReminderService.ReminderRemoverService _removerService;
    private readonly IMemoryCache _memoryCache;
    private readonly ReminderHandler _reminderHandler;

    public ReminderDispatchService(
        ReminderService reminderService,
        ReminderService.ReminderRemoverService removerService,
        IMemoryCache memoryCache,
        ReminderHandler reminderHandler)
    {
        TokenSource = new CancellationTokenSource();
        _reminderService = reminderService;
        _removerService = removerService;
        _memoryCache = memoryCache;
        _reminderHandler = reminderHandler;
    }

    public async Task Start()
    {
        TokenSource = new CancellationTokenSource();

        using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMinutes(10));
        try
        {
            do
            {
                Console.WriteLine("Started reminder operations...");

                IEnumerable<Reminder> reminders = await _reminderService.GetUpcomingReminders();
                ParallelOptions options = new ParallelOptions
                {
                    MaxDegreeOfParallelism = 50,
                    CancellationToken = TokenSource.Token
                };


                await Parallel.ForEachAsync(reminders, options, (reminder, _) =>
                {
                    Task.Run(() => _reminderHandler.ConsumeReminder(reminder), TokenSource.Token);
                    return ValueTask.CompletedTask;
                }).ConfigureAwait(false);

            } while (await timer.WaitForNextTickAsync(TokenSource.Token));
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Cancelled reminder operation");
        }
    }

    public async Task Stop()
    {
        await TokenSource.CancelAsync();
    }

    public async Task StopReminder(Reminder reminder)
    {
        string key = ReminderHandler.GetReminderCancellationTokenCacheKey(reminder.ReminderGuid);
        bool reminding = _memoryCache.TryGetValue(key, out CancellationTokenSource? cancellationToken);

        if (!reminding)
        {
            await _removerService.RemoveReminder(reminder);
        }
        else
        {
            await cancellationToken!.CancelAsync();
        }
    }
}