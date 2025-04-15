using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Serilog;
using SS14.Jetfish.Configuration;
using SS14.Jetfish.Database;
using SS14.Jetfish.FileHosting.Model;
using SS14.Jetfish.Helpers;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.FileHosting.Services;

public sealed class FileService
{
    private readonly IAuthorizationService _authorizationService;
    private readonly ApplicationDbContext _dbContext;
    private readonly ServerConfiguration _serverConfiguration = new ServerConfiguration();

    public FileService(IAuthorizationService authorizationService, ApplicationDbContext context, IConfiguration configuration)
    {
        _authorizationService = authorizationService;
        _dbContext = context;
        configuration.Bind(ServerConfiguration.Name, _serverConfiguration);
    }

    public async Task<IResult> GetProjectFileAsResult(ClaimsPrincipal principal, Guid projectId, Guid fileId)
    {
        var file = await _dbContext.UploadedFile
            .Include(file => file.UsedInProjects)
            .FirstOrDefaultAsync(file =>
            file.Id == fileId && file.UsedInProjects.Any(project => project.Id == projectId)
        );

        if (file == null)
            return Results.NotFound();

        var project = file.UsedInProjects.First(project => project.Id == projectId);
        var authorizationResult = await _authorizationService.AuthorizeAsync(principal, project, nameof(AccessArea.ProjectRead));
        if (!authorizationResult.Succeeded)
            return Results.Unauthorized();

        return InternalGetFileResult(file);
    }
    
    /*
     * Gets a file uploaded by the given user principal
     */
    public async Task<IResult> GetUserFileAsResult(ClaimsPrincipal principal, Guid fileId)
    {
        var file = await _dbContext.UploadedFile.FirstOrDefaultAsync(file => 
            file.Id == fileId && file.UploadedBy.Id == principal.Claims.GetUserId());
        
        if (file == null)
            return Results.NotFound();

        //TODO: Pass project instead of file
        /*var authorizationResult = await _authorizationService.AuthorizeAsync(principal, file, nameof(AccessArea.FileRead));
        if (!authorizationResult.Succeeded)
            return Results.Unauthorized();
        */
        return InternalGetFileResult(file);
    }

    private IResult InternalGetFileResult(UploadedFile file)
    {
        var resolvedPath = Path.Combine(_serverConfiguration.UserContentDirectory, file.RelativePath);
        if (!File.Exists(resolvedPath))
        {
            Log.Error("File exists in DB but not in file system. Path {file}", file.RelativePath);
            return Results.InternalServerError("File does not exist in file system, but exists in DB.");
        }

        var etag = new EntityTagHeaderValue(file.Etag);
        return Results.File(resolvedPath, file.MimeType, file.Name, file.LastModified, etag, true);
    }
}