namespace Catness.Persistence.Models;

public class GuildUser
{
    public ulong GuildId { get; set; }
    public ulong UserId { get; set; }
    
    public bool IsLevelBlacklisted { get; set; }
    public ulong Level { get; set; }
    public ulong Experience { get; set; }
    
    public Guild Guild { get; set; }
    public User User { get; set; }
}