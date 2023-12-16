﻿using System.Reflection;
using Catness.Handlers;
using Catness.Logging;
using Catness.Persistence.Models;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using NodaTime;

namespace Catness.Services;

public class BotService
{
    private readonly DiscordSocketClient _discordSocketClient;
    private readonly InteractionService _interactionService;
    private readonly UserHandler _userHandler;
    private readonly IServiceProvider _serviceProvider;
    private readonly IEnumerable<ILogProvider> _logProviders;
    private readonly ReminderHandler _reminderHandler;
    private readonly BotConfiguration _botConfiguration;
    private readonly CancellationTokenSource _reminderTokenSource;

    public BotService(
        DiscordSocketClient discordSocketClient,
        InteractionService interactionService,
        IServiceProvider serviceProvider,
        IEnumerable<ILogProvider> logProviders,
        IOptions<BotConfiguration> botConfiguration,
        ReminderHandler reminderHandler,
        UserHandler userHandler)
    {
        _discordSocketClient = discordSocketClient;
        _interactionService = interactionService;
        _serviceProvider = serviceProvider;
        _logProviders = logProviders;
        _reminderHandler = reminderHandler;
        _userHandler = userHandler;
        _reminderTokenSource = new CancellationTokenSource();
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

        _discordSocketClient.Disconnected += _ => _reminderTokenSource.CancelAsync();

        _discordSocketClient.Ready += () =>
        {
            foreach (ulong testingGuildID in _botConfiguration.DiscordIDs.TestingGuildIDs)
            {
                _interactionService.RegisterCommandsToGuildAsync(testingGuildID);
            }
            Task.Run(StartReminderHandling);
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

    private async Task StartReminderHandling()
    {
        using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMinutes(10));
        try
        {
            do
            {
                await _reminderHandler.PrepareExpiry(_reminderTokenSource.Token);
            } while (await timer.WaitForNextTickAsync(_reminderTokenSource.Token));
        }
        catch (OperationCanceledException)
        {
            Console.Write("Cancelled");
        }
    }
}