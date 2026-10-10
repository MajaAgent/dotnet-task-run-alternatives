namespace TaskRunAlternatives.Examples;

/// <summary>#3 — StartNew(async lambda).Unwrap(): bez Unwrap() dostałbyś Task&lt;Task&lt;int&gt;&gt;.</summary>
public sealed class Ex03_StartNewAsyncUnwrap : IExample
{
    public int Id => 3;
    public string Title => "StartNew(async) + Unwrap";
    public string Code => "await Task.Factory.StartNew(async () => await FooAsync(), ct, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default).Unwrap();";

    public async Task<string> RunAsync(CancellationToken ct)
    {
        var r = await Task.Factory.StartNew(
                async () => (Value: await Work.IoAsync(3, ct), Thread: Environment.CurrentManagedThreadId),
                CancellationToken.None,
                TaskCreationOptions.DenyChildAttach,
                TaskScheduler.Default)
            .Unwrap();

        return $"IoAsync(3)={r.Value}, wątek={r.Thread}, bez Unwrap() byłby Task<Task<int>>";
    }
}
