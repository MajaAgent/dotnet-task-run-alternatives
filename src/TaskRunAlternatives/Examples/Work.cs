namespace TaskRunAlternatives.Examples;

/// <summary>Wspólna "praca", którą odpala każdy przykład — dzięki temu wyniki są porównywalne.</summary>
public static class Work
{
    /// <summary>Praca CPU-bound: zajmuje wątek na czas obliczeń.</summary>
    public static int CpuBound(int seed, int spins = 150_000)
    {
        var acc = seed;
        for (var i = 0; i < spins; i++) acc = unchecked(acc * 31 + i);
        return seed + 1;
    }

    /// <summary>Praca I/O-bound: zwalnia wątek na czas awaita.</summary>
    public static async Task<int> IoAsync(int seed, CancellationToken ct = default)
    {
        await Task.Delay(25, ct);
        return seed + 1;
    }

    public static string Describe(string label, int value, int threadId, bool isPool, string? extra = null)
        => $"{label}={value}, wątek={threadId}, pool={isPool}{(extra is null ? "" : ", " + extra)}";
}
