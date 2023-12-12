using Catness.IO;
using Discord;
using Discord.Interactions;

namespace Catness.Modules;

public class PingModule : InteractionModuleBase
{
    private readonly IConfigFileService _fileService;
    
    public PingModule(IConfigFileService fileService)
    {
        _fileService = fileService;
    }
    
    [SlashCommand("ping", "ping")]
    public async Task Ping()
    {
        Embed embed = new EmbedBuilder
        {
            Title = "Testing",
            Description = _fileService.ConfigFile.MakesweetKey
        }.Build();
        await RespondAsync(embed:embed);
    }
}