using SS14.Jetfish.Core.Commands;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Security.Commands;

public class ChangeTeamMemberRoleCommand : BaseCommand<Result<TeamMember, Exception>>
{
    public override string Name => nameof(ChangeTeamMemberRoleCommand);
    
    public readonly Guid TeamId;
    public readonly Guid UserId;
    public readonly Guid RoleId;

    public ChangeTeamMemberRoleCommand(Guid teamId, Guid userId, Guid roleId)
    {
        RoleId = roleId;
        TeamId = teamId;
        UserId = userId;
    }
}