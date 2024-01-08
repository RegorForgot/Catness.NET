using Newtonsoft.Json;

namespace Catness.Models.EmojiKitchen;

public record EmojiCombination
{
    [JsonProperty("gStaticUrl")]
    public string URL { get; init; }
    
    [JsonProperty("rightEmojiCodepoint")]
    public string RightEmoji { get; init; }
    
    [JsonProperty("leftEmojiCodepoint")]
    public string LeftEmoji { get; init; }
}