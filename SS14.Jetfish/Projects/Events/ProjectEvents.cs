using SS14.Jetfish.Core.Events;
using SS14.Jetfish.Projects.Model;

namespace SS14.Jetfish.Projects.Events;

public class CardMovedEvent : ConcurrentEvent
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

/// <summary>
/// Basically means we updated background, name or anything else and we need to refresh the state.
/// </summary>
public class ProjectUpdatedEvent : ConcurrentEvent;

public class LaneCreatedEvent : ConcurrentEvent
{
    public required string Title { get; init; }
    public required int ListId { get; init; }
}

public class LaneRemovedEvent : ConcurrentEvent
{
    public required int ListId { get; init; }
}

public class LaneUpdatedEvent : ConcurrentEvent
{
    // TODO: Order?
    public required string Title { get; set; }
    public required int LaneId { get; set; }
}

public class CardUpdatedEvent : ConcurrentEvent
{
    public required Card Card { get; set; }
}

public class CommentAddedEvent : ConcurrentEvent
{
    public required CardComment Comment { get; set; }
}

public class CommentEditedEvent : ConcurrentEvent
{
    public required CardComment Comment { get; set; }
}

public class CommentDeletedEvent : ConcurrentEvent
{
    public required Guid CommentId { get; set; }
}
