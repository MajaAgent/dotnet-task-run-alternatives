namespace TaskRunAlternatives.Examples;

/// <summary>
/// Jeden sposób "odpalenia tego samego co Task.Run" — jeden przykład = jeden plik.
/// </summary>
public interface IExample
{
    /// <summary>Numer przykładu (kolejność w runnerze).</summary>
    int Id { get; }

    /// <summary>Nazwa API, którego dotyczy przykład.</summary>
    string Title { get; }

    /// <summary>Jednolinijkowy wariant, pokazywany w outputcie.</summary>
    string Code { get; }

    /// <summary>Uruchamia przykład i zwraca wynik do wydrukowania.</summary>
    Task<string> RunAsync(CancellationToken ct);
}
