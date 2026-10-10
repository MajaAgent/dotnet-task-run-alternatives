namespace TaskRunAlternatives.Examples;

/// <summary>#6 — ThreadPool.QueueUserWorkItem: bez Task, ale ExecutionContext jest flowowany.</summary>
public sealed class Ex06_QueueUserWorkItem : IExample
{
    private static readonly AsyncLocal<string?> Ambient = new();

    public int Id => 6;
    public string Title => "ThreadPool.QueueUserWorkItem";
    public string Code => "ThreadPool.QueueUserWorkItem(_ => Foo());";

    public async Task<string> RunAsync(CancellationToken ct)
    {
        Ambient.Value = "kontekst-flowuje";

        var tcs = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        int thread = 0;
        var isPool = false;
        string? ambientInside = null;

        ThreadPool.QueueUserWorkItem(_ =>
        {
            thread = Environment.CurrentManagedThreadId;
            isPool = Thread.CurrentThread.IsThreadPoolThread;
            ambientInside = Ambient.Value;
            tcs.SetResult(Work.CpuBound(6));
        });

        var value = await tcs.Task;
        Ambient.Value = null;

        return Work.Describe("CpuBound(6)", value, thread, isPool, $"AsyncLocal w środku=\"{ambientInside}\"");
    }
}
