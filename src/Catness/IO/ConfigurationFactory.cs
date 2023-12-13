using Catness.Exceptions;
using Catness.Persistence.Models;
using Newtonsoft.Json;

namespace Catness.IO;

public static class ConfigurationFactory
{
    private const string FileName = "config.json";
    public static BotConfiguration Data { get; set; }

    static ConfigurationFactory()
    {
        if (!File.Exists(FileName))
        {
            Data = new BotConfiguration
            {
                DiscordConfiguration = new DiscordConfiguration
                {
                    Catchphrases = Array.Empty<string>(),
                    DiscordToken = "DISCORD_TOKEN_HERE"
                },
                DatabaseConfiguration = new DatabaseConfiguration
                {
                    ConnectionString = "Host=localhost;Port=5432;Username=postgres;Password=admin;Database=catness-db;Command Timeout=60;Timeout=60;Persist Security Info=True"
                },
                APIKeys = new APIKeys
                {
                    LastFMKey = "YOUR_LASTFM_API_KEY",
                    SteamKey = "YOUR_STEAM_API_KEY",
                    OpenAIKey = "YOUR_OPEN_AI_KEY",
                    TenorKey = "YOUR_TENOR_AI_KEY",
                    MakesweetKey = "YOUR_MAKESWEET_API_KEY"
                },
                DiscordIDs = new DiscordIDs
                {
                    ContributorIDs = Array.Empty<ulong>(),
                    OwnerIDs = Array.Empty<ulong>(),
                    SpecialIDs = Array.Empty<ulong>(),
                    TestingGuildIDs = Array.Empty<ulong>(),
                    ReportChannelIDs = Array.Empty<ulong>()
                }
            };

            string dataJson = JsonConvert.SerializeObject(Data, Formatting.Indented);
            File.WriteAllText(FileName, dataJson);
            Console.Write("Please enter the configuration in the configuration file created.");
            
            Environment.Exit(0);
        }
        else
        {
            string dataJson = File.ReadAllText(FileName);
            try
            {
                Data = JsonConvert.DeserializeObject<BotConfiguration>(dataJson) ??
                       throw new InvalidConfigException("Invalid configuration text, please make sure it is correctly filled");
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception.Message);
                Environment.Exit(1);
            }
        }
    }
}