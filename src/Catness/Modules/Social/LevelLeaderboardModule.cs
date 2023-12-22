using System.Text;
using Catness.Extensions;
using Catness.Persistence.Models;
using Catness.Services.EntityFramework;
using Catness.Utilities;
using Discord;
using Discord.Interactions;
using Fergun.Interactive;
using Fergun.Interactive.Pagination;

namespace Catness.Modules.Social;

[Group("level", "levelling commands")]
public class LevelLeaderboardModule : InteractionModuleBase
{
    private readonly GuildService _guildService;
    private readonly InteractiveService _interactiveService;

    public LevelLeaderboardModule(
        GuildService guildService,
        InteractiveService interactiveService)
    {
        _guildService = guildService;
        _interactiveService = interactiveService;
    }

    [SlashCommand("leaderboard", "Command to show the level leaderboard in your guild")]
    public async Task LevelLeaderboard()
    {
        List<GuildUser> userList = await _guildService.GetUsersSortedLevel(Context.Guild.Id);
        List<GuildUser[]> userPages = userList.Chunk(15).ToList();

        List<PageBuilder> pageBuilders = [];


        if (userList.Count == 0)
        {
            await RespondAsync(
                "There are no users with levelling in your server",
                ephemeral: true);
        }
        else
        {
            StaticPaginator paginator;
            int count = 1;

            foreach (GuildUser[] page in userPages)
            {
                StringBuilder builder = new StringBuilder();

                foreach (GuildUser user in page)
                {
                    builder.AppendLine($"{count++}. {user.UserId.GetPingString()}: **Lvl.** {user.Level}, **Exp.** {user.Experience}");
                }

                pageBuilders.Add(new PageBuilder()
                    .WithTitle($"Level leaderboard in {Context.Guild.Name}")
                    .WithThumbnailUrl(Context.Guild.IconUrl)
                    .WithDescription(builder.ToString())
                    .WithCurrentTimestamp()
                );
            }

            IDictionary<IEmote, PaginatorAction> paginatorButtons = new Dictionary<IEmote, PaginatorAction>
            {
                { EmoteCollection.CatnessEmotes["page_left"], PaginatorAction.Backward },
                { EmoteCollection.CatnessEmotes["page_right"], PaginatorAction.Forward },
            };

            paginator = new StaticPaginatorBuilder()
                .WithPages(pageBuilders)
                .WithActionOnTimeout(ActionOnStop.DeleteInput)
                .WithUsers(Context.User)
                .WithOptions(paginatorButtons)
                .Build();
            
            await _interactiveService.SendPaginatorAsync(paginator, Context.Interaction);
        }
    }
}