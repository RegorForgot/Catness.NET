namespace Catness.Services.Timed;

public interface ITimedService
{
    public CancellationTokenSource TokenSource { get; }
    public Task Start();
    public Task Stop();
}