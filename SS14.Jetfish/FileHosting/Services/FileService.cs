using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using SS14.Jetfish.Configuration;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Database;
using SS14.Jetfish.FileHosting.Converters;
using SS14.Jetfish.FileHosting.Model;
using SS14.Jetfish.Helpers;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.FileHosting.Services;

public sealed class FileService
{
    private const string ConvertedDirectory = "Converted";

    private readonly IAuthorizationService _authorizationService;
    private readonly ApplicationDbContext _dbContext;
    private readonly ServerConfiguration _serverConfiguration = new();
    private readonly ILogger<FileService> _logger;

    private readonly Dictionary<string, List<IUploadConverter>> _converters = new();

    public FileService(IAuthorizationService authorizationService, ApplicationDbContext context, IConfiguration configuration, ILogger<FileService> logger)
    {
        _logger = logger;
        _authorizationService = authorizationService;
        _dbContext = context;
        configuration.Bind(ServerConfiguration.Name, _serverConfiguration);

        _converters.Add("image/gif", [new ToWebpImageConverter("static")]);
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
            await fileStream.CopyToAsync(fs);
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

        await RunConversions(createdFile.Entity);

        return Result<UploadedFile, Exception>.Success(createdFile.Entity);
    }

    public async Task<Result<UploadedFile, Exception>> UploadGlobalFileUser(IBrowserFile file, Guid userId)
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
                    UploadedFileId = fileId,
                    Public = true,
                }
            }
        });

        await _dbContext.SaveChangesAsync();

        await RunConversions(createdFile.Entity);

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

        var createdFile = await _dbContext.UploadedFile.AddAsync(new UploadedFile
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

        await RunConversions(createdFile.Entity);

        return Result<UploadedFile, Exception>.Success(createdFile.Entity);
    }

    private async Task RunConversions(UploadedFile file)
    {
        if (!_converters.TryGetValue(file.MimeType, out var converters))
            return;

        var outputPath = Path.Combine(_serverConfiguration.UserContentDirectory, ConvertedDirectory);
        Directory.CreateDirectory(outputPath);

        foreach (var converter in converters)
        {

            var resolvedPath = Path.Combine(_serverConfiguration.UserContentDirectory, file.RelativePath);
            var ctSource = new CancellationTokenSource();
            ctSource.CancelAfter(TimeSpan.FromSeconds(30));
            var ct = ctSource.Token;
            var fileId = Guid.NewGuid();
            var result = await converter.Convert(resolvedPath, outputPath, ct);

            await _dbContext.ConvertedFile.AddAsync(new ConvertedFile
            {
                UploadedFileId = file.Id,
                RelativePath = result.Path,
                Id = fileId,
                MimeType = result.MimeType,
                Label = converter.ConverterLabel,
                Etag = $"{fileId.GetHashCode():X}{DateTime.Now.GetHashCode():X}",
            }, ct);
        }

        await _dbContext.SaveChangesAsync();
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

        var authorizationResult = await _authorizationService.AuthorizeAsync(principal, projectId, nameof(Permission.ProjectRead));
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

        // Part 1.5, converted files that are not in the DB.
        if(Directory.Exists(Path.Combine(_serverConfiguration.UserContentDirectory, ConvertedDirectory))) {
            var convertedFiles = Directory.EnumerateFiles(Path.Combine(_serverConfiguration.UserContentDirectory, ConvertedDirectory));
            foreach (var file in convertedFiles)
            {
                var dbFileFound = await _dbContext.ConvertedFile.AnyAsync(x => x.RelativePath == Path.GetFileName(file));
                if (dbFileFound)
                    continue;

                _logger.LogInformation("File {FilePath} was not found in the DB, but exists in the file system, deleting.", file);
                File.Delete(file);
            }
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

        // Part 3: Files that are in the DB but have no file in the file system.
        var dbFiles = await _dbContext.UploadedFile
            .Select(f => new { f.RelativePath, f.Id, f.Name }) // thats the only part we care about
            .ToListAsync();

        foreach (var dbFile in dbFiles)
        {
            if (File.Exists(Path.Combine(_serverConfiguration.UserContentDirectory, dbFile.RelativePath)))
                continue;

            _logger.LogInformation("File {Name} has no attached file system file, deleting.", dbFile.Name);
            // Db file is a tuple, so we refetch the file from the db
            // realistically, the performance gain from querying all files and only taking 3 columns is much greater than refetching the file again.
            await DeleteFile(_dbContext.UploadedFile.First(x => x.Id == dbFile.Id));
        }
    }

    public async Task DeleteFile(Guid fileId)
    {
        var file = await _dbContext.UploadedFile.FirstAsync(x => x.Id == fileId);
        await DeleteFile(file);
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
