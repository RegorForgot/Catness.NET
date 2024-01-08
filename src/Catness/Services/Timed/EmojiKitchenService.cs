using Catness.Clients;
using Microsoft.Extensions.Caching.Memory;

namespace Catness.Services.Timed;

public class EmojiKitchenService : ITimedService
{
    public CancellationTokenSource TokenSource { get; private set; }
    
    private readonly EmojiKitchenClient _emojiKitchenClient;
    private readonly IMemoryCache _memoryCache;

    public EmojiKitchenService(
        EmojiKitchenClient emojiKitchenClient,
        IMemoryCache memoryCache)
    {
        TokenSource = new CancellationTokenSource();
        _emojiKitchenClient = emojiKitchenClient;
        _memoryCache = memoryCache;
    }

    public async Task Start()
    {
        TokenSource = new CancellationTokenSource();

        using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromDays(1));
        try
        {
            do
            {
                Console.WriteLine("Starting emoji kitchen combination caching...");
                var combinations = await _emojiKitchenClient.GetEmojiKitchenDictionary();
                if (combinations is not null)
                {
                    _memoryCache.Set("emoji-kitchen", combinations);
                }
                Console.WriteLine("Ended emoji kitchen combination caching...");
            } while (await timer.WaitForNextTickAsync(TokenSource.Token));
        }
        catch (OperationCanceledException) { }
    }
    
    public async Task Stop()
    {
        await TokenSource.CancelAsync();
    }
}