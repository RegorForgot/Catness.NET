using Newtonsoft.Json;

namespace Catness.Models.Lastfm;

public record FriendsResponse : ILastfmAPIResponse
{
    [JsonProperty("friends")]
    public FriendsContainer FriendsContainer { get; init; }
}

public record FriendsContainer
{
    [JsonProperty("@attr")]
    public FriendInfo UserFriends { get; init; }

    [JsonProperty("user")]
    public List<FriendUser> Friends { get; init; }

    public record FriendInfo
    {
        [JsonProperty("page")]
        public string PageNumber { get; init; }

        [JsonProperty("total")]
        public string FriendCount { get; init; }
    }
}

public record FriendUser
{
    [JsonProperty("name")]
    public string Username { get; init; }

    [JsonProperty("realname")]
    public string RealName { get; init; }

    [JsonProperty("susbcriber")]
    public string IsLastfmPro { get; init; }
}