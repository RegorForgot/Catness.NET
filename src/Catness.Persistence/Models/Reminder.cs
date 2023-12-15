namespace Catness.Persistence.Models;

public class Reminder
{
    public ulong ReminderId { get; set; }
    public string Body { get; set; }
    public bool PrivateReminder { get; set; }
    public bool Reminded { get; set; }
    
    public ulong ChannelId { get; set; }
    
    public virtual User User { get; set; }
    public ulong UserId { get; set; }
    
    public DateTimeOffset ReminderTime { get; set; }
}