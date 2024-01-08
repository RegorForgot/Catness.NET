using System.Reflection;
using Catness.Logging;
using Catness.Persistence.Models;
using Catness.Services.Timed;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace Catness.Handlers;

public class BotHandler
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interactionService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IEnumerable<ILogProvider> _logProviders;
    private readonly IEnumerable<ITimedService> _timedServices;
    private readonly BotConfiguration _botConfiguration;
    private readonly UserHandler _userHandler;

    private bool _isPreviouslyReady;

    public BotHandler(
        DiscordSocketClient client,
        InteractionService interactionService,
        IServiceProvider serviceProvider,
        IEnumerable<ILogProvider> logProviders,
        IOptions<BotConfiguration> botConfiguration,
        IEnumerable<ITimedService> timedServices,
        UserHandler userHandler
    )
    {
        _client = client;
        _interactionService = interactionService;
        _serviceProvider = serviceProvider;
        _logProviders = logProviders;
        _timedServices = timedServices;
        _botConfiguration = botConfiguration.Value;
        _userHandler = userHandler;
    }

    public async Task OnBotStartup()
    {
        await _client.SetStatusAsync(UserStatus.AFK);
        
        _client.InteractionCreated += OnInteractionCreated;
        _client.MessageReceived += _userHandler.HandleLevellingAsync;
        _client.Disconnected += OnDisconnected;
        _client.Connected += OnConnected;
        _client.Ready += OnReady;
        _client.ButtonExecuted += OnButtonClicked;

        foreach (ILogProvider logProvider in _logProviders)
        {
            _client.Log += logProvider.Log;
        }
        
        await _client.LoginAsync(TokenType.Bot, _botConfiguration.DiscordConfiguration.DiscordToken);
        await _interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), _serviceProvider);
    }

    private Task StartServices()
    {
        foreach (ITimedService timedService in _timedServices)
        {
            Task.Run(timedService.Start);
        }
        return Task.CompletedTask;
    }

    private Task OnReady()
    {
        foreach (ulong testingGuildID in _botConfiguration.DiscordIDs.TestingGuildIDs)
        {
            _interactionService.RegisterCommandsToGuildAsync(testingGuildID);
        }
        _isPreviouslyReady = true;
        StartServices();
        return Task.CompletedTask;
    }

    private Task OnConnected()
    {
        if (_isPreviouslyReady)
        {
            StartServices();
        }
        return Task.CompletedTask;
    }

    private Task OnDisconnected(Exception exception)
    {
        foreach (ITimedService timedService in _timedServices)
        {
            Task.Run(timedService.Stop);
        }
        return Task.CompletedTask;
    }

    private async Task OnInteractionCreated(SocketInteraction interaction)
    {
        SocketInteractionContext context = new SocketInteractionContext(_client, interaction);
        await _interactionService.ExecuteCommandAsync(context, _serviceProvider);
    }

    private async Task OnButtonClicked(SocketMessageComponent component)
    {
        SocketInteractionContext context = new SocketInteractionContext(_client, component);
        await _interactionService.ExecuteCommandAsync(context, _serviceProvider);
    }
}