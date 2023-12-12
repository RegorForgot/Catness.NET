namespace Catness;

public class Program
{
    public static Task Main(string[] args)
    {
        return new Bot(ServiceConfigurator.CreateServiceProvider()).MainAsync();
    }
}