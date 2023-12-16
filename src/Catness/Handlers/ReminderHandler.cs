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

    public async Task SendReminder(Reminder reminder)
    {
        Embed embed = new EmbedBuilder()
            .WithTitle($"Reminder <@{reminder.UserId}>!")
            .WithDescription(reminder.Body)
            .WithCurrentTimestamp()
            .Build();

        if (reminder.PrivateReminder)
        {
            if (_client.GetUser(reminder.UserId) is IUser user)
            {
                try
                {
                    await user.SendMessageAsync(embed: embed, allowedMentions: new AllowedMentions(AllowedMentionTypes.Users));
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
                    await channel.SendMessageAsync(embed: embed, allowedMentions: new AllowedMentions(AllowedMentionTypes.Users));
                }
                catch (HttpException) { }
            }
        }
    }
}