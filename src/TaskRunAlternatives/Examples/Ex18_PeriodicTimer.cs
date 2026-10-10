using System.Diagnostics;

namespace TaskRunAlternatives.Examples;

/// <summary>#18 — PeriodicTimer: ticki bez callbacku, w normalnej pętli async (bez timer-driftu jak Task.Delay w while).</summary>
public sealed class Ex18_PeriodicTimer : IExample
{
    public int Id => 18;
    public string Title => "PeriodicTimer w pętli async";
    public string Code => "using var pt = new PeriodicTimer(TimeSpan.FromSeconds(1)); while (await pt.WaitForNextTickAsync()) Foo();";

    public async Task<string> RunAsync(CancellationToken ct)
    {
        var ticks = new List<long>();
        var watch = Stopwatch.StartNew();

        using var linked = CancellationTokenSource.CreateLinkedTokenSource(ct);
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(50));

        try
        {
            while (await timer.WaitForNextTickAsync(linked.Token))
            {
                ticks.Add(watch.ElapsedMilliseconds);
                if (ticks.Count == 3) linked.Cancel();
            }
        }
        catch (OperationCanceledException)
        {
            // 3 ticki odebrane — kończymy pętlę
        }

        return $"3 ticki po ~50 ms → czasy [{string.Join(", ", ticks.Select(t => t + " ms"))}]";
    }
}
