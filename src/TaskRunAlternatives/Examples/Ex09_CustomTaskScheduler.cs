using System.Collections.Concurrent;
using TaskRunAlternatives.Scheduling;

namespace TaskRunAlternatives.Examples;

/// <summary>#9 — własny TaskScheduler: sam decydujesz, gdzie i z jakim limitem lecą zadania.</summary>
public sealed class Ex09_CustomTaskScheduler : IExample
{
    public int Id => 9;
    public string Title => "własny TaskScheduler (limit 3)";
    public string Code => "await Task.Factory.StartNew(() => Foo(), CancellationToken.None, TaskCreationOptions.None, myScheduler);";

    public async Task<string> RunAsync(CancellationToken ct)
    {
        var scheduler = new LimitedConcurrencyLevelTaskScheduler(3);
        var running = 0;
        var maxObserved = 0;
        var gate = new object();

        // Praca jest blokująca (Thread.Sleep), więc realnie zajmuje pracownika schedulera.
        // Przy async ciele zadanie zwalnia pracownika na awaicie i scheduler odpala kolejne
        // mimo limitu — limit dotyczy wątków wykonujących zadania, nie wiszących awaitów.
        var tasks = Enumerable.Range(0, 6).Select(_ => Task.Factory.StartNew(
            () =>
            {
                var now = Interlocked.Increment(ref running);
                lock (gate) maxObserved = Math.Max(maxObserved, now);
                Thread.Sleep(50);
                Interlocked.Decrement(ref running);
                return Work.CpuBound(9);
            },
            CancellationToken.None,
            TaskCreationOptions.None,
            scheduler));

        var results = await Task.WhenAll(tasks);

        return $"6 zadań na {scheduler.GetType().Name}(3) → max równolegle={maxObserved}, " +
               $"MaximumConcurrencyLevel={scheduler.MaximumConcurrencyLevel}, wyniki [{string.Join(", ", results)}]";
    }
}
