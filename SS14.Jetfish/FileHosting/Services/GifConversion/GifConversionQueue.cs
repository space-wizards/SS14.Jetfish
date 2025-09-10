using System.Threading.Channels;

namespace SS14.Jetfish.FileHosting.Services.GifConversion;

public class GifConversionQueue
{
    private const int MaxQueueSize = 100;

    private readonly Channel<GifConversionItem> _queue;

    public int Count => _queue.Reader.Count;

    public GifConversionQueue()
    {
        var channelInformation = new BoundedChannelOptions(MaxQueueSize)
        {
            FullMode = BoundedChannelFullMode.Wait,
        };

        _queue = Channel.CreateBounded<GifConversionItem>(channelInformation);
    }

    public async Task<bool> TryQueueItem(GifConversionItem item)
    {
        if (Count >= MaxQueueSize)
            return false;

        await _queue.Writer.WriteAsync(item);
        return true;
    }

    public async ValueTask<GifConversionItem> DequeueAsync(CancellationToken ct = new())
    {
        return await _queue.Reader.ReadAsync(ct);
    }
}
