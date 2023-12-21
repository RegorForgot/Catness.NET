using System.ComponentModel;
using Catness.Builders;
using Catness.Enums;
using Catness.Utilities;
using Discord;
using Discord.Interactions;

namespace Catness.Modules.Fun;

public class UserModule : InteractionModuleBase
{
    private ComponentBuilder GetButtonComponentBuilder(IUser user, AvatarDisplayType currentMode)
    {
        bool hasAvatar = !string.IsNullOrEmpty(user.AvatarId);
        bool hasGuildAvatar = false;
        if (user is IGuildUser guildUser)
        {
            hasGuildAvatar = hasAvatar && guildUser.DisplayAvatarId != user.AvatarId;
        }

        ButtonStyle globalAvatarStyle = hasAvatar ? ButtonStyle.Primary : ButtonStyle.Secondary;
        ButtonStyle guildAvatarStyle = hasGuildAvatar ? ButtonStyle.Primary : ButtonStyle.Secondary;

        ComponentBuilder builder = new ComponentBuilder()
            .WithButton(
                "Default",
                $"avatar_default_{user.Id}",
                disabled: currentMode == AvatarDisplayType.Default)
            .WithButton(
                "Global",
                $"avatar_global_{user.Id}",
                globalAvatarStyle,
                disabled: !hasAvatar || currentMode == AvatarDisplayType.Global)
            .WithButton(
                "Server",
                $"avatar_guild_{user.Id}",
                guildAvatarStyle,
                disabled: !hasGuildAvatar || currentMode == AvatarDisplayType.Guild)
            .WithButton(
                null,
                "close_interaction",
                ButtonStyle.Danger,
                EmoteCollection.CatnessEmotes.FirstOrDefault(emote => emote.Name == "close"));

        return builder;
    }

    private async Task<IUser> GetUser(ulong userId)
    {
        IGuild? guild = Context.Guild;

        if (guild is null)
        {
            return await Context.Client.GetUserAsync(userId);
        }

        return await guild.GetUserAsync(userId);
    }

    [SlashCommand("avatar", "Get a user's avatar")]
    public async Task GetAvatar(
        [Summary(description: "User to get the avatar for (defaults to you)")]
        IUser? user = null)
    {
        user ??= Context.User;

        if (user.AvatarId != null)
        {
            await RespondAsync(
                embed: AvatarBuilder.MakeAvatar(user, AvatarDisplayType.Global).Build(),
                components: GetButtonComponentBuilder(user, AvatarDisplayType.Global).Build()
            );
        }
        else
        {
            await RespondAsync(
                embed: AvatarBuilder.MakeAvatar(user, AvatarDisplayType.Default).Build(),
                components: GetButtonComponentBuilder(user, AvatarDisplayType.Default).Build()
            );
        }
    }

    [ComponentInteraction("avatar_global_*")]
    public async Task ShowGlobalAvatar(string userId)
    {
        IUser user = await GetUser(ulong.Parse(userId));

        IComponentInteraction interaction = (IComponentInteraction)Context.Interaction;

        await interaction.UpdateAsync(properties =>
            {
                properties.Embed = AvatarBuilder.MakeAvatar(user, AvatarDisplayType.Global).Build();
                properties.Components = GetButtonComponentBuilder(user, AvatarDisplayType.Global).Build();
            }
        );
    }

    [ComponentInteraction("avatar_guild_*")]
    public async Task ShowGuildAvatar(string userId)
    {
        IUser user = await GetUser(ulong.Parse(userId));
        IComponentInteraction interaction = (IComponentInteraction)Context.Interaction;

        await interaction.UpdateAsync(properties =>
            {
                properties.Embed = AvatarBuilder.MakeAvatar(user, AvatarDisplayType.Guild).Build();
                properties.Components = GetButtonComponentBuilder(user, AvatarDisplayType.Guild).Build();
            }
        );
    }

    [ComponentInteraction("avatar_default_*")]
    public async Task ShowDefaultAvatar(string userId)
    {
        IUser user = await GetUser(ulong.Parse(userId));
        IComponentInteraction interaction = (IComponentInteraction)Context.Interaction;

        await interaction.UpdateAsync(properties =>
            {
                properties.Embed = AvatarBuilder.MakeAvatar(user, AvatarDisplayType.Default).Build();
                properties.Components = GetButtonComponentBuilder(user, AvatarDisplayType.Default).Build();
            }
        );
    }

    [ComponentInteraction("close_interaction")]
    public async Task CloseInteraction()
    {
        IComponentInteraction interaction = (IComponentInteraction)Context.Interaction;

        await interaction.Message.DeleteAsync();
    }
}