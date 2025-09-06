using Microsoft.EntityFrameworkCore;
using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Services.Interfaces;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Database;
using SS14.Jetfish.FileHosting.Services;
using SS14.Jetfish.Projects.Events;
using SS14.Jetfish.Projects.Model;

namespace SS14.Jetfish.Projects.Commands.Handlers;

public class UpdateProjectCommandHandler : BaseCommandHandler<UpdateProjectCommand, Result<Project, Exception>>
{
    private readonly ApplicationDbContext _context;
    private readonly FileService _fileService;
    private readonly IConcurrentEventBus _eventBus;

    public UpdateProjectCommandHandler(ApplicationDbContext context, FileService fileService, IConcurrentEventBus eventBus)
    {
        _context = context;
        _fileService = fileService;
        _eventBus = eventBus;
    }

    public override string CommandName => nameof(UpdateProjectCommand);
    protected override async Task<UpdateProjectCommand> Handle(UpdateProjectCommand command)
    {
        // Clear tracked entities to prevent errors in case ef already tracked the project about to be updated
        _context.ChangeTracker.Clear();
        await using var transaction = await _context.Database.BeginTransactionAsync();

        var project = await _context.Project.SingleOrDefaultAsync(project => project.Id == command.ProjectId);
        if (project == null)
        {
            command.Result = Result<Project, Exception>.Failure(new UiException("Project not found"));
            await transaction.RollbackAsync();
            return command;
        }

        project.Name = command.Model.Name;
        project.BackgroundSpecifier = command.Model.BackgroundSpecifier;
        project.Public = command.Model.Public;

        if (project.BackgroundSpecifier == ProjectBackgroundSpecifier.Image
            && (command.Model.BackgroundFile != null || command.Model.BackgroundSpecifier == ProjectBackgroundSpecifier.Color)
            && Guid.TryParse(project.Background, out var backgroundId))
        {
            await _fileService.DeleteProjectFileUsage(project.Id, backgroundId);
        }

        if (command.Model is { BackgroundSpecifier: ProjectBackgroundSpecifier.Image, BackgroundFile: not null })
        {
            var result = await _fileService.UploadFileForProject(command.Model.BackgroundFile, command.Model.UserId, project.Id);
            if (!result.IsSuccess)
            {
                command.Result = Result<Project, Exception>.Failure(result.Error);
                await transaction.RollbackAsync();
                return command;
            }
            project.Background = result.Value.Id.ToString();
        }

        if (command.Model.BackgroundSpecifier == ProjectBackgroundSpecifier.Color)
            project.Background = command.Model.BackgroundColor ?? "#000000";

        command.Model.Team.Projects.Add(project);
        await _context.SaveChangesAsync();
        command.Result = Result<Project, Exception>.Success(project);
        await transaction.CommitAsync();
        await _eventBus.PublishAsync(project.Id, new ProjectUpdatedEvent());
        return command;
    }
}
