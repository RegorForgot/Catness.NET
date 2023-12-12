using Catness.Enums;
using Catness.IO;
using Catness.Services;
using Discord;
using Discord.Interactions;

namespace Catness.Modules.Fun;

public class MakesweetModule : InteractionModuleBase
{
    private readonly IConfigFileService _fileService;
    private readonly MakesweetAPIService _makesweetAPIService;

    public MakesweetModule(IConfigFileService fileService, MakesweetAPIService makesweetAPIService)
    {
        _fileService = fileService;
        _makesweetAPIService = makesweetAPIService;
    }
    
    [SlashCommand("ping", "ping")]
    public async Task Ping(MakesweetTemplate template, Attachment attachment)
    {
        Embed embed = new EmbedBuilder
        {
            Title = template.GetMakesweetURL(),
            Description = _fileService.ConfigFile.MakesweetKey,
            ImageUrl = attachment.Url
        }.Build();
        await RespondAsync(embed:embed);
    }
}