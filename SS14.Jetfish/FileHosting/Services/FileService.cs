using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Serilog;
using SS14.Jetfish.Configuration;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Database;
using SS14.Jetfish.FileHosting.Model;
using SS14.Jetfish.Helpers;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.FileHosting.Services;

public sealed class FileService
{
    private readonly IAuthorizationService _authorizationService;
    private readonly ApplicationDbContext _dbContext;
    private readonly ServerConfiguration _serverConfiguration = new();
    private readonly ILogger<FileService> _logger;

    public FileService(IAuthorizationService authorizationService, ApplicationDbContext context, IConfiguration configuration, ILogger<FileService> logger)
    {
        _logger = logger;
        _authorizationService = authorizationService;
        _dbContext = context;
        configuration.Bind(ServerConfiguration.Name, _serverConfiguration);
    }

    public async Task<Result<UploadedFile, Exception>> UploadGlobalFile(Stream fileStream, string name, string contentType)
    {
        var fileId = Guid.NewGuid();
        var fileName = $"{fileId}{Path.GetExtension(name)}";

        if (fileStream.Length > _serverConfiguration.MaxUploadSize)
            throw new IOException(
                $"File size of {fileStream.Length} exceed maximum upload size {_serverConfiguration.MaxUploadSize.Bytes}!");

        try
        {
            var resolvedPath = Path.Combine(_serverConfiguration.UserContentDirectory, fileName);
            await using FileStream fs = new(resolvedPath, FileMode.Create);
        }
        catch (IOException e)
        {
            return Result<UploadedFile, Exception>.Failure(e);
        }

        var createdFile = await _dbContext.UploadedFile.AddAsync(new UploadedFile()
        {
            RelativePath = fileName,
            Name = WebUtility.HtmlEncode(name),
            Id = fileId,
            MimeType = contentType,
            Etag = $"{fileId.GetHashCode():X}{DateTime.Now.GetHashCode():X}",
            UploadedById = null,
            Usages = new List<FileUsage>()
            {
                new()
                {
                    ProjectId = null,
                    CardId = null,
                    UploadedFileId = fileId,
                    Public = true,
                }
            }
        });

        await _dbContext.SaveChangesAsync();

        return Result<UploadedFile, Exception>.Success(createdFile.Entity);
    }

    public async Task<Result<UploadedFile, Exception>> UploadFileForProject(IBrowserFile file, Guid userId, Guid projectId, Guid? cardId = null)
    {
        var fileId = Guid.NewGuid();
        var fileName = $"{fileId}{Path.GetExtension(file.Name)}";

        try
        {
            var resolvedPath = Path.Combine(_serverConfiguration.UserContentDirectory, fileName);
            await using FileStream fs = new(resolvedPath, FileMode.Create);
            await file.OpenReadStream(_serverConfiguration.MaxUploadSize).CopyToAsync(fs);
        }
        catch (IOException e)
        {
            return Result<UploadedFile, Exception>.Failure(e);
        }

        var createdFile = await _dbContext.UploadedFile.AddAsync(new UploadedFile()
        {
            RelativePath = fileName,
            Name = WebUtility.HtmlEncode(file.Name),
            Id = fileId,
            MimeType = file.ContentType,
            Etag = $"{fileId.GetHashCode():X}{DateTime.Now.GetHashCode():X}",
            UploadedById = userId,
            Usages = new List<FileUsage>()
            {
                new()
                {
                    ProjectId = projectId,
                    UploadedFileId = fileId,
                    CardId = cardId
                }
            }
        });

        await _dbContext.SaveChangesAsync();

        return Result<UploadedFile, Exception>.Success(createdFile.Entity);
    }

    public async Task<IResult> GetGlobalFileAsResult(Guid fileId)
    {
        var file = await _dbContext.UploadedFile
            .Include(file => file.Usages)
            .FirstOrDefaultAsync(file =>
                file.Id == fileId && file.Usages.Any(usage => usage.Public)
            );

        if (file == null)
            return Results.NotFound();

        return InternalGetFileResult(file);
    }

    public async Task<IResult> GetProjectFileAsResult(ClaimsPrincipal principal, Guid projectId, Guid fileId)
    {
        var file = await _dbContext.UploadedFile
            .Include(file => file.Usages)
            .FirstOrDefaultAsync(file =>
            file.Id == fileId && file.Usages.Any(usage => usage.ProjectId == projectId)
        );

        if (file == null)
            return Results.NotFound();

        var project = file.Usages.First(usage => usage.ProjectId == projectId);
        var authorizationResult = await _authorizationService.AuthorizeAsync(principal, project, nameof(Permission.ProjectRead));
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
            file.Id == fileId && file.UploadedBy != null && file.UploadedBy.Id == principal.Claims.GetUserId());

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
            _logger.LogError("File exists in DB but not in file system. Path {File}", file.RelativePath);
            return Results.InternalServerError("File does not exist in file system, but exists in DB.");
        }

        var etag = new EntityTagHeaderValue($"\"{file.Etag}\"");
        return Results.File(Path.GetFullPath(resolvedPath), file.MimeType, file.Name, file.LastModified, etag, true);
    }

    public async Task DeleteProjectFileUsage(Guid projectId, Guid backgroundId)
    {
        await _dbContext.FileUsage
            .Where(usage => usage.Id == backgroundId)
            .Where(usage => usage.ProjectId == projectId)
            .ExecuteDeleteAsync();
    }

    /// <summary>
    /// Deletes "lost files" that are in the file system but not the DB and deletes files without any usages.
    /// </summary>
    public async Task DeleteLostFiles()
    {
        if (!Directory.Exists(_serverConfiguration.UserContentDirectory))
            return;

        // TODO: Verify that the user content directory is not set to something "dangerous" like the root folder and we just delete every file in there
        // Possibly if the directory is too shallow?

        // Part 1, files that are not in the DB.
        var files = Directory.EnumerateFiles(_serverConfiguration.UserContentDirectory);
        foreach (var file in files)
        {
            var dbFileFound = await _dbContext.UploadedFile.AnyAsync(x => x.RelativePath == Path.GetFileName(file));
            if (dbFileFound)
                continue;

            _logger.LogInformation("File {FilePath} was not found in the DB, but exists in the file system, deleting.", file);
            File.Delete(file);
        }

        // Part 2, files without any usages.
        var lostFiles = await _dbContext.UploadedFile
            .Include(f => f.Usages)
            .Where(f => f.Usages.Count == 0)
            .ToListAsync();

        foreach (var lostFile in lostFiles)
        {
            _logger.LogInformation("File {Name} has no usages, deleting.", lostFile.Name);
            await DeleteFile(lostFile);
        }
    }

    /// <summary>
    /// Deletes a given file.
    /// </summary>
    // TODO: Make this use Result
    public async Task DeleteFile(UploadedFile file)
    {
        _dbContext.UploadedFile.Remove(file);
        await _dbContext.SaveChangesAsync();

        if (File.Exists(Path.Combine(_serverConfiguration.UserContentDirectory, file.RelativePath)))
            File.Delete(Path.Combine(_serverConfiguration.UserContentDirectory, file.RelativePath));

        _logger.LogInformation("Deleted file {FileName} - {Id}", file.RelativePath, file.Id);
    }
}