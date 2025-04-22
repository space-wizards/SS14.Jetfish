using SS14.Jetfish.Core.Commands;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Security.Model;
using SS14.Jetfish.Security.Model.FormModel;

namespace SS14.Jetfish.Security.Commands;

public class AddTeamMemberCommand : BaseCommand<Result<TeamMember, Exception>>
{
    public override string Name => nameof(AddTeamMemberCommand);

    public readonly NewMemberFormModel Model;

    public AddTeamMemberCommand(NewMemberFormModel model)
    {
        Model = model;
    }
}