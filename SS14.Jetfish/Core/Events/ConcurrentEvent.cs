namespace SS14.Jetfish.Core.Events;

public abstract class ConcurrentEvent
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
