using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Security.Model;
using SS14.Jetfish.Security.Repositories;

namespace SS14.Jetfish.Security.Commands.Handlers;

public class CreateOrUpdateTeamCommandHandler : BaseCommandHandler<CreateOrUpdateTeamCommand, Result<Team, Exception>>
{
    private readonly TeamRepository _teamRepository;

    public CreateOrUpdateTeamCommandHandler(TeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public override string CommandName => nameof(CreateOrUpdateTeamCommand);

    protected override async Task<CreateOrUpdateTeamCommand> Handle(CreateOrUpdateTeamCommand command)
    {
        var result  = await _teamRepository.AddOrUpdate(command.Team);
        command.Result = result;

        return command;
    }
}