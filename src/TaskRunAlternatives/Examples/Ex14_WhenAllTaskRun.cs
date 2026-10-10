namespace TaskRunAlternatives.Examples;

/// <summary>#14 — ten sam delegat N razy równolegle: pętla + Task.Run + Task.WhenAll.</summary>
public sealed class Ex14_WhenAllTaskRun : IExample
{
    public int Id => 14;
    public string Title => "Task.WhenAll + N x Task.Run";
    public string Code => "await Task.WhenAll(Enumerable.Range(0, 10).Select(_ => Task.Run(Foo)));";

    public async Task<string> RunAsync(CancellationToken ct)
    {
        var results = await Task.WhenAll(
            Enumerable.Range(0, 5).Select(i => Task.Run(() => Work.CpuBound(i), ct)));

        return $"5x Task.Run równolegle → [{string.Join(", ", results)}]";
    }
}
