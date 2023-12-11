using Discord;

namespace Catness.NET.Logging;

public class ConsoleLogProvider : IConsoleLogProvider
{
    public Task Log(LogMessage message)
    {
        Console.WriteLine(message.ToString());
        return Task.CompletedTask;
    }
    
    public Task Log(string message)
    {
        Console.WriteLine(message);
        return Task.CompletedTask;
    }
}