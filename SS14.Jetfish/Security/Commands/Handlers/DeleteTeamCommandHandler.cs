using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Security.Model;
using SS14.Jetfish.Security.Repositories;

namespace SS14.Jetfish.Security.Commands.Handlers;

public class DeleteTeamCommandHandler : BaseCommandHandler<DeleteTeamCommand, Result<Team, Exception>>
{
    private readonly TeamRepository _teamRepository;
    private readonly RoleRepository _roleRepository;
    
    
    public DeleteTeamCommandHandler(TeamRepository teamRepository, RoleRepository roleRepository)
    {
        _teamRepository = teamRepository;
        _roleRepository = roleRepository;
    }

    public override string CommandName => nameof(DeleteTeamCommand);
    protected override async Task<DeleteTeamCommand> Handle(DeleteTeamCommand command)
    {
        var deletedTeamId = command.Team.Id;
        var roles = await _roleRepository.GetAllAsync(deletedTeamId);
        await _roleRepository.Delete(roles);
        command.Result = await _teamRepository.Delete(command.Team);
        
        return command;
    }
}