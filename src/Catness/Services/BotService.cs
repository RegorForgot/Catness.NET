using Catness.Handlers;
using Discord.WebSocket;

namespace Catness.Services;

public class BotService
{
    private readonly DiscordSocketClient _client;
    private readonly BotHandler _botHandler;

    public BotService(
        DiscordSocketClient client,
        BotHandler botHandler)
    {
        _client = client;
        _botHandler = botHandler;
    }

    public async Task StartAsync()
    {
        await _botHandler.OnBotStartup();
        await _client.StartAsync();
    }
}