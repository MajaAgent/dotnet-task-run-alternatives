namespace TaskRunAlternatives.Examples;

/// <summary>#2 — Task.Factory.StartNew(Action): dokładnie to, co Task.Run robi pod spodem.</summary>
public sealed class Ex02_StartNewAction : IExample
{
    public int Id => 2;
    public string Title => "Task.Factory.StartNew(Action)";
    public string Code => "await Task.Factory.StartNew(() => Foo(), CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);";

    public async Task<string> RunAsync(CancellationToken ct)
    {
        int value = 0, thread = 0;
        var isPool = false;

        await Task.Factory.StartNew(
            () =>
            {
                value = Work.CpuBound(2);
                thread = Environment.CurrentManagedThreadId;
                isPool = Thread.CurrentThread.IsThreadPoolThread;
            },
            CancellationToken.None,
            TaskCreationOptions.DenyChildAttach,
            TaskScheduler.Default);

        return Work.Describe("CpuBound(2)", value, thread, isPool, "DenyChildAttach + TaskScheduler.Default = Task.Run");
    }
}
