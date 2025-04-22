using SS14.Jetfish.Core.Commands;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Security.Commands;

public class DeleteTeamMemberCommand : BaseCommand<Result<TeamMember, Exception>>
{
    public override string Name => nameof(DeleteTeamMemberCommand);
    
    public readonly TeamMember TeamMember;

    public DeleteTeamMemberCommand(TeamMember teamMember)
    {
        TeamMember = teamMember;
    }
}