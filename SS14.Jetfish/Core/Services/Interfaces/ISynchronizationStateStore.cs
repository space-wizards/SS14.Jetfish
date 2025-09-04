namespace SS14.Jetfish.Core.Services.Interfaces;

public interface ISynchronizationStateStore
{
    Guid GetState(Guid key);

    (Guid state, Guid nextState) Advance(Guid key);

    bool Matches(Guid key, Guid state);
}
