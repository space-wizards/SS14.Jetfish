using MessagePipe;
using SS14.Jetfish.Core.Events;
using SS14.Jetfish.Core.Services.Interfaces;
using SS14.Jetfish.Core.Types;

namespace SS14.Jetfish.Core.Services;

public class ConcurrentEventBus : IConcurrentEventBus
{
    private readonly ISynchronizationStateStore _stateStore;
    private readonly ILogger<ConcurrentEventBus> _logger;
    private readonly IServiceProvider _serviceProvider;

    public ConcurrentEventBus(ISynchronizationStateStore stateStore, ILogger<ConcurrentEventBus> logger, IServiceProvider serviceProvider)
    {
        _stateStore = stateStore;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public Guid GetState(Guid key) => _stateStore.GetState(key);

    public IDisposable Subscribe<TEvent>(Guid key, Func<TEvent, CancellationToken, ValueTask> handler)
    {
        var subscriber = _serviceProvider.GetRequiredService<IAsyncSubscriber<Guid, TEvent>>();
        return subscriber.Subscribe(key, handler);
    }

    public async Task PublishAsync<TEvent>(Guid key, TEvent @event, CancellationToken ct = default) where TEvent : ConcurrentEvent
    {
        var (state, nextState) = _stateStore.Advance(key);
        _logger.LogDebug("Publishing event {EventName} with state {State}", typeof(TEvent).Name, state);
        @event.StateId = state;
        @event.NextStateId = nextState;
        var publisher = _serviceProvider.GetRequiredService<IAsyncPublisher<Guid, TEvent>>();
        await publisher.PublishAsync(key, @event, ct);
    }

    public async Task<Result<TResult, Exception>> CallSynced<TResult>(Guid key, Guid state, Func<Task<TResult>> action) where TResult : class
    {
        if (!_stateStore.Matches(key, state))
            return Result<TResult, Exception>.Failure(new SynchronizationException(key, state, _stateStore.GetState(key)));

        try
        {
            return Result<TResult, Exception>.Success(await action());
        }
        catch (Exception e)
        {
            return Result<TResult, Exception>.Failure(e);
        }
    }

    public async Task<Result<TResult, Exception>> CallSynced<TResult>(Guid key, Guid state, Func<Task<Result<TResult, Exception>>> action) where TResult : class
    {
        if (!_stateStore.Matches(key, state))
            return Result<TResult, Exception>.Failure(new SynchronizationException(key, state, _stateStore.GetState(key)));

        return await action();
    }
}
