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

    public virtual bool Equals(EmojiCombination? other)
    {
        return other?.LeftEmoji == LeftEmoji && other.RightEmoji == RightEmoji;
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(RightEmoji, LeftEmoji);
    }
}