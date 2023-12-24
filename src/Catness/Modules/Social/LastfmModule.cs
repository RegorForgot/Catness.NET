using Catness.Clients;
using Catness.Exceptions;
using Catness.Models.Lastfm;
using Catness.Persistence.Models;
using Catness.Services.EntityFramework;
using Discord.Interactions;
using User = Catness.Persistence.Models.User;

namespace Catness.Modules.Social;

[Group("lastfm", "Social last.fm commands")]
public class LastfmModule : InteractionModuleBase
{
    private readonly UserService _userService;
    private readonly LastfmClient _lastfmClient;

    public LastfmModule(UserService userService,
        LastfmClient lastfmClient)
    {
        _userService = userService;
        _lastfmClient = lastfmClient;
    }

    [SlashCommand("set", "Set your last.fm username")]
    public async Task SetLastfmUsername(string lastfmUsername)
    {
        if (lastfmUsername.Length > 15)
        {
            await RespondAsync("Please enter a valid Last.fm username",
                ephemeral: true);
            return;
        }

        User user = await _userService.GetOrAddUser(Context.User.Id);

        if (user.LastfmUsername == lastfmUsername)
        {
            await RespondAsync("This is your current username!",
                ephemeral: true);
            return;
        }

        try
        {
            await _lastfmClient.GetUserInfo(lastfmUsername);
        }
        catch (LastfmAPIException ex)
        {
            if (ex.ErrorCode == LastfmErrorCode.InvalidParam)
            {
                await RespondAsync("A user does not exist with that name. Please enter a valid username.",
                    ephemeral: true);
                return;
            }

            await RespondAsync($"An error occured, please try again: {ex.ErrorCode}: {ex.Message}",
                ephemeral: true);
            return;
        }

        user.LastfmUsername = lastfmUsername;
        await _userService.UpdateUser(user);

        await RespondAsync($"Updated Last.fm username to {lastfmUsername}!");
    }
}