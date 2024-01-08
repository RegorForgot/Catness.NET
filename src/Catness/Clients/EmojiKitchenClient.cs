using System.Text;
using Catness.Extensions;
using Catness.Models.EmojiKitchen;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Catness.Clients;

public class EmojiKitchenClient : IRestClient
{
    public RestClient Client { get; }
    public string BaseUrl => "https://raw.githubusercontent.com/xsalazar/emoji-kitchen/main/src/Components/metadata.json";

    public EmojiKitchenClient()
    {
        Client = new RestClient(BaseUrl);
        Client.AddDefaultHeader("User-Agent", "Catness.NET");
    }

    public async Task<HashSet<EmojiCombination>?> GetEmojiCombinationSet()
    {
        RestRequest request = new RestRequest
        {
            Timeout = 20000
        };

        byte[]? response = await Client.DownloadDataAsync(request);
        if (response == null)
        {
            return null;
        }

        string downloadedEmojiData = Encoding.UTF8.GetString(response);
        var emojiData = JsonConvert.DeserializeObject<Dictionary<string, object>>(downloadedEmojiData);

        if (emojiData is null)
        {
            return null;
        }


        JObject dataDictionary = (JObject)emojiData["data"];
        var test = dataDictionary.ToObject<Dictionary<string, Dictionary<string, object>>>();

        var emojiCombinations = new HashSet<EmojiCombination>();
        
        foreach (JArray array in test!.Keys.Select(key => (JArray)test[key]["combinations"]))
        {
            emojiCombinations.TryAddAll(array.ToObject<HashSet<EmojiCombination>>() ?? []);
        }

        return emojiCombinations;
    }
}