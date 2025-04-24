using SS14.Jetfish.Core.Commands;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Projects.Model;

namespace SS14.Jetfish.Projects.Commands;

public class DeleteProjectCommand : BaseCommand<Result<Project, Exception>>
{
    public override string Name => nameof(DeleteProjectCommand);

    public Project Project { get; set; }

    public DeleteProjectCommand(Project project)
    {
        Project = project;
    }
}