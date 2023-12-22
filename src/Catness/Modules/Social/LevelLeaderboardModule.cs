using System.Text;
using Catness.Extensions;
using Catness.Persistence.Models;
using Catness.Services;
using Catness.Services.EntityFramework;
using Discord.Interactions;
using Fergun.Interactive;

namespace Catness.Modules.Social;

[Group("level", "levelling commands")]
public class LevelLeaderboardModule : InteractionModuleBase
{
    private readonly GuildService _guildService;
    private readonly PaginatorService _paginatorService;

    public LevelLeaderboardModule(
        GuildService guildService,
        PaginatorService paginatorService)
    {
        _guildService = guildService;
        _paginatorService = paginatorService;
    }

    [SlashCommand("leaderboard", "Command to show the level leaderboard in your guild")]
    public async Task LevelLeaderboard()
    {
        List<GuildUser> userList = await _guildService.GetUsersSortedLevel(Context.Guild.Id);

        if (userList.Count == 0)
        {
            await RespondAsync(
                "There are no users with levelling in your server",
                ephemeral: true);
        }
        else
        {
            List<GuildUser[]> userPages = userList.Chunk(15).ToList();
            List<PageBuilder> pageBuilders = [];

            int count = 1;

            foreach (GuildUser[] page in userPages)
            {
                StringBuilder builder = new StringBuilder();

                foreach (GuildUser user in page)
                {
                    builder.AppendLine($"{count++}. {user.UserId.GetPingString()}: **Lvl.** `{user.Level}`, **Exp.** `{user.Experience}`");
                }

                pageBuilders.Add(new PageBuilder()
                    .WithTitle($"Level leaderboard in {Context.Guild.Name}")
                    .WithThumbnailUrl(Context.Guild.IconUrl)
                    .WithDescription(builder.ToString())
                    .WithCurrentTimestamp()
                );
            }

            await _paginatorService.SendPaginator(pageBuilders, Context);
        }
    }
}