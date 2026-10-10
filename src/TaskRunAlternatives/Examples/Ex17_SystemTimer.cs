namespace TaskRunAlternatives.Examples;

/// <summary>#17 — System.Threading.Timer: callback leci na ThreadPool (bez własnego Taska).</summary>
public sealed class Ex17_SystemTimer : IExample
{
    public int Id => 17;
    public string Title => "System.Threading.Timer";
    public string Code => "_ = new System.Threading.Timer(_ => Foo(), null, TimeSpan.Zero, TimeSpan.FromSeconds(1));";

    public async Task<string> RunAsync(CancellationToken ct)
    {
        var ticks = 0;
        var isPool = false;
        var thread = 0;
        var third = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);

        using var timer = new System.Threading.Timer(_ =>
        {
            thread = Environment.CurrentManagedThreadId;
            isPool = Thread.CurrentThread.IsThreadPoolThread;
            if (Interlocked.Increment(ref ticks) == 3) third.TrySetResult(ticks);
        }, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(60));

        var count = await third.Task.WaitAsync(TimeSpan.FromSeconds(5), ct);

        return $"callback odpalił się {count}x (co 60 ms), wątek={thread}, pool={isPool}";
    }
}
