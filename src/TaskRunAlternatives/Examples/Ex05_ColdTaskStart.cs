namespace TaskRunAlternatives.Examples;

/// <summary>#5 — "zimny" Task + Start(): ręczna inicjacja, Start() tylko raz.</summary>
public sealed class Ex05_ColdTaskStart : IExample
{
    public int Id => 5;
    public string Title => "new Task(...).Start()";
    public string Code => "var t = new Task(Foo); t.Start(); await t;";

    public async Task<string> RunAsync(CancellationToken ct)
    {
        int value = 0, thread = 0;
        var isPool = false;

        var task = new Task(() =>
        {
            value = Work.CpuBound(5);
            thread = Environment.CurrentManagedThreadId;
            isPool = Thread.CurrentThread.IsThreadPoolThread;
        });

        task.Start();
        await task;

        var restart = "brak próby";
        try
        {
            task.Start();
        }
        catch (InvalidOperationException e)
        {
            restart = $"drugi Start() → InvalidOperationException ({e.Message[..Math.Min(40, e.Message.Length)]}...)";
        }

        return Work.Describe("CpuBound(5)", value, thread, isPool, restart);
    }
}
