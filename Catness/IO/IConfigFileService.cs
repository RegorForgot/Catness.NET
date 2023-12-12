namespace Catness.IO;

public interface IConfigFileService
{
    public ConfigFile ConfigFile { get; set; }
    public bool Configured { get; set; }
}