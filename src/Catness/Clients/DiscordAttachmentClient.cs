namespace Catness.Clients;

public class DiscordAttachmentClient
{
    private readonly HttpClient _client;

    public DiscordAttachmentClient()
    {
        _client = new HttpClient();
    }

    public async Task<byte[]?> GetByteArray(string? url)
    {
        if (url is null)
        {
            return null;
        }
        using HttpResponseMessage response = await _client.GetAsync(url);
        return await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
    }
}