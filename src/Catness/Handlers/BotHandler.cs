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
    private readonly BotConfiguration _botConfiguration;
    private readonly ReminderDispatchService _reminderDispatchService;
    private readonly StatusService _statusService;
    private readonly UserHandler _userHandler;
    private readonly BirthdayService _birthdayService;

    private bool _isPreviouslyReady;

    public BotHandler(
        DiscordSocketClient client,
        InteractionService interactionService,
        IServiceProvider serviceProvider,
        IEnumerable<ILogProvider> logProviders,
        IOptions<BotConfiguration> botConfiguration,
        ReminderDispatchService reminderDispatchService,
        StatusService statusService,
        UserHandler userHandler,
        BirthdayService birthdayService
    )
    {
        _client = client;
        _interactionService = interactionService;
        _serviceProvider = serviceProvider;
        _logProviders = logProviders;
        _botConfiguration = botConfiguration.Value;
        _reminderDispatchService = reminderDispatchService;
        _statusService = statusService;
        _userHandler = userHandler;
        _birthdayService = birthdayService;
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
        Task.Run(_reminderDispatchService.Start);
        Task.Run(_statusService.Start);
        Task.Run(_birthdayService.Start);
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
        Task.Run(_reminderDispatchService.Stop);
        Task.Run(_statusService.Stop);
        Task.Run(_birthdayService.Stop);
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