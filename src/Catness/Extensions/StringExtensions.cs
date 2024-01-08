using Discord;

namespace Catness.Extensions;

public static class StringExtensions
{
    public static string? Truncate(this string? value, int maxChars)
        =>
            string.IsNullOrEmpty(value) ? value :
            value.Length <= maxChars ? value :
            value[..maxChars] + "...";

    public static void ReplaceOrAddToDictionary(this Dictionary<string, string> currentDictionary, Dictionary<string, string> optionsToAdd)
    {
        foreach (var optionToAdd in optionsToAdd)
        {
            if (!currentDictionary.TryGetValue(optionToAdd.Key, out _))
            {
                currentDictionary.Add(optionToAdd.Key, optionToAdd.Value);
            }
        }
    }
        
    public static string IsoCountryCodeToFlagEmoji(this string country)
    {
        return string.Concat(country.ToUpper().Select(x => char.ConvertFromUtf32(x + 0x1F1A5)));
    }

    public static string GetEmojiCodePoint(this string emoji)
    {
        for (int i = 0; i < emoji.Length; i += char.IsSurrogatePair(emoji, i) ? 2 : 1)
        {
            int codepoint = char.ConvertToUtf32(emoji, i);

            return $"{codepoint:X4}".ToLower();
        }
        return string.Empty;
    }
    
    public static string GetEmojiCodePoint(this Emoji emoji)
    {
        return emoji.ToString().GetEmojiCodePoint();
    }
}