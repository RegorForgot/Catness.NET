#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Catness.IO;

public abstract class AbstractIOService
{
    public virtual string FilePath { get; protected set; }

    protected static readonly string SaveFolder;
    protected static readonly object FileLock;

    static AbstractIOService()
    {
        SaveFolder = "./";
        FileLock = new object();
    }

    protected string ReadData()
    {
        string value;

        lock (FileLock)
        {
            value = File.ReadAllText(FilePath);
        }

        return value;
    }

    protected bool FileExists()
    {
        lock (FileLock)
        {
            return File.Exists(FilePath);
        }
    }

    protected abstract void CreateFile();
}