using Newtonsoft.Json;

namespace Catness.IO;

public record ConfigFile
{
    [JsonProperty] public string DiscordToken { get; init; } = "YOUR_DISCORD_BOT_TOKEN";
    [JsonProperty] public string LastFMKey { get; init; } = "YOUR_LASTFM_API_KEY";
    [JsonProperty] public string SteamKey { get; init; } = "YOUR_STEAM_API_KEY";
    [JsonProperty] public string OpenAIKey { get; init; } = "YOUR_OPEN_AI_KEY";
    [JsonProperty] public string TenorKey { get; init; } = "YOUR_TENOR_AI_KEY";
    [JsonProperty] public string MakesweetKey { get; init; } = "YOUR_MAKESWEET_API_KEY";

    [JsonProperty] public string Prefix { get; init; } = "!";
    [JsonProperty] public UInt128 ReportChannelID { get; init; } = 1234;

    [JsonProperty] public UInt128[] ContributorIDs { get; init; } = Array.Empty<UInt128>();
    [JsonProperty] public UInt128[] SpecialIDs { get; init; } = Array.Empty<UInt128>();
    [JsonProperty] public UInt128[] OwnerIDs { get; init; } = Array.Empty<UInt128>();

    [JsonProperty] public string[] Catchphrases { get; init; } = Array.Empty<string>();
}