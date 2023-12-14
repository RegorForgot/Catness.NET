namespace Catness.Persistence.Models;

public class User
{
    public ulong UserId { get; set; }

    public DateOnly? Birthday { get; set; }
    
    public ulong Experience { get; set; }
    public ulong Level { get; set; }
    
    public bool LevellingEnabled { get; set; }
    
    public ICollection<Follow> Following { get; set; }
    public ICollection<Follow> Followers { get; set; }
    
    public ICollection<GuildUser> Guilds { get; set; }
}