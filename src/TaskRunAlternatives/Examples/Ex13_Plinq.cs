namespace TaskRunAlternatives.Examples;

/// <summary>#13 — PLINQ: równoległość deklaratywnie w LINQ (bez Task/await).</summary>
public sealed class Ex13_Plinq : IExample
{
    public int Id => 13;
    public string Title => "PLINQ AsParallel()";
    public string Code => "var r = Enumerable.Range(0, 10).AsParallel().WithDegreeOfParallelism(4).Select(i => Foo()).ToArray();";

    public Task<string> RunAsync(CancellationToken ct)
    {
        var results = Enumerable.Range(0, 6)
            .AsParallel()
            .AsOrdered()
            .WithDegreeOfParallelism(4)
            .Select(i => Work.CpuBound(i))
            .ToArray();

        return Task.FromResult($"6 elementów, degree=4 → [{string.Join(", ", results)}] (kolejność zachowana przez AsOrdered)");
    }
}
