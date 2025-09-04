using System.Collections.Concurrent;
using SS14.Jetfish.Core.Services.Interfaces;

namespace SS14.Jetfish.Core.Services;

public class SynchronizationStateStore : ISynchronizationStateStore
{
    private readonly ConcurrentDictionary<Guid, Guid> _states = new();

    public Guid GetState(Guid key) => _states.GetValueOrDefault(key, Guid.Empty);

    public (Guid state, Guid nextState) Advance(Guid key)
    {
        var next = Guid.CreateVersion7();
        var state = _states.GetValueOrDefault(key, Guid.Empty);
        _states.AddOrUpdate(key, next, (_, _) => next);
        return (state, next);
    }

    public bool Matches(Guid key, Guid state) =>
        _states.TryGetValue(key, out var stored) ? stored == state : state == Guid.Empty;

}
