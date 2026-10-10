namespace TaskRunAlternatives.Examples;

/// <summary>#1 — Task.Run: domyślna droga na ThreadPool (ExecutionContext flowuje).</summary>
public sealed class Ex01_TaskRun : IExample
{
    public int Id => 1;
    public string Title => "Task.Run";
    public string Code => "await Task.Run(() => Work.CpuBound(1), ct);";

    public async Task<string> RunAsync(CancellationToken ct)
    {
        var r = await Task.Run(
            () => (Value: Work.CpuBound(1), Thread: Environment.CurrentManagedThreadId,
                   IsPool: Thread.CurrentThread.IsThreadPoolThread), ct);

        return Work.Describe("CpuBound(1)", r.Value, r.Thread, r.IsPool);
    }
}
