namespace TaskRunAlternatives.Examples;

/// <summary>#10 — ConcurrentExclusiveSchedulerPair: ConcurrentScheduler = pula z limitem, ExclusiveScheduler = mutex dla "writera".</summary>
public sealed class Ex10_ConcurrentExclusivePair : IExample
{
    public int Id => 10;
    public string Title => "ConcurrentExclusiveSchedulerPair(4)";
    public string Code => "var s = new ConcurrentExclusiveSchedulerPair(TaskScheduler.Default, 4).ConcurrentScheduler; await Task.Factory.StartNew(() => Foo(), ct, TaskCreationOptions.None, s);";

    public async Task<string> RunAsync(CancellationToken ct)
    {
        var pair = new ConcurrentExclusiveSchedulerPair(TaskScheduler.Default, maxConcurrencyLevel: 4);

        var running = 0;
        var maxObserved = 0;
        var gate = new object();

        // Blokująca praca, żeby limit 4 był widoczny (patrz komentarz w przykładzie #9).
        var concurrent = Enumerable.Range(0, 8).Select(_ => Task.Factory.StartNew(
            () =>
            {
                var now = Interlocked.Increment(ref running);
                lock (gate) maxObserved = Math.Max(maxObserved, now);
                Thread.Sleep(40);
                Interlocked.Decrement(ref running);
                return Work.CpuBound(10);
            },
            CancellationToken.None,
            TaskCreationOptions.None,
            pair.ConcurrentScheduler));

        var concurrentResults = await Task.WhenAll(concurrent);

        var exclusiveRan = await Task.Factory.StartNew(
            () => Work.CpuBound(10),
            CancellationToken.None,
            TaskCreationOptions.None,
            pair.ExclusiveScheduler);

        return $"8 zadań na ConcurrentScheduler(4) → max równolegle={maxObserved} (limit trzyma), " +
               $"wyniki [{string.Join(", ", concurrentResults)}]; " +
               $"ExclusiveScheduler oddał CpuBound(10)={exclusiveRan} (wchodzi dopiero gdy concurrent jest puste)";
    }
}
