namespace Catness.Persistence.Models;

public class GuildChannel
{
    public ulong ChannelId { get; set; }
    public ulong GuildId { get; set; }
    public bool CommandsDisabled { get; set; }
    public bool LevellingEnabled { get; set; }
    
    public Guild Guild { get; set; }
}