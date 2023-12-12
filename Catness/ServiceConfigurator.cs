using Catness.IO;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace Catness;

public static class ServiceConfigurator
{
    public static IServiceProvider CreateServiceProvider()
    {
        IServiceCollection serviceCollection = new ServiceCollection()
            .Scan(scan => scan
                .FromAssemblyOf<Bot>()

                .AddClasses(classes => classes.InNamespaces("Catness.Logging"))
                .UsingRegistrationStrategy(RegistrationStrategy.Append)
                .AsSelfWithInterfaces()
                .WithSingletonLifetime()
            );

        serviceCollection.AddSingleton<IConfigFileService, ConfigFileService>();
        serviceCollection.AddSingleton<DiscordSocketClient>();
        serviceCollection.AddSingleton<InteractionService>();
        
        return serviceCollection.BuildServiceProvider();
    }
}