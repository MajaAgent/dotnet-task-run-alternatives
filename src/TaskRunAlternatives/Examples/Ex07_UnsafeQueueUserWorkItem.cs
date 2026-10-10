namespace TaskRunAlternatives.Examples;

/// <summary>#7 — UnsafeQueueUserWorkItem: szybciej, ale NIE flowuje ExecutionContext (AsyncLocal zostaje pusty).</summary>
public sealed class Ex07_UnsafeQueueUserWorkItem : IExample
{
    private static readonly AsyncLocal<string?> Ambient = new();

    public int Id => 7;
    public string Title => "ThreadPool.UnsafeQueueUserWorkItem";
    public string Code => "ThreadPool.UnsafeQueueUserWorkItem(_ => Foo(), null);";

    public async Task<string> RunAsync(CancellationToken ct)
    {
        Ambient.Value = "kontekst-flowuje";

        var tcs = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        int thread = 0;
        var isPool = false;
        string? ambientInside = null;

        ThreadPool.UnsafeQueueUserWorkItem(_ =>
        {
            thread = Environment.CurrentManagedThreadId;
            isPool = Thread.CurrentThread.IsThreadPoolThread;
            ambientInside = Ambient.Value ?? "(null)";
            tcs.SetResult(Work.CpuBound(7));
        }, null);

        var value = await tcs.Task;
        Ambient.Value = null;

        return Work.Describe("CpuBound(7)", value, thread, isPool, $"AsyncLocal w środku={ambientInside} → kontekst nie przepłynął");
    }
}
