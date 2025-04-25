using SS14.Jetfish.Core.Commands;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Projects.Model;
using SS14.Jetfish.Projects.Model.FormModel;

namespace SS14.Jetfish.Projects.Commands;

public class CreateProjectCommand : BaseCommand<Result<Project, Exception>>
{
    public ProjectFormModel Model { get; set; }

    public CreateProjectCommand(ProjectFormModel model)
    {
        Model = model;
    }

    public override string Name => nameof(CreateProjectCommand);
}