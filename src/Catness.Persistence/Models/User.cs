namespace Catness.Persistence.Models;

public class User
{
    public ulong Id { get; set; }

    public DateTimeOffset? Birthday { get; set; }
}