using Newtonsoft.Json;

namespace Catness.Models.Lastfm;

public record RecentTrackResponse : ILastfmAPIResponse
{
    [JsonProperty("recenttracks")]
    public RecentTrackList RecentTracks { get; init; }

    public record RecentTrackList
    {
        [JsonProperty("track")]
        public List<Track> Tracks { get; init; }

        [JsonProperty("@attr")]
        public ResponseFooter Footer { get; init; }
    }

    public record ResponseFooter
    {
        [JsonProperty("total")]
        public string PlayCount { get; init; }
    }
}

public record Track
{
    [JsonProperty("name")]
    public string Name { get; init; } = string.Empty;

    [JsonProperty("artist")]
    public TrackArtist Artist { get; init; } = new TrackArtist();

    [JsonProperty("album")]
    public TrackAlbum Album { get; init; } = new TrackAlbum();

    [JsonProperty("@attr")]
    public TrackNowPlaying NowPlaying { get; init; } = new TrackNowPlaying();

    [JsonProperty("date")]
    public PlayDate Date { get; init; } = new PlayDate();

    [JsonProperty("image")]
    public List<AlbumImage> Images { get; init; } = new List<AlbumImage>(4);

    public record TrackArtist
    {
        [JsonProperty("#text")]
        public string Name { get; init; } = string.Empty;
    }

    public record TrackAlbum
    {
        [JsonProperty("#text")]
        public string Name { get; init; } = string.Empty;
    }

    public record AlbumImage
    {
        [JsonProperty("#text")]
        public string URL { get; init; }
    }

    public record TrackNowPlaying
    {
        [JsonProperty("nowplaying")]
        public string State { get; init; } = string.Empty;
    }

    public record PlayDate
    {
        [JsonProperty("uts")]
        public string Timestamp { get; init; } = string.Empty;
    }
}