using System.Text;
using Catness.Persistence.Models;
using Catness.Services.EFServices;
using Discord;
using Discord.Interactions;

namespace Catness.Modules.Social;

[Group("follow", "Follow another user")]
public class FollowModule : InteractionModuleBase
{
    private readonly UserService _userService;
    private readonly FollowService _followService;

    public FollowModule(
        UserService userService,
        FollowService followService)
    {
        _userService = userService;
        _followService = followService;
    }

    [SlashCommand("add", "Follow another user")]
    public async Task FollowUser(
        [Summary(description: "User to follow")]
        IUser user)
    {
        if (user == Context.User)
        {
            await RespondAsync("You cannot follow yourself!", ephemeral: true);
            return;
        }

        if (user is ISelfUser)
        {
            await RespondAsync("You cannot follow me", ephemeral: true);
            return;
        }

        _ = await _userService.GetOrAddUser(user.Id);
        Follow? follow = await _followService.GetFollow(Context.User.Id, user.Id);

        if (follow is null)
        {
            await _followService.AddFollow(Context.User.Id, user.Id);
            await RespondAsync($"Started following <@{user.Id}>!", allowedMentions: new AllowedMentions(AllowedMentionTypes.Users));
        }
        else
        {
            await RespondAsync($"You are already following <@{user.Id}>.", allowedMentions: AllowedMentions.None);
        }
    }

    [SlashCommand("remove", "Remove a follower from your following list")]
    public async Task UnfollowUser(
        IUser user)
    {
        Follow? follow = await _followService.GetFollow(Context.User.Id, user.Id);

        if (follow is null)
        {
            await RespondAsync($"You are not following <@{user.Id}>.",
                allowedMentions: new AllowedMentions(AllowedMentionTypes.Users),
                ephemeral: true);
        }
        else
        {
            await _followService.RemoveFollow(follow);
            await RespondAsync($"You are no longer following <@{user.Id}>.",
                allowedMentions: AllowedMentions.None,
                ephemeral: true);
        }
    }

    [SlashCommand("following", "Get the list of people you are following")]
    public async Task GetFollowing()
    {
        User? user = await _userService.GetUserWithFollows(Context.User.Id);

        if (user is null || user.Following.Count == 0)
        {
            await RespondAsync("You are not following anyone.",
                ephemeral: true);
        }
        else
        {
            StringBuilder builder = new StringBuilder();

            int count = 1;

            foreach (Follow follow in user.Following)
            {
                builder.AppendLine($"{count++}. <@{follow.FollowedId}>");
            }

            Embed embed = new EmbedBuilder()
                .WithTitle("List of users you are following")
                .WithDescription(builder.ToString())
                .WithFooter($"Following {user.Following.Count} users")
                .WithCurrentTimestamp()
                .Build();

            await RespondAsync(embed: embed);
        }
    }

    [SlashCommand("followers", "Get the list of people you are followed by")]
    public async Task GetFollowers()
    {
        User? user = await _userService.GetUserWithFollows(Context.User.Id);

        if (user is null || user.Followers.Count == 0)
        {
            await RespondAsync("You are not followed by anyone.",
                ephemeral: true);
        }
        else
        {
            StringBuilder builder = new StringBuilder();

            int count = 1;

            foreach (Follow follow in user.Followers)
            {
                builder.AppendLine($"{count++}. {follow.FollowerId}");
            }

            Embed embed = new EmbedBuilder()
                .WithTitle("List of users you are followed by")
                .WithDescription(builder.ToString())
                .WithFooter($"Followed by <@{user.Followers.Count}> users")
                .WithCurrentTimestamp()
                .Build();

            await RespondAsync(embed: embed);
        }
    }
}