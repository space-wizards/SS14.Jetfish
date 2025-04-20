using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Projects.Model;
using SS14.Jetfish.Projects.Repositories;

namespace SS14.Jetfish.Projects.Commands.Handlers;

public class CreateOrUpdateProjectCommandHandler : BaseCommandHandler<CreateOrUpdateProjectCommand, Result<Project, Exception>>
{
    private readonly ProjectRepository _projectRepository;

    public CreateOrUpdateProjectCommandHandler(ProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public override string CommandName => nameof(CreateOrUpdateProjectCommand);

    protected override async Task<CreateOrUpdateProjectCommand> Handle(CreateOrUpdateProjectCommand command)
    {
        var result  = await _projectRepository.AddOrUpdate(command.Project);
        command.Result = result;

        return command;
    }
}