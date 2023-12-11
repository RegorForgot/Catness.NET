using Discord;

namespace Catness.NET.Logging;

public interface ILogProvider
{
    public Task Log(LogMessage message);
}