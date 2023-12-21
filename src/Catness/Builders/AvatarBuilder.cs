using Catness.Enums;
using Discord;

namespace Catness.Builders;

public class AvatarBuilder
{
    public static EmbedBuilder MakeAvatar(IUser user, AvatarDisplayType type)
    {
        IGuildUser? guildUser = user as IGuildUser;

        string? imageUrl = type switch
        {
            AvatarDisplayType.Global => user.GetAvatarUrl(ImageFormat.Auto, 2048),
            AvatarDisplayType.Guild => guildUser?.GetDisplayAvatarUrl(ImageFormat.Auto, 2048),
            AvatarDisplayType.Default => user.GetDefaultAvatarUrl(),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        return new EmbedBuilder()
            .WithCurrentTimestamp()
            .WithColor(Color.Blue)
            .WithAuthor(
                new EmbedAuthorBuilder()
                    .WithName(user.Username)
            )
            .WithImageUrl(imageUrl);
    }
}