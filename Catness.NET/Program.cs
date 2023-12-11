using Autofac;

namespace Catness.NET;

public class Program
{
    public static Task Main(string[] args)
    {
        IContainer container = ContainerConfigurator.ConfigureContainer();
        using ILifetimeScope scope = container.BeginLifetimeScope();
        
        Bot bot = scope.Resolve<Bot>();
        return bot.MainAsync();
    }
}