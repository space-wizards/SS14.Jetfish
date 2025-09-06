using SS14.Jetfish.Core.Events;
using SS14.Jetfish.Core.Types;

namespace SS14.Jetfish.Core.Services.Interfaces;

public interface IConcurrentEventBus
{
    Guid GetState(Guid key);
    IDisposable Subscribe<TEvent>(Guid key, Func<TEvent, CancellationToken, ValueTask> handler);

    Task PublishAsync<TEvent>(Guid key, TEvent @event, CancellationToken ct = default)  where TEvent : ConcurrentEvent;

    Task<Result<TResult, Exception>> CallSynced<TResult>(Guid key, Guid state, Func<Task<TResult>> action) where TResult : class;

    Task<Result<TResult, Exception>> CallSynced<TResult>(Guid key, Guid state, Func<Task<Result<TResult, Exception>>> action) where TResult : class;
}
