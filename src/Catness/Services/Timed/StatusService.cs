using Catness.Persistence.Models;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace Catness.Services.Timed;

public class StatusService : ITimedService
{
    public CancellationTokenSource TokenSource { get; private set; }

    private readonly BotConfiguration _botConfiguration;
    private readonly DiscordSocketClient _client;

    public StatusService(
        IOptions<BotConfiguration> botConfiguration,
        DiscordSocketClient client)
    {
        TokenSource = new CancellationTokenSource();
        _botConfiguration = botConfiguration.Value;
        _client = client;
    }

    public async Task Stop()
    {
        await TokenSource.CancelAsync();
    }

    public async Task Start()
    {
        TokenSource = new CancellationTokenSource();

        using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMinutes(30));
        try
        {
            do
            {
                if (_botConfiguration.DiscordConfiguration.Catchphrases.Length == 0)
                {
                    await TokenSource.CancelAsync();
                }

                string status = _botConfiguration.DiscordConfiguration.Catchphrases
                    .OrderBy(_ => Guid.NewGuid()).ToArray()[0];

                await _client.SetCustomStatusAsync(status);
            } while (await timer.WaitForNextTickAsync(TokenSource.Token));
        }
        catch (OperationCanceledException) { }
    }
}