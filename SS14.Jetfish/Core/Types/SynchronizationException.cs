namespace SS14.Jetfish.Core.Types;

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
