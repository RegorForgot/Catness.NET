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
    private CancellationTokenSource _source = new CancellationTokenSource();

    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interactionService;
    private readonly UserHandler _userHandler;
    private readonly IServiceProvider _serviceProvider;
    private readonly IEnumerable<ILogProvider> _logProviders;
    private readonly ReminderService _reminderService;
    private readonly BotConfiguration _botConfiguration;

    private bool _ready;

    public BotService(
        DiscordSocketClient client,
        InteractionService interactionService,
        IServiceProvider serviceProvider,
        IEnumerable<ILogProvider> logProviders,
        IOptions<BotConfiguration> botConfiguration,
        ReminderService reminderService,
        UserHandler userHandler)
    {
        _client = client;
        _interactionService = interactionService;
        _serviceProvider = serviceProvider;
        _logProviders = logProviders;
        _reminderService = reminderService;
        _userHandler = userHandler;
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
            Task.Run(_reminderService.StopAllReminders);
            Task.Run(_source.CancelAsync);
            return Task.CompletedTask;
        };

        _client.Connected += () =>
        {
            if (_ready)
            {
                Task.Run(_reminderService.StartReminderHandling);
                Task.Run(SetStatus);
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
            Task.Run(_reminderService.StartReminderHandling);
            Task.Run(SetStatus);
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

    private async Task SetStatus()
    {
        using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMinutes(30));
        try
        {
            do
            {
                _source = new CancellationTokenSource();

                if (_botConfiguration.DiscordConfiguration.Catchphrases.Length == 0)
                {
                    await _source.CancelAsync();
                }

                string status = _botConfiguration.DiscordConfiguration.Catchphrases
                    .OrderBy(_ => Guid.NewGuid()).ToArray()[0];

                await _client.SetCustomStatusAsync(status);
            } while (await timer.WaitForNextTickAsync(_source.Token));
        }
        catch (OperationCanceledException) { }
    }
}