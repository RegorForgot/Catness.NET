namespace Catness.Persistence.Models;

public class Reminder
{
    public Guid ReminderGuid { get; set; }
    public string Body { get; set; }
    public bool PrivateReminder { get; set; }

    public RemindedType Reminded { get; set; } = RemindedType.None;

    public ulong? ChannelId { get; set; }

    public virtual User User { get; set; }
    public ulong UserId { get; set; }

    public DateTimeOffset ReminderTime { get; set; }
    public DateTimeOffset TimeCreated { get; set; }
}