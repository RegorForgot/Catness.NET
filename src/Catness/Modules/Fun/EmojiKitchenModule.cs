using Catness.Extensions;
using Catness.Models.EmojiKitchen;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Caching.Memory;

namespace Catness.Modules.Fun;

public class EmojiKitchenModule : InteractionModuleBase
{
    private readonly IMemoryCache _memoryCache;

    public EmojiKitchenModule(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    [SlashCommand("emoji-kitchen", "Mix emojis together!")]
    public async Task EmojiKitchen(
        [Summary(name: "first-emoji")] string emoji1,
        [Summary(name: "second-emoji")] string emoji2)
    {
        try
        {
            bool valid = Emoji.TryParse(emoji1, out Emoji leftEmoji) & Emoji.TryParse(emoji2, out Emoji rightEmoji);

            if (!valid)
            {
                await RespondAsync("You must enter two valid emojis!");
                return;
            }

            bool success = _memoryCache.TryGetValue("emoji-kitchen", out List<EmojiCombination>? emojiCombinations);
            if (success)
            {
                string? url = emojiCombinations!
                    .AsQueryable()
                    .Where(combination => combination.LeftEmoji == leftEmoji.GetEmojiCodePoint())
                    .FirstOrDefault(combination => combination.RightEmoji == rightEmoji.GetEmojiCodePoint())?.URL;

                if (string.IsNullOrEmpty(url))
                {
                    url = emojiCombinations!
                        .AsQueryable()
                        .Where(combination => combination.RightEmoji == leftEmoji.GetEmojiCodePoint())
                        .FirstOrDefault(combination => combination.LeftEmoji == rightEmoji.GetEmojiCodePoint())?.URL;
                    if (string.IsNullOrEmpty(url))
                    {
                        await RespondAsync("That combination does not exist.");
                        return;
                    }
                }

                Embed embed = new EmbedBuilder()
                    .WithTitle($"Combination of {leftEmoji} and {rightEmoji}")
                    .WithThumbnailUrl(url)
                    .WithDescription("Changing the order of the emojis\nmay yield different results!")
                    .WithCurrentTimestamp()
                    .WithColor(Color.Teal)
                    .Build();
                await RespondAsync(embed: embed);
            }
            else
            {
                await RespondAsync("There was an error with the Emoji Kitchen service.");
            }
        }
        catch (Exception ex)
        {
            Console.Write(ex.Message);
        }
    }
}