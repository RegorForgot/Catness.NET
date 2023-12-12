namespace Catness.IO;

public struct ConfigFileDefaults
{
    public const string DefaultDiscordToken  = "YOUR_DISCORD_BOT_TOKEN";
    public const string DefaultLastFMKey  = "YOUR_LASTFM_API_KEY";
    public const string DefaultSteamKey  = "YOUR_STEAM_API_KEY";
    public const string DefaultOpenAIKey  = "YOUR_OPEN_AI_KEY";
    public const string DefaultTenorKey  = "YOUR_TENOR_AI_KEY";
    public const string DefaultMakesweetKey  = "YOUR_MAKESWEET_API_KEY";
    public const string DefaultPrefix  = "!";
    public const ulong DefaultReportChannelID  = 1234;
    public static readonly ulong[] DefaultContributorIDs = Array.Empty<ulong>();
    public static readonly ulong[] DefaultSpecialIDs = Array.Empty<ulong>();
    public static readonly ulong[] DefaultOwnerIDs = Array.Empty<ulong>();
    public const ulong DefaultTestingGuildID  = 0;
    public static readonly string[] DefaultCatchphrases = Array.Empty<string>();
}