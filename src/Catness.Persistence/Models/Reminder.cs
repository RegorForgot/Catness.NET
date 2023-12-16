namespace Catness.Persistence.Models;

public class Reminder
{
    public Guid ReminderGuid { get; set; }
    public string Body { get; set; }
    public bool PrivateReminder { get; set; }
    
    public ulong? ChannelId { get; set; }

    public virtual User User { get; set; }
    public ulong UserId { get; set; }

    public DateTime ReminderTime { get; set; }
    public DateTime TimeCreated { get; set; }
}