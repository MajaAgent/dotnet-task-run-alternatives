using System.Threading.Channels;

namespace TaskRunAlternatives.Examples;

/// <summary>#16 — Channel&lt;T&gt;: kolejka producent→konsument, konsument chodzi w tle.</summary>
public sealed class Ex16_Channel : IExample
{
    public int Id => 16;
    public string Title => "Channel<T> + konsument w tle";
    public string Code => "await channel.Writer.WriteAsync(item);";

    public async Task<string> RunAsync(CancellationToken ct)
    {
        var channel = Channel.CreateBounded<int>(new BoundedChannelOptions(4)
        {
            FullMode = BoundedChannelFullMode.Wait,
        });

        var consumed = new List<int>();
        var consumer = Task.Run(async () =>
        {
            await foreach (var item in channel.Reader.ReadAllAsync(ct))
                consumed.Add(item);
        }, ct);

        foreach (var i in Enumerable.Range(0, 5))
            await channel.Writer.WriteAsync(i, ct);

        channel.Writer.Complete();
        await consumer;

        return $"Bounded(4): producent zapisał 5, konsument odebrał [{string.Join(", ", consumed)}]";
    }
}
