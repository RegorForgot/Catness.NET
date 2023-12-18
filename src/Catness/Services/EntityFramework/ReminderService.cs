using Catness.Handlers;
using Catness.Persistence;
using Catness.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Catness.Services.EntityFramework;

public class ReminderService
{
    private CancellationTokenSource ReminderCanceller { get; set; } = new CancellationTokenSource();

    private readonly IDbContextFactory<CatnessDbContext> _dbContextFactory;
    private readonly ReminderHandler _reminderHandler;

    public ReminderService(
        IDbContextFactory<CatnessDbContext> dbContextFactory,
        ReminderHandler reminderHandler)
    {
        _dbContextFactory = dbContextFactory;
        _reminderHandler = reminderHandler;
    }

    public async Task<Reminder?> GetReminder(Guid guid)
    {
        await using CatnessDbContext dbContext = await _dbContextFactory.CreateDbContextAsync();

        return await dbContext.Reminders.AsNoTracking().FirstOrDefaultAsync(reminder => reminder.ReminderGuid == guid);
    }

    public async Task<List<Reminder>> GetUserReminders(ulong userId, bool includePrivate)
    {
        await using CatnessDbContext dbContext = await _dbContextFactory.CreateDbContextAsync();

        IQueryable<Reminder> reminderQuery = dbContext.Reminders.Where(reminder => reminder.UserId == userId);

        if (!includePrivate)
        {
            reminderQuery = reminderQuery.Where(reminder => !reminder.PrivateReminder);
        }

        return await reminderQuery
            .OrderBy(reminder => reminder.ReminderTime)
            .ThenBy(reminder => reminder.TimeCreated)
            .AsNoTracking()
            .ToListAsync();
    }

    public Task<List<Reminder>> GetUserReminders(User user, bool includePrivate)
    {
        return GetUserReminders(user.UserId, includePrivate);
    }

    public async Task<bool> AddReminder(Reminder reminder)
    {
        List<Reminder> reminders = await GetUserReminders(reminder.UserId, true);

        if (reminders.Count >= 7)
        {
            return false;
        }

        await using CatnessDbContext dbContext = await _dbContextFactory.CreateDbContextAsync();

        await dbContext.Reminders.AddAsync(reminder);

        if (reminder.ReminderTime - DateTime.UtcNow < TimeSpan.FromMinutes(10))
        {
            Task.Run(() => _reminderHandler.ConsumeReminder(reminder), ReminderCanceller.Token);
        }

        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<List<Reminder>> GetUpcomingReminders()
    {
        await using CatnessDbContext dbContext = await _dbContextFactory.CreateDbContextAsync();

        List<Reminder> reminders = await dbContext.Reminders
            .Where(reminder => reminder.ReminderTime - DateTime.UtcNow < TimeSpan.FromMinutes(10))
            .AsNoTracking()
            .ToListAsync();

        return reminders;
    }

    public class ReminderRemoverService
    {
        private readonly IDbContextFactory<CatnessDbContext> _dbContextFactory;

        public ReminderRemoverService(IDbContextFactory<CatnessDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task RemoveReminder(Reminder reminder)
        {
            await using CatnessDbContext context = await _dbContextFactory.CreateDbContextAsync();

            context.Reminders.Remove(reminder);
            await context.SaveChangesAsync();
        }
    }
}