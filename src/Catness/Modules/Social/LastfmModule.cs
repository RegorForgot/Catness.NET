using System.Text;
using Catness.Clients;
using Catness.Enums;
using Catness.Exceptions;
using Catness.Models.Lastfm;
using Catness.Services.EntityFramework;
using Discord;
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

    [SlashCommand("link", "Set your last.fm username")]
    public async Task SetLastfmUsername(
        [Summary("username", "your Last.fm username")]
        string lastfmUsername)
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
            UserInfoResponse? response = await LastfmClient.GetLastfmResponse(_lastfmClient.GetUserInfoRequest, lastfmUsername);

            if (response == null)
            {
                await RespondAsync("There is an error with Last.fm, please try again later.",
                    ephemeral: true);
                return;
            }
        }
        catch (LastfmAPIException ex)
        {
            await HandleLastfmException(ex);
            return;
        }

        user.LastfmUsername = lastfmUsername;
        await _userService.UpdateUser(user);

        await RespondAsync($"Updated Last.fm username to {lastfmUsername}!",
            ephemeral: true);
    }

    [SlashCommand("now-playing", "get your current listening")]
    public async Task GetCurrentPlaying(
        string? username = null)
    {
        if (username is null)
        {
            User? user = await _userService.GetUser(Context.User.Id);

            username = user?.LastfmUsername;
            if (username is null)
            {
                await RespondAsync("Please input a Last.fm username or set your own using " +
                                   @"`\lastfm link`", ephemeral: true);
                return;
            }
        }

        RecentTrackResponse? recentTracks;
        try
        {
            recentTracks = await LastfmClient.GetLastfmResponse(_lastfmClient.GetLastTrackRequest, username);

            if (recentTracks is null)
            {
                await RespondAsync("There is an error with Last.fm, please try again later.",
                    ephemeral: true);
                return;
            }
        }
        catch (LastfmAPIException ex)
        {
            await HandleLastfmException(ex);
            return;
        }

        if (recentTracks.RecentTracks.Tracks.Count == 0)
        {
            await RespondAsync($"{username} does not have any previous tracks!",
                ephemeral: true);
            return;
        }

        Track currentTrack = recentTracks.RecentTracks.Tracks[0];
        StringBuilder artistAlbum = new StringBuilder($"By {currentTrack.Artist.Name}");

        if (!string.IsNullOrEmpty(currentTrack.Album.Name))
        {
            artistAlbum.Append($" | On {currentTrack.Album.Name}");
        }

        EmbedBuilder builder = new EmbedBuilder()
            .WithAuthor($"{username} - Now playing")
            .WithThumbnailUrl(currentTrack.Images[3].URL)
            .WithCurrentTimestamp()
            .WithColor(Color.Red)
            .WithTitle(currentTrack.Name)
            .WithDescription(artistAlbum.ToString());

        await RespondAsync(embed: builder.Build());
    }

    private async Task HandleLastfmException(LastfmAPIException ex)
    {
        if (ex.ErrorCode == LastfmErrorCode.InvalidParam)
        {
            await RespondAsync("A user does not exist with that name. Please enter a valid username.",
                ephemeral: true);
            return;
        }

        await RespondAsync($"An error occured, please try again: {ex.ErrorCode}: {ex.Message}",
            ephemeral: true);
    }
}