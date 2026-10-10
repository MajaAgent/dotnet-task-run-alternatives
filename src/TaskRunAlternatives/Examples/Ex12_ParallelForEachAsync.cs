using System.Collections.Concurrent;

namespace TaskRunAlternatives.Examples;

/// <summary>#12 — Parallel.ForEachAsync: async pętla z limitem równoległości (nowe API z .NET 6).</summary>
public sealed class Ex12_ParallelForEachAsync : IExample
{
    public int Id => 12;
    public string Title => "Parallel.ForEachAsync(degree=4)";
    public string Code => "await Parallel.ForEachAsync(Enumerable.Range(0, 10), new ParallelOptions { MaxDegreeOfParallelism = 4 }, async (i, ct) => await FooAsync(ct));";

    public async Task<string> RunAsync(CancellationToken ct)
    {
        var results = new ConcurrentBag<int>();

        await Parallel.ForEachAsync(
            Enumerable.Range(0, 6),
            new ParallelOptions { MaxDegreeOfParallelism = 4, CancellationToken = ct },
            async (i, token) => results.Add(await Work.IoAsync(i, token)));

        return $"6 iteracji, MaxDegreeOfParallelism=4 → wyniki [{string.Join(", ", results.Order())}]";
    }
}
