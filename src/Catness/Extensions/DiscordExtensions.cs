namespace Catness.Extensions;

public static class DiscordExtensions
{
    public static string GetPingString(this ulong discordID)
    {
        return $"<@{discordID}>";
    }
}