﻿using Catness.Handlers;
using Catness.IO;
using Catness.Persistence;
using Catness.Persistence.Models;
using Catness.Services;
using Catness.Services.EFServices;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
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

        DiscordSocketConfig config = new DiscordSocketConfig
        {
            GatewayIntents =
                GatewayIntents.AllUnprivileged &
                ~GatewayIntents.GuildScheduledEvents &
                ~GatewayIntents.GuildInvites
        };

        DiscordSocketClient client = new DiscordSocketClient(config);

        serviceCollection
            .AddSingleton<MakesweetAPIService>()
            .AddSingleton<GuildService>()
            .AddSingleton<FollowService>()
            .AddSingleton<UserService>()
            .AddSingleton<UserHandler>()
            .AddSingleton<BotHandler>();
        serviceCollection.AddSingleton(client);
        serviceCollection.AddSingleton<InteractionService>();
        serviceCollection.AddSingleton<BotService>();

        serviceCollection.AddDbContextFactory<CatnessDbContext>(optionsBuilder =>
            optionsBuilder.UseNpgsql(Configuration["DatabaseConfiguration:ConnectionString"]));

        serviceCollection.AddMemoryCache();
        
        return serviceCollection.BuildServiceProvider();
    }
}