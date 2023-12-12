namespace Catness.Logging;

public interface IConsoleLogProvider : ILogProvider
{
    public Task Log(string message);
}