using Catness.Persistence.Models;
using Catness.Services.EFServices;
using Discord;
using Discord.Interactions;

namespace Catness.Modules.Fun;

[Group("user", "User commands")]
public class UserCommandsModule : InteractionModuleBase
{
    private readonly UserService _userService;
    private readonly FollowService _followService;

    public UserCommandsModule(UserService userService,
        FollowService followService)
    {
        _userService = userService;
        _followService = followService;
    }
    
    [SlashCommand("avatar", "Get a user's avatar")]
    public async Task GetAvatar(
        [Summary(description: "User to get the avatar for")]
        IUser? user = null,
        [Summary(description: "Server avatar")]
        bool server = false,
        [Summary(description: "Whether the embed response should be private ")]
        bool ephemeral = false)
    {
        user ??= Context.User;

        string imageUrl;

        if (user is IGuildUser guildUser && server)
        {
            imageUrl = guildUser.GetDisplayAvatarUrl(ImageFormat.Auto, 2048);
        }
        else
        {
            imageUrl = user.GetAvatarUrl(ImageFormat.Auto, 2048);
        }

        Embed embed = new EmbedBuilder
        {
            Title = $"{user.Username}'s avatar",
            ImageUrl = imageUrl
        }.Build();

        await RespondAsync(embed: embed, ephemeral: ephemeral);
    }

    [SlashCommand("follow", "Follow another user")]
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
}