using Discord;

namespace Catness.Logging;

public interface ILogProvider
{
    public Task Log(LogMessage message);
}