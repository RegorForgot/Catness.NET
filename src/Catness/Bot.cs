using System.Reflection;
using Catness.IO;
using Catness.Logging;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Catness;

public class Bot
{
    private readonly IServiceProvider _serviceProvider;

    public Bot(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MainAsync()
    {
        DiscordSocketClient client = _serviceProvider.GetService<DiscordSocketClient>()!;
        InteractionService interactionService = _serviceProvider.GetService<InteractionService>()!;
        IEnumerable<ILogProvider> logProviders = _serviceProvider.GetServices<ILogProvider>().ToList();
        IConfigFileService fileService = _serviceProvider.GetService<IConfigFileService>()!;
        
        
        foreach (ILogProvider logProvider in logProviders)
        {
            client.Log += logProvider.Log;
        }
        
        await interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), _serviceProvider);
        
        if (fileService.Configured)
        {
            await client.LoginAsync(TokenType.Bot, fileService.ConfigFile.DiscordToken);
            await client.StartAsync();
            await Task.Delay(Timeout.Infinite);
        }
        else
        {
            foreach (ILogProvider provider in logProviders)
            {
                if (provider is IConsoleLogProvider consoleProvider)
                {
                    await consoleProvider.Log("Please ensure your details are entered in the configuration file");
                }
            }
        }
    }
}