namespace TaskRunAlternatives.Examples;

/// <summary>#8 — new Thread: pełna kontrola (IsBackground, Name, apartament), nie zajmuje poolu.</summary>
public sealed class Ex08_NewThread : IExample
{
    public int Id => 8;
    public string Title => "new Thread(...).Start()";
    public string Code => "new Thread(Foo) { IsBackground = true }.Start();";

    public async Task<string> RunAsync(CancellationToken ct)
    {
        var done = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        int value = 0, thread = 0;
        var isPool = true;

        var worker = new Thread(() =>
        {
            value = Work.CpuBound(8);
            thread = Environment.CurrentManagedThreadId;
            isPool = Thread.CurrentThread.IsThreadPoolThread;
            done.SetResult();
        })
        {
            IsBackground = true,
            Name = "ex08-worker",
        };

        worker.Start();
        await done.Task;

        return Work.Describe("CpuBound(8)", value, thread, isPool, $"wątek nazwany={worker.Name}, join={worker.Join(1000)}");
    }
}
