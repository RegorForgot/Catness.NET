using Catness.Persistence;
using Catness.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Catness.Services.EFServices;

public class ReminderService
{
    private readonly IDbContextFactory<CatnessDbContext> _dbContextFactory;

    public ReminderService(
        IDbContextFactory<CatnessDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<List<Reminder>> GetUpcomingActiveReminders()
    {
        await using CatnessDbContext dbContext = await _dbContextFactory.CreateDbContextAsync();

        return await dbContext.Reminders
            .Where(reminder => reminder.ReminderTime - DateTimeOffset.Now < TimeSpan.FromMinutes(10) && reminder.Reminded == RemindedType.None)
            .ToListAsync();
    }

    public async Task<List<Reminder>> GetUserReminders(User user)
    {
        await using CatnessDbContext dbContext = await _dbContextFactory.CreateDbContextAsync();
        
        return await dbContext.Reminders
            .Where(reminder => reminder.UserId == user.UserId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Guid> AddReminder(Reminder reminder)
    {
        await using CatnessDbContext dbContext = await _dbContextFactory.CreateDbContextAsync();

        await dbContext.Reminders.AddAsync(reminder);
        await dbContext.SaveChangesAsync();
        return reminder.ReminderGuid;
    }
    
    
    public async Task UpdateReminder(Reminder reminder)
    {
        await using CatnessDbContext context = await _dbContextFactory.CreateDbContextAsync();

        context.Reminders.Update(reminder);
        await context.SaveChangesAsync();
    }
}