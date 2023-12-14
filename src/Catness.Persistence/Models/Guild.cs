namespace Catness.Persistence.Models;

public class Guild
{
    public ulong GuildId { get; set; }
    
    public bool LevellingEnabled { get; set; } = true;
    
    public ICollection<GuildUser> Users { get; set; }
}