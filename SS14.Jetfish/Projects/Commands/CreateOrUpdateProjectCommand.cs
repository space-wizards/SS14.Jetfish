using SS14.Jetfish.Core.Commands;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Projects.Model;

namespace SS14.Jetfish.Projects.Commands;

public class CreateOrUpdateProjectCommand : BaseCommand<Result<Project, Exception>>
{
    public Project Project { get; set; }

    public CreateOrUpdateProjectCommand(Project project)
    {
        Project = project;
    }

    public override string Name => nameof(CreateOrUpdateProjectCommand);
}