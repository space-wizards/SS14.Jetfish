using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Security.Model;
using SS14.Jetfish.Security.Repositories;

namespace SS14.Jetfish.Security.Commands.Handlers;

public class DeleteTeamMemberCommandHandler : BaseCommandHandler<DeleteTeamMemberCommand, Result<TeamMember, Exception>>
{
    private readonly TeamMemberRepository _teamMemberRepository;

    public DeleteTeamMemberCommandHandler(TeamMemberRepository teamMemberRepository)
    {
        _teamMemberRepository = teamMemberRepository;
    }

    public override string CommandName => nameof(DeleteTeamMemberCommand);
    
    protected override async Task<DeleteTeamMemberCommand> Handle(DeleteTeamMemberCommand command)
    {
        var result  = await _teamMemberRepository.Delete(command.TeamMember);
        command.Result = result;

        return command;
    }
}