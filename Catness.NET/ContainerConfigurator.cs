using System.Reflection;
using Autofac;
using Catness.NET.IO;
using Discord.Interactions;
using Discord.WebSocket;

namespace Catness.NET;

public static class ContainerConfigurator
{
    public static IContainer ConfigureContainer()
    {
        ContainerBuilder containerBuilder = new ContainerBuilder();

        containerBuilder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            .Where(t => t.Namespace.Contains("Logging"))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
        
        containerBuilder.RegisterType<ConfigIOService>().As<AbstractIOService>().AsSelf().InstancePerLifetimeScope();
        containerBuilder.RegisterType<DiscordSocketClient>().SingleInstance();
        containerBuilder.RegisterType<InteractionService>().SingleInstance();
        containerBuilder.RegisterType<Bot>().SingleInstance();


        return containerBuilder.Build();
    }
}