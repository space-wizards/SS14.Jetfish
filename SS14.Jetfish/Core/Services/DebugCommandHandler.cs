using SS14.Jetfish.Core.Commands;

namespace SS14.Jetfish.Core.Services;

public class DebugCommandHandler : BaseCommandHandler<DebugCommand, string>
{
    private readonly ILogger<DebugCommandHandler> _logger;

    public DebugCommandHandler(ILogger<DebugCommandHandler> logger)
    {
        _logger = logger;
    }

    public override string CommandName => nameof(DebugCommand);
    protected override Task<DebugCommand> Handle(DebugCommand command)
    {
        _logger.LogDebug("Debug Command: {Message}", command.Message);
        command.Result = "Test";
        return Task.FromResult(command);
    }
}