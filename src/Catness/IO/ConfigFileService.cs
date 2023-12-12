using Catness.Exceptions;
using Catness.Logging;
using Newtonsoft.Json;
using static Catness.IO.ConfigFileDefaults;

namespace Catness.IO;

public sealed class ConfigFileService : AbstractIOService, IConfigFileService
{
    private IConsoleLogProvider LogProvider { get; }
    
    public override string FilePath { get; protected set; } = $"{SaveFolder}/config.json";
    public ConfigFile ConfigFile { get; set; }
    public bool Configured { get; set; }

    public ConfigFileService(IConsoleLogProvider logProvider)
    {
        LogProvider = logProvider;
        Configured = false;
        
        if (!FileExists())
        {
            CreateFile();
        }
        else
        {
            ReadConfigData();

            Configured =
                ConfigFile is not null &&
                ConfigFile.DiscordToken != DefaultDiscordToken && 
                !ConfigFile.TestingGuildIDs.SequenceEqual(DefaultTestingGuildIDs);
            
            if (!Configured)
            {
                CreateFile();
            }
        }
    }

    private void ReadConfigData()
    {
        try
        {
            string configText = ReadData();

            ConfigFile configFile = JsonConvert.DeserializeObject<ConfigFile>(configText) ?? throw new InvalidConfigException("Invalid configuration text, please make sure it is correctly filled");
            ConfigFile = configFile;
        }
        catch (Exception ex) when (ex is IOException or JsonException)
        {
            LogProvider.Log(ex.Message);
            
            CreateFile();
        }
        catch (Exception ex) when (ex is InvalidConfigException)
        {
            LogProvider.Log(ex.Message);
            
            CreateFile();
        }
    }

    protected override void CreateFile()
    {
        lock (FileLock)
        {
            using (File.Create(FilePath)) { }
            
            string config = JsonConvert.SerializeObject(new ConfigFile(), Formatting.Indented);
            File.WriteAllText(FilePath, config);
        }
        
        ConfigFile = new ConfigFile();
    }
}