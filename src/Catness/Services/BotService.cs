using System.Reflection;
using Catness.Logging;
using Catness.Persistence.Models;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace Catness.Services;

public class BotService
{
    private readonly DiscordSocketClient _discordSocketClient;
    private readonly InteractionService _interactionService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IEnumerable<ILogProvider> _logProviders;
    private readonly BotConfiguration _botConfiguration;

    public BotService(
        DiscordSocketClient discordSocketClient,
        InteractionService interactionService,
        IServiceProvider serviceProvider,
        IEnumerable<ILogProvider> logProviders,
        IOptions<BotConfiguration> botConfiguration)
    {
        _discordSocketClient = discordSocketClient;
        _interactionService = interactionService;
        _serviceProvider = serviceProvider;
        _logProviders = logProviders;
        _botConfiguration = botConfiguration.Value;
    }
    
    public async Task StartAsync()
    {
        _discordSocketClient.InteractionCreated += async interaction =>
        {
            SocketInteractionContext context = new SocketInteractionContext(_discordSocketClient, interaction);
            await _interactionService.ExecuteCommandAsync(context, _serviceProvider);
        };

        _discordSocketClient.Ready += () =>
        {
            foreach (ulong testingGuildID in _botConfiguration.DiscordIDs.TestingGuildIDs)
            {
                _interactionService.RegisterCommandsToGuildAsync(testingGuildID);
            }
            return Task.CompletedTask;
        };

        foreach (ILogProvider logProvider in _logProviders)
        {
            _discordSocketClient.Log += logProvider.Log;
        }

        await _discordSocketClient.LoginAsync(TokenType.Bot, _botConfiguration.DiscordConfiguration.DiscordToken);
        await _interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), _serviceProvider);

        await _discordSocketClient.StartAsync();
    }
}