using Catness.Clients;
using Catness.Enums;
using Discord;
using Discord.Interactions;

namespace Catness.Modules.Fun;

public class MakesweetModule : InteractionModuleBase
{
    private readonly MakesweetClient _makesweetClient;
    private readonly DiscordAttachmentClient _attachmentClient;

    public MakesweetModule(MakesweetClient makesweetClient,
        DiscordAttachmentClient attachmentClient)
    {
        _makesweetClient = makesweetClient;
        _attachmentClient = attachmentClient;
    }

    [SlashCommand("makesweet", "Make a makesweet gif!")]
    public async Task MakesweetGif(
        MakesweetTemplate template,
        Attachment? image = null,
        string? text = null,
        [MinValue(0)] int textBorder = 0,
        bool textFirst = false)
    {
        await DeferAsync();

        byte[]? makesweetBytes;

        if (image is not null)
        {
            byte[]? attachmentBytes = await _attachmentClient.GetByteArray(image.Url);
            if (attachmentBytes is null)
            {
                await FollowupAsync("Error uploading image!");
                return;
            }

            try
            {
                makesweetBytes = await _makesweetClient.GetMakesweetGif(
                    template,
                    attachmentBytes,
                    text,
                    textBorder,
                    textFirst);
            }
            catch (HttpRequestException ex)
            {
                await FollowupAsync($"Error contacting Makesweet!\n" +
                                    $"{(int) ex.StatusCode!}: {ex.StatusCode} - {ex.Message}");
                return;
            }
        }
        else if (text is not null)
        {
            makesweetBytes = await _makesweetClient.GetMakesweetGif(
                template,
                null,
                text,
                textBorder,
                textFirst);
        }
        else
        {
            await FollowupAsync("Please enter at least a message or image");
            return;
        }

        if (makesweetBytes is null)
        {
            await FollowupAsync("Failure in returning gif!");
        }
        else
        {
            await FollowupWithFileAsync(
                new MemoryStream(makesweetBytes),
                "makesweetGif.gif",
                text: "Here is your makesweet gif!");
        }
    }
}