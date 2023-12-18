using Catness.Handlers;
using Catness.Persistence.Models;
using Catness.Services.EntityFramework;

namespace Catness.Services.Timed;

public class BirthdayService : ITimedService
{
    public CancellationTokenSource TokenSource { get; private set; }

    private readonly BotPersistenceService _persistenceService;
    private readonly UserService _userService;
    private readonly BirthdayHandler _birthdayHandler;

    public BirthdayService(
        BotPersistenceService persistenceService,
        UserService userService,
        BirthdayHandler birthdayHandler)
    {
        TokenSource = new CancellationTokenSource();
        _persistenceService = persistenceService;
        _userService = userService;
        _birthdayHandler = birthdayHandler;
    }

    public async Task Start()
    {
        TokenSource = new CancellationTokenSource();
        BotPersistence persistence = await _persistenceService.GetOrAddKeyValuePair("birthday");

        using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromDays(1));
        try
        {
            do
            {
                DateTime.UtcNow.Deconstruct(out DateOnly today, out _);

                DateOnly databaseDate = DateOnly.Parse(persistence.Value ?? "01-01-0001");
                DateTime lastDatabaseBirthday = new DateTime(databaseDate, new TimeOnly(0));

                if (persistence.Value is not null && DateTime.UtcNow - lastDatabaseBirthday < TimeSpan.FromDays(1))
                {
                    continue;
                }
                
                persistence.Value = today.ToString("O");
                await _persistenceService.UpdateKeyValuePair(persistence);

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
                }).ConfigureAwait(false);
            } while (await timer.WaitForNextTickAsync(TokenSource.Token));
        }
        catch (OperationCanceledException) { }
    }

    public async Task Stop()
    {
        await TokenSource.CancelAsync();
    }
}