using Discord;

namespace Catness.Logging;

public class DiscordLogProvider : ILogProvider
{
    public Task Log(LogMessage message)
    {
        return Task.CompletedTask;
    }
}