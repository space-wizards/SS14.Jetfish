using SS14.Jetfish.Core.Commands;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Security.Model;
using SS14.Jetfish.Security.Model.FormModel;

namespace SS14.Jetfish.Security.Commands;

public sealed class CreateTeamCommand : BaseCommand<Result<Team, Exception>>
{
    public override string Name => nameof(CreateTeamCommand);
    
    public readonly NewTeamFormModel Model;

    public CreateTeamCommand(NewTeamFormModel model)
    {
        Model = model;
    }
}