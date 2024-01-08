using Catness.Clients;
using Catness.Handlers;
using Catness.IO;
using Catness.Models.EmojiKitchen;
using Catness.Persistence.Models;
using Catness.Services.EntityFramework;
using Microsoft.Extensions.Caching.Memory;

namespace Catness.Services.Timed;

public class TimedPersistenceService : ITimedService
{
    public CancellationTokenSource TokenSource { get; private set; }

    private readonly EmojiKitchenClient _emojiKitchenClient;
    private readonly IMemoryCache _memoryCache;
    private readonly UserService _userService;
    private readonly BirthdayHandler _birthdayHandler;
    private readonly BotPersistenceService _persistenceService;

    public TimedPersistenceService(
        EmojiKitchenClient emojiKitchenClient,
        BotPersistenceService persistenceService,
        BirthdayHandler birthdayHandler,
        UserService userService,
        IMemoryCache memoryCache)
    {
        TokenSource = new CancellationTokenSource();
        _emojiKitchenClient = emojiKitchenClient;
        _persistenceService = persistenceService;
        _birthdayHandler = birthdayHandler;
        _userService = userService;
        _memoryCache = memoryCache;
    }

    public async Task Start()
    {
        TokenSource = new CancellationTokenSource();
        BotPersistence persistence = await _persistenceService.GetOrAddKeyValuePair("last_started");

        using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromDays(1));
        try
        {
            do
            {
                DateTime.UtcNow.Deconstruct(out DateOnly today, out _);

                DateOnly databaseDate = DateOnly.Parse(persistence.Value ?? "01-01-0001");
                DateTime lastStart = new DateTime(databaseDate, new TimeOnly(0));

                if (persistence.Value is not null && DateTime.UtcNow - lastStart < TimeSpan.FromDays(1))
                {
                    await GetEmojiKitchen();
                    continue;
                }

                persistence.Value = today.ToString("O");
                await _persistenceService.UpdateKeyValuePair(persistence);

                await SetEmojiKitchen();
                await SendBirthdays();
            } while (await timer.WaitForNextTickAsync(TokenSource.Token));
        }
        catch (OperationCanceledException) { }
    }

    private async Task SendBirthdays()
    {
        Console.WriteLine("Sending birthdays...");
        List<User> birthdaysToday = await _userService.GetUsersWithFollowersWithBirthday();

        ParallelOptions options = new ParallelOptions
        {
            MaxDegreeOfParallelism = 50,
            CancellationToken = TokenSource.Token
        };

        await Parallel.ForEachAsync(birthdaysToday, options, (user, _) =>
        {
            Task.Run(async () =>
            {
                foreach (Follow follow in user.Followers)
                {
                    await _birthdayHandler.SendBirthday(follow.FollowerId, user.UserId);
                }
            }, TokenSource.Token);
            return ValueTask.CompletedTask;
        });
    }

    private async Task GetEmojiKitchen()
    {
        Console.WriteLine("Reading emoji kitchen combination cache...");
        var combinations = EmojiKitchenIO.ReadEmojis();
        if (combinations is not null)
        {
            _memoryCache.Set("emoji-kitchen", combinations);
            Console.WriteLine("Emoji kitchen cache set...");
            return;
        }

        Console.WriteLine("Failed to read cache...");
        await SetEmojiKitchen();
    }

    private async Task SetEmojiKitchen()
    {
        Console.WriteLine("Starting emoji kitchen combination caching...");
        var combinations = await _emojiKitchenClient.GetEmojiCombinationSet();
        if (combinations is not null)
        {
            if (_memoryCache.TryGetValue("emoji-kitchen", out HashSet<EmojiCombination>? oldCombinations) && oldCombinations is not null)
            {
                if (combinations.SetEquals(oldCombinations))
                {
                    Console.WriteLine("Cache is up to date...");
                }
            } 
            
            _memoryCache.Set("emoji-kitchen", combinations);

            var fileCache = EmojiKitchenIO.ReadEmojis();

            if (fileCache is not null)
            {
                if (combinations.SetEquals(fileCache))
                {
                    Console.WriteLine("File cache is up to date...");
                    return;
                }
            }

            EmojiKitchenIO.WriteEmojis(combinations);
        }
        Console.WriteLine("Ended emoji kitchen combination caching...");
    }

    public async Task Stop()
    {
        await TokenSource.CancelAsync();
    }
}