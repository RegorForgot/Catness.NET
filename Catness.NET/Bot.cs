using System.Reflection;
using Catness.NET.IO;
using Catness.NET.Logging;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Catness.NET;

public class Bot
{
    private readonly IEnumerable<ILogProvider> _logProviders;
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interactionService;
    private ConfigIOService _ioService;

    public Bot(IEnumerable<ILogProvider> logProviders, ConfigIOService ioService, DiscordSocketClient client, InteractionService interactionService)
    {
        _logProviders = logProviders;
        _ioService = ioService;
        _client = client;
        _interactionService = interactionService;

        foreach (ILogProvider logProvider in _logProviders)
        {
            _client.Log += logProvider.Log;
        }
    }

    public async Task MainAsync()
    {
        await _interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), IServiceProvider);
        if (_ioService.Configured)
        {
            await _client.LoginAsync(TokenType.Bot, _ioService.ConfigFile.DiscordToken);
            await _client.StartAsync();
            await Task.Delay(Timeout.Infinite);
        }
        else
        {
            foreach (ILogProvider provider in _logProviders)
            {
                if (provider is IConsoleLogProvider consoleProvider)
                {
                    await consoleProvider.Log("Please ensure your details are entered in the configuration file");
                }
            }
        }
    }
}