namespace TaskRunAlternatives.Examples;

/// <summary>#15 — throttling: Task.Run bez limitu to przepis na zagłodzenie poolu, SemaphoreSlim go pilnuje.</summary>
public sealed class Ex15_SemaphoreThrottle : IExample
{
    public int Id => 15;
    public string Title => "Task.Run + SemaphoreSlim(2)";
    public string Code => "var sem = new SemaphoreSlim(2); await Task.WhenAll(items.Select(async i => { await sem.WaitAsync(); try { await Task.Run(Foo); } finally { sem.Release(); } }));";

    public async Task<string> RunAsync(CancellationToken ct)
    {
        using var semaphore = new SemaphoreSlim(2);
        var running = 0;
        var maxObserved = 0;
        var gate = new object();

        var results = await Task.WhenAll(Enumerable.Range(0, 6).Select(async i =>
        {
            await semaphore.WaitAsync(ct);
            try
            {
                var now = Interlocked.Increment(ref running);
                lock (gate) maxObserved = Math.Max(maxObserved, now);
                var value = await Task.Run(() => Work.CpuBound(i), ct);
                Interlocked.Decrement(ref running);
                return value;
            }
            finally
            {
                semaphore.Release();
            }
        }));

        return $"6x Task.Run z limitem 2 → max równolegle={maxObserved}, wyniki [{string.Join(", ", results)}]";
    }
}
