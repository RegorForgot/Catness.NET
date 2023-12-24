using Newtonsoft.Json;

namespace Catness.Models.Lastfm;

public record LastfmErrorResponse : ILastfmAPIResponse
{
    [JsonProperty("message")]
    public string Message { get; init; } = "OK";

    [JsonProperty("error")] 
    public LastfmErrorCode Error { get; init; } = LastfmErrorCode.OK;
}