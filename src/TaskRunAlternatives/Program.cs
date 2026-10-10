using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using TaskRunAlternatives.Examples;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// Zbieramy wszystkie przykłady z assembly i sortujemy po numerze — dorzucenie pliku
// Ex21_Cos.cs automatycznie dopina nowy przykład do runnera.
var all = Assembly.GetExecutingAssembly()
    .GetTypes()
    .Where(t => t is { IsAbstract: false, IsInterface: false } && typeof(IExample).IsAssignableFrom(t))
    .Select(t => (IExample)Activator.CreateInstance(t)!)
    .OrderBy(e => e.Id)
    .ToArray();

if (args.Contains("--list"))
{
    foreach (var e in all)
        Console.WriteLine($"[{e.Id:D2}] {e.Title,-38} {e.Code}");

    Console.WriteLine();
    Console.WriteLine("Użycie: dotnet run -- [numery...] [--no-code] [--list]");
    return 0;
}

var wanted = args.Where(a => int.TryParse(a, out _)).Select(int.Parse).ToHashSet();
var showCode = !args.Contains("--no-code");
var examples = wanted.Count > 0 ? all.Where(e => wanted.Contains(e.Id)).ToArray() : all;

Console.WriteLine($"=== {examples.Length} sposobów na to samo co Task.Run ({RuntimeInformation.FrameworkDescription}) ===");
Console.WriteLine();

var failed = 0;

foreach (var example in examples)
{
    Console.WriteLine($"[{example.Id:D2}] {example.Title}");
    if (showCode) Console.WriteLine($"     {example.Code}");

    var watch = Stopwatch.StartNew();
    try
    {
        var result = await example.RunAsync(CancellationToken.None);
        watch.Stop();
        Console.WriteLine($"     OK  ({watch.ElapsedMilliseconds,4} ms)  {result}");
    }
    catch (Exception ex)
    {
        watch.Stop();
        failed++;
        Console.WriteLine($"     FAIL({watch.ElapsedMilliseconds,4} ms)  {ex.GetType().Name}: {ex.Message}");
    }

    Console.WriteLine();
}

Console.WriteLine($"=== {examples.Length - failed}/{examples.Length} OK, {failed} FAIL ===");
return failed == 0 ? 0 : 1;
