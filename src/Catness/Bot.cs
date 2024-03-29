﻿using Catness.Clients;
using Catness.Handlers;
using Catness.IO;
using Catness.Persistence;
using Catness.Persistence.Models;
using Catness.Services;
using Catness.Services.EntityFramework;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Fergun.Interactive;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace Catness;

public class Bot
{
    private IConfiguration Configuration { get; }

    private Bot()
    {
        _ = ConfigurationFactory.Data;

        IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("config.json", true, true);

        Configuration = configurationBuilder.Build();
    }

    public async static Task StartBot(string[] args)
    {
        Bot bot = new Bot();
        await bot.MainAsync();
    }

    private async Task MainAsync()
    {
        IServiceProvider serviceProvider = CreateServiceProvider();
        await serviceProvider.GetService<BotService>()!.StartAsync();

        await Task.Delay(Timeout.Infinite);
    }

    private ServiceProvider CreateServiceProvider()
    {
        IServiceCollection serviceCollection = new ServiceCollection()
            .Configure<BotConfiguration>(Configuration);

        serviceCollection.Scan(scan => scan
            .FromAssemblyOf<Bot>()
            .AddClasses(classes => classes.InNamespaces("Catness.Logging"))
            .UsingRegistrationStrategy(RegistrationStrategy.Append)
            .AsSelfWithInterfaces()
            .WithSingletonLifetime()
        );

        DiscordSocketConfig clientConfig = new DiscordSocketConfig
        {
            GatewayIntents =
                GatewayIntents.AllUnprivileged &
                ~GatewayIntents.GuildScheduledEvents &
                ~GatewayIntents.GuildInvites
        };

        InteractionServiceConfig interactionConfig = new InteractionServiceConfig
        {
            EnableAutocompleteHandlers = true
        };

        InteractiveConfig interactiveConfig = new InteractiveConfig()
        {
            DefaultTimeout = TimeSpan.FromMinutes(10)
        };

        DiscordSocketClient client = new DiscordSocketClient(clientConfig);
        InteractionService interactionService = new InteractionService(client, interactionConfig);
        InteractiveService interactiveService = new InteractiveService(client, interactiveConfig);

        serviceCollection
            .AddSingleton<MakesweetClient>()
            .AddSingleton<GuildService>()
            .AddSingleton<FollowService>()
            .AddSingleton<UserService>()
            .AddSingleton<ReminderService>()
            .AddSingleton<ChannelService>()
            .AddSingleton<BotPersistenceService>()
            .AddSingleton<ReminderService.ReminderRemoverService>()
            .AddSingleton<UserHandler>()
            .AddSingleton<BotHandler>()
            .AddSingleton<BirthdayHandler>()
            .AddSingleton<PaginatorService>()
            .AddSingleton<ReminderHandler>();
        
        serviceCollection.Scan(scan => scan
            .FromAssemblyOf<Bot>()
            .AddClasses(classes => classes.InNamespaces("Catness.Services.Timed"))
            .UsingRegistrationStrategy(RegistrationStrategy.Append)
            .AsSelfWithInterfaces()
            .WithSingletonLifetime()
        );

        serviceCollection
            .AddSingleton<DiscordAttachmentClient>()
            .AddSingleton<MakesweetClient>()
            .AddSingleton<LastfmClient>()
            .AddSingleton<EmojiKitchenClient>();

        serviceCollection.AddSingleton(client);
        serviceCollection.AddSingleton(interactionService);
        serviceCollection.AddSingleton(interactiveService);
        serviceCollection.AddSingleton<BotService>();

        serviceCollection.AddDbContextFactory<CatnessDbContext>(optionsBuilder =>
            optionsBuilder.UseNpgsql(Configuration["DatabaseConfiguration:ConnectionString"]));

        serviceCollection.AddMemoryCache();

        return serviceCollection.BuildServiceProvider();
    }
}