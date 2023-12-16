using Catness.Handlers;
using Catness.Persistence;
using Catness.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Catness.Services.EFServices;

public class ReminderService
{
    public CancellationTokenSource ReminderCanceller { get; private set; } = new CancellationTokenSource();

    private readonly IDbContextFactory<CatnessDbContext> _dbContextFactory;
    private readonly IMemoryCache _memoryCache;
    private readonly ReminderHandler _reminderHandler;

    public ReminderService(
        IDbContextFactory<CatnessDbContext> dbContextFactory,
        ReminderHandler reminderHandler,
        IMemoryCache memoryCache)
    {
        _dbContextFactory = dbContextFactory;
        _reminderHandler = reminderHandler;
        _memoryCache = memoryCache;
    }

    public async Task<List<Reminder>> GetUserReminders(User user, bool noPrivate)
    {
        await using CatnessDbContext dbContext = await _dbContextFactory.CreateDbContextAsync();

        IQueryable<Reminder> reminderQuery = dbContext.Reminders.Where(reminder => reminder.UserId == user.UserId);

        if (noPrivate)
        {
            reminderQuery = reminderQuery.Where(reminder => !reminder.PrivateReminder);
        }

        return await reminderQuery
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddReminder(Reminder reminder)
    {
        await using CatnessDbContext dbContext = await _dbContextFactory.CreateDbContextAsync();

        await dbContext.Reminders.AddAsync(reminder);

        if (reminder.ReminderTime - DateTime.UtcNow < TimeSpan.FromMinutes(10))
        {
            Task.Run(() => ConsumeReminder(reminder), ReminderCanceller.Token);
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateReminder(Reminder reminder)
    {
        await using CatnessDbContext context = await _dbContextFactory.CreateDbContextAsync();

        context.Reminders.Update(reminder);
        await context.SaveChangesAsync();
    }

    private async Task<List<Reminder>> GetUpcomingActiveReminders()
    {
        await using CatnessDbContext dbContext = await _dbContextFactory.CreateDbContextAsync();

        List<Reminder> reminders = await dbContext.Reminders
            .Where(reminder => reminder.ReminderTime - DateTime.UtcNow < TimeSpan.FromMinutes(10))
            .Where(reminder => reminder.Reminded == RemindedType.None)
            .AsNoTracking()
            .ToListAsync();

        return reminders;
    }

    public void CreateNewToken()
    {
        ReminderCanceller = new CancellationTokenSource();
    }

    public async Task PrepareReminderExpiry() { }

    private async Task ConsumeReminder(Reminder reminder)
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
            await _reminderHandler.SendReminder(reminder);
            reminder.Reminded = RemindedType.Reminded;
        }
        else
        {
            CancellationTokenSource cancellationToken = new CancellationTokenSource();

            _memoryCache.Set(key, cancellationToken, timeToReminder);
            try
            {
                await Task.Delay(timeToReminder, cancellationToken.Token);
                await _reminderHandler.SendReminder(reminder);
                reminder.Reminded = RemindedType.Reminded;
            }
            catch (OperationCanceledException)
            {
                reminder.Reminded = RemindedType.Cancelled;
                _memoryCache.Remove(key);
            }
        }

        await UpdateReminder(reminder);
    }


    public async Task StopReminder(Reminder reminder)
    {
        if (reminder.Reminded != RemindedType.None)
        {
            return;
        }

        string key = GetReminderCancellationTokenCacheKey(reminder.ReminderGuid);
        bool reminding = _memoryCache.TryGetValue(key, out CancellationTokenSource? cancellationToken);

        if (!reminding)
        {
            reminder.Reminded = RemindedType.Cancelled;
            await UpdateReminder(reminder);
        }
        else
        {
            await cancellationToken!.CancelAsync();
        }
    }

    public async Task StopAllReminders()
    {
        await ReminderCanceller.CancelAsync();
    }

    public async Task StartReminderHandling()
    {
        using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMinutes(10));
        try
        {
            do
            {
                CreateNewToken();
                IEnumerable<Reminder> reminders = await GetUpcomingActiveReminders();

                ParallelOptions options = new ParallelOptions
                {
                    MaxDegreeOfParallelism = 50,
                    CancellationToken = ReminderCanceller.Token
                };

                await Parallel.ForEachAsync(reminders, options, (reminder, _) =>
                {
                    Task.Run(() => ConsumeReminder(reminder), ReminderCanceller.Token);
                    return ValueTask.CompletedTask;
                }).ConfigureAwait(false);

            } while (await timer.WaitForNextTickAsync(ReminderCanceller.Token));
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Cancelled reminder waiting");
        }
    }

    private static string GetReminderCancellationTokenCacheKey(Guid reminderId)
    {
        return $"reminder-token-{reminderId}";
    }
}