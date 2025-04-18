using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Security.Model;
using SS14.Jetfish.Security.Repositories;

namespace SS14.Jetfish.Security.Commands.Handlers;

public class DeleteTeamCommandHandler : BaseCommandHandler<DeleteTeamCommand, Result<Team, Exception>>
{
    private readonly TeamRepository _teamRepository;

    public DeleteTeamCommandHandler(TeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public override string CommandName => nameof(DeleteTeamCommand);
    protected override async Task<DeleteTeamCommand> Handle(DeleteTeamCommand command)
    {
        command.Result = await _teamRepository.Delete(command.Team);
        return command;
    }
}