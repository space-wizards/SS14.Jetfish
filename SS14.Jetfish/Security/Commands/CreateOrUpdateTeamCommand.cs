using SS14.Jetfish.Core.Commands;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Security.Commands;

public class CreateOrUpdateTeamCommand : BaseCommand<Result<Team, Exception>>
{
    public readonly Team Team;

    public CreateOrUpdateTeamCommand(Team team)
    {
        Team = team;
    }

    public override string Name => nameof(CreateOrUpdateTeamCommand);
}