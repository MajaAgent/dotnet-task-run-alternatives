using Microsoft.Extensions.Hosting;

namespace TaskRunAlternatives.Examples;

/// <summary>Usługa w tle używana przez przykład #19.</summary>
public sealed class DemoBackgroundService(TaskCompletionSource<string> summary) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var processed = 0;

        while (!stoppingToken.IsCancellationRequested && processed < 3)
        {
            await Work.IoAsync(processed, stoppingToken);
            processed++;
        }

        summary.TrySetResult(
            $"host wystartował, ExecuteAsync przetworzył {processed} elementy (wątek={Environment.CurrentManagedThreadId}), host zatrzymany");
    }
}
