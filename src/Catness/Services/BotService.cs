using System.Reflection;
using Catness.Handlers;
using Catness.Logging;
using Catness.Persistence.Models;
using Catness.Services.EFServices;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace Catness.Services;

public class BotService
{
    private readonly DiscordSocketClient _discordSocketClient;
    private readonly InteractionService _interactionService;
    private readonly UserHandler _userHandler;
    private readonly IServiceProvider _serviceProvider;
    private readonly IEnumerable<ILogProvider> _logProviders;
    private readonly ReminderService _reminderService;
    private readonly BotConfiguration _botConfiguration;

    private bool _ready;

    public BotService(
        DiscordSocketClient discordSocketClient,
        InteractionService interactionService,
        IServiceProvider serviceProvider,
        IEnumerable<ILogProvider> logProviders,
        IOptions<BotConfiguration> botConfiguration,
        ReminderService reminderService,
        UserHandler userHandler)
    {
        _discordSocketClient = discordSocketClient;
        _interactionService = interactionService;
        _serviceProvider = serviceProvider;
        _logProviders = logProviders;
        _reminderService = reminderService;
        _userHandler = userHandler;
        _botConfiguration = botConfiguration.Value;
    }

    public async Task StartAsync()
    {
        _discordSocketClient.InteractionCreated += async interaction =>
        {
            SocketInteractionContext context = new SocketInteractionContext(_discordSocketClient, interaction);
            await _interactionService.ExecuteCommandAsync(context, _serviceProvider);
        };

        _discordSocketClient.MessageReceived += message =>
            _userHandler.HandleLevellingAsync(message);

        _discordSocketClient.Disconnected += _ => _reminderService.StopAllReminders();

        _discordSocketClient.Connected += () =>
        {
            if (_ready)
            {
                Task.Run(_reminderService.StartReminderHandling);
            }
            return Task.CompletedTask;
        };

        _discordSocketClient.Ready += () =>
        {
            foreach (ulong testingGuildID in _botConfiguration.DiscordIDs.TestingGuildIDs)
            {
                _interactionService.RegisterCommandsToGuildAsync(testingGuildID);
            }
            _ready = true;
            Task.Run(_reminderService.StartReminderHandling);
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