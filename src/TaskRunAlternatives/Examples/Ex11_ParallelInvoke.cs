using System.Collections.Concurrent;

namespace TaskRunAlternatives.Examples;

/// <summary>#11 — Parallel.Invoke: kilka delegatów równolegle, te same workery co ThreadPool.</summary>
public sealed class Ex11_ParallelInvoke : IExample
{
    public int Id => 11;
    public string Title => "Parallel.Invoke";
    public string Code => "Parallel.Invoke(Foo, Foo);";

    public Task<string> RunAsync(CancellationToken ct)
    {
        var bag = new ConcurrentBag<string>();

        Parallel.Invoke(
            () => bag.Add($"{Work.CpuBound(11)}@wątek{Environment.CurrentManagedThreadId}"),
            () => bag.Add($"{Work.CpuBound(11)}@wątek{Environment.CurrentManagedThreadId}"));

        return Task.FromResult($"2x CpuBound(11) → [{string.Join(", ", bag)}]");
    }
}
