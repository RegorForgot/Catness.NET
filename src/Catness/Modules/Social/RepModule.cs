using System.Text;
using Catness.Extensions;
using Catness.Persistence.Models;
using Catness.Services.EntityFramework;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Options;

namespace Catness.Modules.Social;

public class RepModule : InteractionModuleBase
{
    private readonly UserService _userService;
    private readonly BotConfiguration _botConfiguration;
    private static readonly TimeSpan Cooldown = TimeSpan.FromDays(1);

    public RepModule(
        UserService userService,
        IOptions<BotConfiguration> botOptions)
    {
        _userService = userService;
        _botConfiguration = botOptions.Value;
    }

    [SlashCommand("rep", "Add reputation to a user!")]
    public async Task GiveRep(
        IUser user)
    {
        if (user.Id == Context.User.Id)
        {
            await RespondAsync("You cannot give rep to yourself!");
            return;
        }

        User contextUser = await _userService.GetOrAddUser(Context.User.Id);

        DateTime lastRepTime = contextUser.LastRepTime ?? DateTime.MinValue;
        DateTime now = DateTime.UtcNow;

        StringBuilder response = new StringBuilder();

        if (!_botConfiguration.DiscordIDs.OwnerIDs.Contains(Context.User.Id) && now - lastRepTime < Cooldown)
        {
            TimestampTag timestamp = new TimestampTag(new DateTimeOffset(lastRepTime + Cooldown));

            response.AppendLine("You can only rep once every day!");
            response.AppendLine($"The next time you can do is {timestamp}");

            await RespondAsync(response.ToString());
            return;
        }

        User otherUser = await _userService.GetOrAddUser(user.Id);
        otherUser.Rep++;
        contextUser.LastRepTime = now;

        await _userService.UpdateUser(otherUser, contextUser);
        await RespondAsync($"Gave one rep to {user.Id.GetPingString()}!");
    }
}