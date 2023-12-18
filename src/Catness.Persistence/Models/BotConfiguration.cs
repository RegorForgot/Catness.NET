namespace Catness.Persistence.Models;

public class BotConfiguration
{
    public DiscordConfiguration DiscordConfiguration { get; set; }
    public DatabaseConfiguration DatabaseConfiguration { get; set; }
    public DiscordIDs DiscordIDs { get; set; }
    public APIKeys APIKeys { get; set; }
}

public class DiscordConfiguration
{
    public string DiscordToken { get; set; }
    public string[] Catchphrases { get; set; }
}

public class DatabaseConfiguration
{
    public string ConnectionString { get; set; }
}

public class APIKeys
{
    public string LastFMKey { get; set; }
    public string SteamKey { get; set; }
    public string TenorKey { get; set; }
    public string MakesweetKey { get; set; }
    public string ImgurKey { get; set; }
}

public class DiscordIDs
{
    public ulong[] ReportChannelIDs { get; set; }
    public ulong[] ContributorIDs { get; set; }
    public ulong[] SpecialIDs { get; set; }
    public ulong[] OwnerIDs { get; set; }
    public ulong[] TestingGuildIDs { get; set; }
}