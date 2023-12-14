namespace Catness.Persistence.Models;

public class GuildUser
{
    public Guild Guild { get; set; }
    public User User { get; set; }
    
    public ulong GuildId { get; set; }
    public ulong UserId { get; set; }
}