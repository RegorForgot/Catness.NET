using Catness.Exceptions;
using Catness.Models.Lastfm;
using Catness.Persistence.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;

namespace Catness.Clients;

public class LastfmClient : IRestClient
{
    private const string RecentTracksSignature = "user.getrecenttracks";
    private const string UserInfoSignature = "user.getinfo";
    private const string GetFriendsSignature = "user.getfriends";

    private readonly BotConfiguration _botConfiguration;

    public RestClient Client { get; }
    public string BaseUrl => "https://ws.audioscrobbler.com/2.0/";

    public LastfmClient(IOptions<BotConfiguration> botConfiguration)
    {
        Client = new RestClient(BaseUrl);
        _botConfiguration = botConfiguration.Value;
        Client.AddDefaultHeader("User-Agent", "Catness.NET");
    }

    public async Task<RecentTrackResponse> GetLastTrack(string username)
    {
        RestRequest request = new RestRequest();

        request.AddParameter("format", "json");
        request.AddParameter("method", RecentTracksSignature);
        request.AddParameter("limit", "1");
        request.AddParameter("user", username);
        request.AddParameter("api_key", _botConfiguration.APIKeys.LastFMKey);
        request.Timeout = 20000;

        RestResponse response = await Client.ExecuteGetAsync(request).ConfigureAwait(false);

        return GetResponse<RecentTrackResponse>(response);
    }

    public async Task<UserInfoResponse> GetUserInfo(string username)
    {
        RestRequest request = new RestRequest();

        request.AddParameter("format", "json");
        request.AddParameter("method", UserInfoSignature);
        request.AddParameter("user", username);
        request.AddParameter("api_key", _botConfiguration.APIKeys.LastFMKey);
        request.Timeout = 20000;

        RestResponse response = await Client.ExecuteGetAsync(request).ConfigureAwait(false);

        return GetResponse<UserInfoResponse>(response);
    }

    public async Task<FriendsResponse> GetUserFriends(string username, int pageNumber = 1)
    {
        RestRequest request = new RestRequest();

        request.AddParameter("format", "json");
        request.AddParameter("method", GetFriendsSignature);
        request.AddParameter("user", username);
        request.AddParameter("page", pageNumber);
        request.AddParameter("api_key", _botConfiguration.APIKeys.LastFMKey);

        RestResponse response = await Client.ExecuteGetAsync(request).ConfigureAwait(false);
        return GetResponse<FriendsResponse>(response);
    }

    private static T GetResponse<T>(RestResponseBase response) where T : ILastfmAPIResponse
    {
        if (response.Content == null)
        {
            throw new HttpRequestException(Enum.GetName(response.StatusCode), null, response.StatusCode);
        }

        LastfmErrorResponse e = JsonConvert.DeserializeObject<LastfmErrorResponse>(response.Content)!;
        if (e.Error != LastfmErrorCode.OK)
        {
            throw new LastfmAPIException(e.Message, e.Error);
        }

        return JsonConvert.DeserializeObject<T>(response.Content)!;
    }
}