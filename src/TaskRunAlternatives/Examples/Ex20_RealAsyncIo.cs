namespace TaskRunAlternatives.Examples;

/// <summary>#20 — prawdziwe I/O: tu Task.Run jest zbędny, async/await nie zajmuje żadnego wątku na czas operacji.</summary>
public sealed class Ex20_RealAsyncIo : IExample
{
    public int Id => 20;
    public string Title => "async I/O bez Task.Run";
    public string Code => "var text = await File.ReadAllTextAsync(path, ct);  // albo httpClient.GetStringAsync(url)";

    public async Task<string> RunAsync(CancellationToken ct)
    {
        var path = Path.Combine(Path.GetTempPath(), $"ex20-{Guid.NewGuid():N}.txt");
        var payload = "async I/O bez zajmowania wątku";

        await File.WriteAllTextAsync(path, payload, ct);
        var text = await File.ReadAllTextAsync(path, ct);
        File.Delete(path);

        return $"File.ReadAllTextAsync → \"{text}\" ({text.Length} znaków); " +
               $"żaden wątek nie był zablokowany, wątek={Environment.CurrentManagedThreadId}, pool={Thread.CurrentThread.IsThreadPoolThread}";
    }
}
