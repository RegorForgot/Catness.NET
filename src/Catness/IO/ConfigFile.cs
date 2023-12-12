using Newtonsoft.Json;
using static Catness.IO.ConfigFileDefaults;

namespace Catness.IO;

public record ConfigFile
{
    [JsonProperty] public string DiscordToken { get; init; } = DefaultDiscordToken;
    [JsonProperty] public string LastFMKey { get; init; } = DefaultLastFMKey;
    [JsonProperty] public string SteamKey { get; init; } = DefaultSteamKey;
    [JsonProperty] public string OpenAIKey { get; init; } = DefaultOpenAIKey;
    [JsonProperty] public string TenorKey { get; init; } = DefaultTenorKey;
    [JsonProperty] public string MakesweetKey { get; init; } = DefaultMakesweetKey;
    [JsonProperty] public ulong ReportChannelID { get; init; } = DefaultReportChannelID;
    [JsonProperty] public ulong[] ContributorIDs { get; init; } = DefaultContributorIDs;
    [JsonProperty] public ulong[] SpecialIDs { get; init; } = DefaultSpecialIDs;
    [JsonProperty] public ulong[] OwnerIDs { get; init; } = DefaultOwnerIDs;
    [JsonProperty] public ulong[]  TestingGuildIDs { get; init; } = DefaultTestingGuildIDs;
    [JsonProperty] public string[] Catchphrases { get; init; } = DefaultCatchphrases;
}