using Discord;
using Discord.Interactions;

namespace Catness.Modules.Fun;

public class UserModule : InteractionModuleBase
{
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
}