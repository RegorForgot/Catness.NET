using Catness.Enums;
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
    public async Task Ping(MakesweetTemplate template)
    {
        Embed embed = new EmbedBuilder
        {
            Title = template.GetMakesweetURL(),
            Description = _fileService.ConfigFile.MakesweetKey
        }.Build();
        await RespondAsync(embed:embed);
    }
}