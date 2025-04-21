using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Database;
using SS14.Jetfish.FileHosting.Services;
using SS14.Jetfish.Projects.Model;

namespace SS14.Jetfish.Projects.Commands.Handlers;

public class CreateProjectCommandHandler : BaseCommandHandler<CreateProjectCommand, Result<Project, Exception>>
{
    private readonly ApplicationDbContext _context;
    private readonly FileService _fileService;

    public CreateProjectCommandHandler(ApplicationDbContext context, FileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public override string CommandName => nameof(CreateProjectCommand);
    protected override async Task<CreateProjectCommand> Handle(CreateProjectCommand command)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        // TODO: Use repositories, they currently dont take transactions but use their own dbcontext

        var project = new Project
        {
            Name = command.Model.Name,
            BackgroundSpecifier = ProjectBackgroundSpecifier.Color,
            Background = command.Model.BackgroundColor ?? "#000000"
        };

        var projectResult = await _context.Project.AddAsync(project);
        await _context.SaveChangesAsync();

        project = projectResult.Entity;

        if (command.Model.BackgroundSpecifier == ProjectBackgroundSpecifier.Image)
        {
            ArgumentNullException.ThrowIfNull(command.Model.BackgroundFile);

            var fileResult = await _fileService.UploadFileForProject(command.Model.BackgroundFile, command.Model.UserId,  project.Id);
            if (!fileResult.IsSuccess)
            {
                await transaction.RollbackAsync();
                command.Result = Result<Project, Exception>.Failure(fileResult.Error);
                return command;
            }

            project.BackgroundSpecifier = ProjectBackgroundSpecifier.Image;
            project.Background = fileResult.Value.Id.ToString();

            await _context.SaveChangesAsync();
        }

        command.Model.Team.Projects.Add(project);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        command.Result = Result<Project, Exception>.Success(project);
        return command;
    }
}