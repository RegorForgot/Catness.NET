using Discord;

namespace Catness.NET.Logging;

public class DiscordLogProvider : ILogProvider
{
    public Task Log(LogMessage message)
    {
        return Task.CompletedTask;
    }
}