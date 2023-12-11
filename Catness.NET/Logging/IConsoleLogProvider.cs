namespace Catness.NET.Logging;

public interface IConsoleLogProvider : ILogProvider
{
    public Task Log(string message);
}