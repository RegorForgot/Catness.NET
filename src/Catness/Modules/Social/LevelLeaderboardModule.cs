using System.Text;
using Catness.Persistence.Models;
using Catness.Services.EntityFramework;
using Discord;
using Discord.Interactions;

namespace Catness.Modules.Social;

[Group("level", "levelling commands")]
public class LevelLeaderboardModule : InteractionModuleBase
{
    private readonly GuildService _guildService;

    public LevelLeaderboardModule(
        GuildService guildService)
    {
        _guildService = guildService;

    }

    [SlashCommand("leaderboard", "Command to show the level leaderboard in your guild")]
    public async Task LevelLeaderboard()
    {
        List<GuildUser> userList = await _guildService.GetUsersSortedLevel(Context.Guild.Id, numberOfItems: 15);

        if (userList.Count == 0)
        {
            await RespondAsync(
                "There are no users with levelling in your server",
                ephemeral: true);
        }
        else
        {
            StringBuilder builder = new StringBuilder();

            int count = 1;

            foreach (GuildUser user in userList.TakeWhile(_ => count <= 15))
            {
                builder.AppendLine($"{count++}. <@{user.UserId}>: **Lvl.** {user.Level}, **Exp.** {user.Experience}");
            }

            Embed embed = new EmbedBuilder()
                .WithTitle($"Level leaderboard in {Context.Guild.Name}")
                .WithDescription(builder.ToString())
                .WithCurrentTimestamp()
                .Build();

            await RespondAsync(embed: embed);
        }
    }
}