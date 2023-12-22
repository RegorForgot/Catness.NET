using Catness.Enums;
using Catness.Extensions;
using Catness.Persistence.Models;
using Catness.Services.EntityFramework;
using Discord;
using Discord.Interactions;

namespace Catness.Modules.Social;

[Group("birthday", "Commands to set your birthday!")]
public class BirthdayModule : InteractionModuleBase
{
    private readonly UserService _userService;

    public BirthdayModule(UserService userService)
    {
        _userService = userService;
    }

    [SlashCommand("set", "Set your birthday")]
    public async Task SetBirthday(
        [ComplexParameter] BirthdayType birthday)
    {
        DateOnly date;

        try
        {
            date = birthday.GetDate();
        }
        catch (ArgumentOutOfRangeException ex)
        {
            await RespondAsync($"Error! {ex.Message}");
            return;
        }

        User user = await _userService.GetOrAddUser(Context.User.Id);

        if (user.Birthday == date)
        {
            await RespondAsync($"Birthday provided is equal, not removing birthday");
            return;
        }

        user.Birthday = date;
        await _userService.UpdateUser(user);

        string response = user.Birthday is null ? $"Added birthday as {date}" : $"Updated birthday to {date}";

        await RespondAsync(response);
    }

    [SlashCommand("get", "Get someone's birthday!")]
    public async Task GetBirthday(
        IUser? user = null)
    {
        user ??= Context.User;

        string username;

        if (user is IGuildUser guildUser)
        {
            username = guildUser.Nickname ?? guildUser.DisplayName ?? guildUser.Username;
        }
        else
        {
            username = user.Username;
        }

        User? userDb = await _userService.GetUser(user.Id);

        if (userDb?.Birthday is null)
        {
            await RespondAsync($"{username} has not set a birthday.", ephemeral: true);
        }
        else
        {
            await RespondAsync(
                $"{user.Id.GetPingString()}'s birthday is on {userDb.Birthday}!",
                allowedMentions: AllowedMentions.None);
        }
    }

    [SlashCommand("unset", "Remove your birthday")]
    public async Task RemoveBirthday()
    {
        User? user = await _userService.GetUser(Context.User.Id);

        if (user?.Birthday is null)
        {
            await RespondAsync("You have not set a birthday.");
        }
        else
        {
            user.Birthday = null;
            await _userService.UpdateUser(user);
            await RespondAsync("Removed birthday");
        }
    }
}