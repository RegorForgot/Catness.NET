using Newtonsoft.Json;

namespace Catness.Models.Lastfm;

public record UserInfoResponse : ILastfmAPIResponse { }

public record User
{
    [JsonProperty("age")]
    public string Age { get; init; }

    [JsonProperty("subscriber")]
    public string IsLastfmPro { get; init; }

    [JsonProperty("playcount")]
    public string PlayCount { get; init; }

    [JsonProperty("artist_count")]
    public string ArtistCount { get; init; }

    [JsonProperty("album_count")]
    public string AlbumCount { get; init; }

    [JsonProperty("track_count")]
    public string TrackCount { get; init; }

    [JsonProperty("image")]
    public List<UserImage> Images { get; init; } = new List<UserImage>(4);

    [JsonProperty("registered")]
    public RegistrationTime Registered { get; init; }

    [JsonProperty("country")]
    public string Country { get; init; }

    public record UserImage
    {
        private const string DefaultProfilePicture =
            @"https://lastfm.freetls.fastly.net/i/u/avatar170s/818148bf682d429dc215c1705eb27b98.png";

        private string _imageUrl = DefaultProfilePicture;

        [JsonProperty("size")]
        public string Size { get; init; }

        [JsonProperty("#text")]
        public string ImageUrl
        {
            get => _imageUrl;
            init
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _imageUrl = value;
                }
            }
        }
    }

    public record RegistrationTime
    {
        [JsonProperty("unixtime")]
        public string UnixTimestamp { get; init; }
    }
}