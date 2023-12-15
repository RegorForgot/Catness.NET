namespace Catness.Persistence.Models;

public class User
{
    public ulong UserId { get; set; }

    public string? LastfmUsername { get; set; }
    public string? SteamVanity { get; set; }

    public DateOnly? Birthday { get; set; }
    public DateTimeOffset? LastRepTime { get; set; }

    public ulong Experience { get; set; }
    public short Level { get; set; }

    public bool LevellingEnabled { get; set; }
    public bool PrivateUser { get; set; } = true;

    public ulong Rep { get; set; }

    public string? Locale { get; set; }

    public ICollection<Follow> Following { get; set; }
    public ICollection<Follow> Followers { get; set; }

    public ICollection<Reminder> Reminders { get; set; }
    public ICollection<GuildUser> Guilds { get; set; }
}