using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore.Query.Internal;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Projects.Model;

namespace SS14.Jetfish.Projects.Hubs;

public class ProjectHub
{
    private readonly ConcurrentDictionary<Type, List<Func<object, object, Task>>> _handlers = new();
    private readonly ConcurrentDictionary<object, Guid> _states = new();

    private readonly ILogger<ProjectHub> _logger;

    public ProjectHub(ILogger<ProjectHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Attempts to call the method passed in. This method checks for the state and fails if you are behind.
    /// </summary>
    /// <param name="sender">The object key you are trying to update.</param>
    /// <param name="state">The state that *you* are aware of.</param>
    /// <param name="method">The method to call.</param>
    /// <typeparam name="T">What you expect the method to return.</typeparam>
    /// <returns>A result of type T or an exception raised by the method.</returns>
    public async Task<Result<T, Exception>> AttemptCallSynced<T>(object sender, Guid state, Func<Task<T>> method) where T : class
    {
        if (_states.TryGetValue(sender, out var hubState))
        {
            if (hubState != state)
                return Result<T, Exception>.Failure(new SynchronizationException(sender, state, hubState));
        }

        try
        {
            return Result<T, Exception>.Success(await method());
        }
        catch (Exception e)
        {
            return Result<T, Exception>.Failure(e);
        }
    }

    public void RegisterHandler<TEvent>(Func<object, TEvent, Task> handler) where TEvent : ProjectEvent
    {
        var eventType = typeof(TEvent);
        _logger.LogDebug("Registering handler for {EventTypeName}", eventType.Name);

        Func<object, object, Task> wrappedHandler = async (sender, e) =>
        {
            if (e is TEvent typedEvent)
            {
                await handler(sender, typedEvent);
            }
        };

        _handlers.AddOrUpdate(eventType,
            _ => [wrappedHandler],
            (_, existing) =>
            {
                lock (existing)
                {
                    existing.Add(wrappedHandler);
                }
                return existing;
            });
    }

    public void UnregisterHandler<TEvent>(Func<object, TEvent, Task> handler) where TEvent : ProjectEvent
    {
        var eventType = typeof(TEvent);
        _logger.LogDebug("Unregistering handler for {EventTypeName}", eventType.Name);

        if (!_handlers.TryGetValue(eventType, out var handlers))
            return;

        lock (handlers)
        {
            handlers.RemoveAll(h =>
            {
                // Compare the original delegate by unwrapping
                if (h.Target is Func<object, TEvent, Task> original)
                    return original == handler;
                return false;
            });

            if (handlers.Count == 0)
                _handlers.TryRemove(eventType, out _);
        }
    }

    public async Task PublishAsync<TEvent>(object sender, TEvent eventArgs) where TEvent : ProjectEvent
    {
        if (_states.TryGetValue(sender, out var state))
        {
            eventArgs.StateId = state;
            var nextState = Guid.CreateVersion7();
            _states[sender] = nextState;
            eventArgs.NextStateId = nextState;
        }
        else
        {
            var nextState = Guid.CreateVersion7();
            _states.TryAdd(sender, nextState);
            eventArgs.NextStateId = nextState;
            eventArgs.StateId = Guid.Empty; // First state.
        }

        if (_handlers.TryGetValue(typeof(TEvent), out var handlers))
        {
            List<Func<object, object, Task>> handlersCopy;
            lock (handlers)
            {
                handlersCopy = new List<Func<object, object, Task>>(handlers);
            }

            foreach (var handler in handlersCopy)
            {
                try
                {
                    await handler(sender, eventArgs);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Handler for {type} failed!", typeof(TEvent).Name);
                }
            }
        }
    }

    public Guid GetNextState(object stateFor)
    {
        return _states.TryGetValue(stateFor, out var nextState) ? nextState : Guid.Empty;
    }
}

/// <summary>
/// Exception that indicates that a state passed into the method does not reflect the real state.
/// </summary>
public sealed class SynchronizationException : Exception
{
    public override string Message { get; }

    public SynchronizationException(object sender, Guid expectedState, Guid realState)
    {
        Message = $"State for object {sender} was {realState}, expected {expectedState}";
        Data.Add(UiException.RequiresReloadKey, true);
    }
}


public class CardMovedEvent : ProjectEvent
{
    /// <summary>
    /// The card we moved.
    /// </summary>
    public required Guid CardId { get; set; }
    /// <summary>
    /// The list the card is now a part of.
    /// </summary>
    public required int NewListId { get; set; }
    /// <summary>
    /// The list where the card previously was.
    /// </summary>
    public required int OldListId { get; set; }
    /// <summary>
    /// A dictionary of the positions of the cards in the new list.
    /// </summary>
    public required Dictionary<Guid, float> Orders { get; set; }
}

public class LaneCreatedEvent : ProjectEvent
{
    public required string Title { get; init; }
    public required int ListId { get; init; }
}

public class LaneRemovedEvent : ProjectEvent
{
    public required int ListId { get; init; }
}

public class LaneUpdatedEvent : ProjectEvent
{
    // TODO: Order?
    public required string Title { get; set; }
    public required int LaneId { get; set; }
}

public class CardUpdatedEvent : ProjectEvent
{
    public required Card Card { get; set; }
}

public class CommentAddedEvent : ProjectEvent
{
    public required CardComment Comment { get; set; }
}

public class CommentEditedEvent : ProjectEvent
{
    public required CardComment Comment { get; set; }
}

public class CommentDeletedEvent : ProjectEvent
{
    public required Guid CommentId { get; set; }
}

public abstract class ProjectEvent
{
    /// <summary>
    /// Next state you should expect.
    /// </summary>
    public Guid NextStateId { get; set; }
    /// <summary>
    /// Unique state.
    /// </summary>
    public Guid StateId { get; set; }
}
