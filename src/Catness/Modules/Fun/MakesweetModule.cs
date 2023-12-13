using Catness.Enums;
using Catness.Persistence;
using Catness.Persistence.Models;
using Catness.Services;
using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Catness.Modules.Fun;

public class MakesweetModule : InteractionModuleBase
{
    private readonly BotConfiguration _configuration;
    private readonly IDbContextFactory<CatnessDbContext> _dbContextFactory;
    private readonly MakesweetAPIService _makesweetAPIService;

    public MakesweetModule(
        IOptions<BotConfiguration> configuration,
        IDbContextFactory<CatnessDbContext> dbContextFactory,
        MakesweetAPIService makesweetAPIService)
    {
        _configuration = configuration.Value;
        _dbContextFactory = dbContextFactory;
        _makesweetAPIService = makesweetAPIService;
    }

    [SlashCommand("ping", "ping")]
    public async Task Ping(MakesweetTemplate template, Attachment attachment)
    {
        await using CatnessDbContext dbContext = await _dbContextFactory.CreateDbContextAsync();
        
        Embed embed = new EmbedBuilder
        {
            Title = template.GetMakesweetURL(),
            Description = dbContext.Users.Count().ToString(),
            ImageUrl = attachment.Url
        }.Build();
        await RespondAsync(embed: embed);
    }
}