using Discord.WebSocket;

namespace Catness.Handlers;

public class BotHandler
{
    private readonly DiscordSocketClient _client;
    
    public BotHandler(
        DiscordSocketClient client)
    {
        _client = client;

    }
}