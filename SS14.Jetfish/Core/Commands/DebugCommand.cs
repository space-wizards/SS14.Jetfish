namespace SS14.Jetfish.Core.Commands;

public class DebugCommand : BaseCommand<string>
{
    public override string Name => nameof(DebugCommand);
    
    public string Message { get; set; } = string.Empty;
}