using Catness.Enums;
using Catness.Persistence.Models;
using Catness.Services;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Options;

namespace Catness.Modules.Fun;

public class MakesweetModule : InteractionModuleBase
{
    private readonly BotConfiguration _configuration;
    private readonly MakesweetAPIService _makesweetAPIService;

    public MakesweetModule(IOptions<BotConfiguration> configuration, MakesweetAPIService makesweetAPIService)
    {
        _configuration = configuration.Value;
        _makesweetAPIService = makesweetAPIService;
    }
    
    [SlashCommand("ping", "ping")]
    public async Task Ping(MakesweetTemplate template, Attachment attachment)
    {
        Embed embed = new EmbedBuilder
        {
            Title = template.GetMakesweetURL(),
            Description = _configuration.APIKeys.MakesweetKey,
            ImageUrl = attachment.Url
        }.Build();
        await RespondAsync(embed:embed);
    }
}