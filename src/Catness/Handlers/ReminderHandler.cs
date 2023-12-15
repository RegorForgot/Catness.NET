using Catness.Persistence.Models;
using Catness.Services.EFServices;

namespace Catness.Handlers;

public class ReminderHandler
{
    private readonly ReminderService _reminderService;

    public ReminderHandler(ReminderService reminderService)
    {
        _reminderService = reminderService;
    }

    public async Task PrepareExpiry()
    {
        IEnumerable<Reminder> reminders = await _reminderService.GetUpcomingActiveReminders();
    }
}

public class ReminderInteraction
{
    public ulong ReminderId { get; set; }
    public string Body { get; set; }
    public bool PrivateReminder { get; set; }
    
    public bool Reminding { get; set; }
    public bool Reminded { get; set; }
    
    public ulong ChannelId { get; set; }

    public User User { get; set; }
}