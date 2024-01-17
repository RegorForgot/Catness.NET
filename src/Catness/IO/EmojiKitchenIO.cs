using System.Runtime.Serialization.Formatters.Binary;
using Catness.Models.EmojiKitchen;
using Newtonsoft.Json;

namespace Catness.IO;

public static class EmojiKitchenIO
{
    private const string FileName = "emoji.json";

    public static void WriteEmojis(HashSet<EmojiCombination> emojiCombinations)
    {
        string dataJson = JsonConvert.SerializeObject(emojiCombinations, Formatting.Indented);
        File.WriteAllText(FileName, dataJson);
    }

    public static IEnumerable<EmojiCombination>? ReadEmojis()
    {
        if (!File.Exists(FileName))
        {
            return null;
        }
        string dataJson = File.ReadAllText(FileName);
        try
        {
            return JsonConvert.DeserializeObject<HashSet<EmojiCombination>>(dataJson);
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception.Message);
            return null;
        }
    }
}