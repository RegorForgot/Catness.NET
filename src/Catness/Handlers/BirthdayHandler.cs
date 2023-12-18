using Discord;
using Discord.Net;
using Discord.WebSocket;

namespace Catness.Handlers;

public class BirthdayHandler
{
    private readonly DiscordSocketClient _client;

    public BirthdayHandler(
        DiscordSocketClient client)
    {
        _client = client;
    }

    public async Task SendBirthday(ulong followerId, ulong followedId)
    {
        Embed embed = new EmbedBuilder()
            .WithTitle($"Birthday!")
            .WithDescription($"It's <@{followedId}>'s birthday!\nWish them a happy birthday!")
            .WithCurrentTimestamp()
            .Build();

        if (_client.GetUser(followerId) is IUser user)
        {
            try
            {
                await user.SendMessageAsync($"<@{followerId}>",
                    embed: embed,
                    allowedMentions: new AllowedMentions(AllowedMentionTypes.Users));
            }
            catch (HttpException) { }
        }
    }
}