using SS14.Jetfish.Core.Commands;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Projects.Model;
using SS14.Jetfish.Projects.Model.FormModel;

namespace SS14.Jetfish.Projects.Commands;

public class UpdateProjectCommand : BaseCommand<Result<Project, Exception>>
{
    public override string Name => nameof(UpdateProjectCommand);
    
    public Guid ProjectId { get; set; }
    public ProjectFormModel Model { get; set; }
    
    public UpdateProjectCommand(Guid projectId, ProjectFormModel model)
    {
        ProjectId = projectId;
        Model = model;
    }
}