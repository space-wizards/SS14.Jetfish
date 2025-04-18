using SS14.Jetfish.Core.Commands;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Security.Commands;

public class DeleteTeamCommand : BaseCommand<Result<Team, Exception>>
{
    public override string Name => nameof(DeleteTeamCommand);
    public readonly Team Team;

    public DeleteTeamCommand(Team team)
    {
        Team = team;
    }
}