using System.Reflection;
using Catness.Handlers;
using Catness.Logging;
using Catness.Persistence.Models;
using Catness.Services.EFServices;
using Catness.Services.Timed;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace Catness.Services;

public class BotService
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interactionService;
    private readonly ReminderDispatchService _reminderDispatchService;
    private readonly BirthdayService _birthdayService;
    private readonly StatusService _statusService;
    private readonly UserHandler _userHandler;
    private readonly BotConfiguration _botConfiguration;
    private readonly IEnumerable<ILogProvider> _logProviders;
    private readonly IServiceProvider _serviceProvider;

    private bool _ready;

    public BotService(
        DiscordSocketClient client,
        InteractionService interactionService,
        IServiceProvider serviceProvider,
        IEnumerable<ILogProvider> logProviders,
        IOptions<BotConfiguration> botConfiguration,
        ReminderDispatchService reminderDispatchService,
        StatusService statusService,
        UserHandler userHandler,
        BirthdayService birthdayService)
    {
        _client = client;
        _interactionService = interactionService;
        _serviceProvider = serviceProvider;
        _logProviders = logProviders;
        _reminderDispatchService = reminderDispatchService;
        _statusService = statusService;
        _userHandler = userHandler;
        _birthdayService = birthdayService;
        _botConfiguration = botConfiguration.Value;
    }

    public async Task StartAsync()
    {

        await _client.SetStatusAsync(UserStatus.AFK);

        _client.InteractionCreated += async interaction =>
        {
            SocketInteractionContext context = new SocketInteractionContext(_client, interaction);
            await _interactionService.ExecuteCommandAsync(context, _serviceProvider);
        };

        _client.MessageReceived += message =>
            _userHandler.HandleLevellingAsync(message);

        _client.Disconnected += _ =>
        {
            Task.Run(_reminderDispatchService.Stop);
            Task.Run(_statusService.Stop);
            Task.Run(_birthdayService.Stop);
            return Task.CompletedTask;
        };

        _client.Connected += () =>
        {
            if (_ready)
            {
                StartServices();
            }
            return Task.CompletedTask;
        };

        _client.Ready += () =>
        {
            foreach (ulong testingGuildID in _botConfiguration.DiscordIDs.TestingGuildIDs)
            {
                _interactionService.RegisterCommandsToGuildAsync(testingGuildID);
            }
            _ready = true;
            StartServices();
            return Task.CompletedTask;
        };

        foreach (ILogProvider logProvider in _logProviders)
        {
            _client.Log += logProvider.Log;
        }

        await _client.LoginAsync(TokenType.Bot, _botConfiguration.DiscordConfiguration.DiscordToken);
        await _interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), _serviceProvider);

        await _client.StartAsync();
    }

    public Task StartServices()
    {
        Task.Run(_reminderDispatchService.Start);
        Task.Run(_statusService.Start);
        Task.Run(_birthdayService.Start);
        return Task.CompletedTask;
    }
}