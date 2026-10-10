namespace TaskRunAlternatives.Examples;

/// <summary>#4 — LongRunning: dedykowany wątek zamiast wątku z poolu (dla blokującego kodu).</summary>
public sealed class Ex04_StartNewLongRunning : IExample
{
    public int Id => 4;
    public string Title => "StartNew(LongRunning)";
    public string Code => "await Task.Factory.StartNew(() => Foo(), ct, TaskCreationOptions.LongRunning, TaskScheduler.Default);";

    public async Task<string> RunAsync(CancellationToken ct)
    {
        var r = await Task.Factory.StartNew(
            () => (Value: Work.CpuBound(4), Thread: Environment.CurrentManagedThreadId,
                   IsPool: Thread.CurrentThread.IsThreadPoolThread),
            CancellationToken.None,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);

        return Work.Describe("CpuBound(4)", r.Value, r.Thread, r.IsPool, "własny wątek, nie ThreadPool");
    }
}
