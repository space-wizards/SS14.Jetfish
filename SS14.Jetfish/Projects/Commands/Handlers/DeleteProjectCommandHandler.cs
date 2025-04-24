using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Projects.Model;
using SS14.Jetfish.Projects.Repositories;

namespace SS14.Jetfish.Projects.Commands.Handlers;

public class DeleteProjectCommandHandler : BaseCommandHandler<DeleteProjectCommand, Result<Project, Exception>>
{
    private readonly ProjectRepository _projectRepository;

    public DeleteProjectCommandHandler(ProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public override string CommandName => nameof(DeleteProjectCommand);
    protected override async Task<DeleteProjectCommand> Handle(DeleteProjectCommand command)
    {
        command.Result = await _projectRepository.Delete(command.Project);
        return command;
    }
}