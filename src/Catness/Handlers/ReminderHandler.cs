using Catness.Persistence.Models;
using Discord;
using Discord.Net;
using Discord.WebSocket;

namespace Catness.Handlers;

public class ReminderHandler
{
    private readonly DiscordSocketClient _client;

    public ReminderHandler(
        DiscordSocketClient client)
    {
        _client = client;
    }

    public async Task SendReminder(Reminder reminder, bool late = false)
    {
        EmbedBuilder embedBuilder = new EmbedBuilder()
            .WithTitle($"Reminder!")
            .WithDescription($"{reminder.Body}")
            .WithCurrentTimestamp();

        if (late)
        {
            embedBuilder = embedBuilder.WithFooter("Sorry for the late reminder!");
        }

        Embed embed = embedBuilder.Build();

        if (reminder.PrivateReminder)
        {
            if (_client.GetUser(reminder.UserId) is IUser user)
            {
                try
                {
                    await user.SendMessageAsync($"<@{reminder.UserId}", embed: embed, allowedMentions: new AllowedMentions(AllowedMentionTypes.Users));
                }
                catch (HttpException) { }
            }
        }
        else
        {
            if (_client.GetChannel(reminder.ChannelId ?? 0) is ITextChannel channel)
            {

                try
                {
                    await channel.SendMessageAsync($"<@{reminder.UserId}", embed: embed, allowedMentions: new AllowedMentions(AllowedMentionTypes.Users));
                }
                catch (HttpException) { }
            }
        }
    }
}