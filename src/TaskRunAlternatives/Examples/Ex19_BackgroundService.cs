using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TaskRunAlternatives.Examples;

/// <summary>#19 — BackgroundService + Generic Host: standardowy sposób trzymania "Task.Run" w aplikacji.</summary>
public sealed class Ex19_BackgroundService : IExample
{
    public int Id => 19;
    public string Title => "BackgroundService + Generic Host";
    public string Code => "protected override Task ExecuteAsync(CancellationToken ct) => Task.Run(Foo, ct);";

    public async Task<string> RunAsync(CancellationToken ct)
    {
        var summary = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);

        var builder = Host.CreateApplicationBuilder();
        builder.Logging.ClearProviders(); // żeby logi Hosta nie mieszały się z outputem runnera
        builder.Services.AddSingleton(new DemoBackgroundService(summary));
        builder.Services.AddHostedService(sp => sp.GetRequiredService<DemoBackgroundService>());

        using var host = builder.Build();

        await host.StartAsync(ct);
        var report = await summary.Task.WaitAsync(TimeSpan.FromSeconds(5), ct);
        await host.StopAsync(ct);

        return report;
    }
}
