namespace SS14.Jetfish.Core.Types;

public sealed class UiException : Exception
{
    public const string RequiresReloadKey = "requires_reload";
    
    public UiException(string? message, bool requiresReload = true) : base(message)
    {
        Data.Add(RequiresReloadKey, requiresReload);
    }


}

public static class UiExceptionExtension
{
    public static bool RequiresReload(this Exception exception)
    {
        return exception.Data[UiException.RequiresReloadKey] is null or true;
    }
}