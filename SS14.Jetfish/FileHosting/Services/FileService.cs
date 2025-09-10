using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
    private readonly IOptionsMonitor<FileConfiguration> _fileConfiguration;
    private readonly ILogger<FileService> _logger;

    private readonly Dictionary<string, List<IUploadConverter>> _converters = new();

    public FileService(
        IAuthorizationService authorizationService,
        ApplicationDbContext context,
        IOptionsMonitor<FileConfiguration> fileConfiguration,
        ILogger<FileService> logger)
    {
        _logger = logger;
        _fileConfiguration = fileConfiguration;
        _authorizationService = authorizationService;
        _dbContext = context;

        // The order the converters are configured in is important as a converter replacing the original file stops the ones after it from running..
        _converters.Add("image/gif", [
            new ToWebpImageConverter("static"),
            new GifToVideoConverter(_fileConfiguration),
        ]);
    }

    public async Task<Result<UploadedFile, Exception>> UploadGlobalFile(Stream fileStream, string name, string contentType)
    {
        var configuration = _fileConfiguration.CurrentValue;

        if (fileStream.Length > configuration.MaxUploadSize)
        {
            var exception = new IOException($"File size of {fileStream.Length} exceed maximum upload size {configuration.MaxUploadSize.Bytes}!");
            return Result<UploadedFile, Exception>.Failure(exception);
        }

        var fileId = Guid.NewGuid();
        var usages = new List<FileUsage>()
        {
            new()
            {
                ProjectId = null,
                CardId = null,
                UploadedFileId = fileId,
                Public = true,
            },
        };

        return await UploadFile(fileId, name, contentType, fileStream, Guid.Empty, usages);
    }

    public async Task<Result<UploadedFile, Exception>> UploadGlobalFileUser(IBrowserFile file, Guid userId)
    {
        var fileId = Guid.NewGuid();
        await using var stream = file.OpenReadStream(_fileConfiguration.CurrentValue.MaxUploadSize);

        return await UploadFile(fileId, file.Name, file.ContentType, stream, userId, [
            new FileUsage
            {
                UploadedFileId = fileId,
                Public = true,
            },
        ]);
    }

    public async Task<Result<UploadedFile, Exception>> UploadFileForProject(IBrowserFile file, Guid userId, Guid projectId, Guid? cardId = null)
    {
        var fileId = Guid.NewGuid();
        await using var stream = file.OpenReadStream(_fileConfiguration.CurrentValue.MaxUploadSize);

        return await UploadFile(fileId, file.Name, file.ContentType, stream, userId, [
            new FileUsage
            {
                ProjectId = projectId,
                CardId = cardId,
                UploadedFileId = fileId,
                Public = false,
            },
        ]);
    }

    private async Task<Result<UploadedFile, Exception>> UploadFile(
        Guid fileId,
        string rawFileName,
        string contentType,
        Stream fileStream,
        Guid userId,
        List<FileUsage> usages)
    {
        var fileName = $"{fileId}{Path.GetExtension(rawFileName)}";

        try
        {
            var resolvedPath = Path.Combine(_fileConfiguration.CurrentValue.UserContentDirectory, fileName);
            await using FileStream fs = new(resolvedPath, FileMode.Create);
            await fileStream.CopyToAsync(fs);
        }
        catch (IOException e)
        {
            return Result<UploadedFile, Exception>.Failure(e);
        }

        var createdFile = await _dbContext.UploadedFile.AddAsync(new UploadedFile
        {
            RelativePath = fileName,
            Name = WebUtility.HtmlEncode(rawFileName),
            Id = fileId,
            MimeType = contentType,
            Etag = $"{fileId.GetHashCode():X}{DateTime.Now.GetHashCode():X}",
            UploadedById = userId,
            Usages = usages,
        });

        await _dbContext.SaveChangesAsync();
        await RunConversions(createdFile.Entity);

        return Result<UploadedFile, Exception>.Success(createdFile.Entity);
    }

    private async Task RunConversions(UploadedFile file)
    {
        if (!_converters.TryGetValue(file.MimeType, out var converters))
            return;

        var config = _fileConfiguration.CurrentValue;

        var outputPath = Path.Combine(config.UserContentDirectory, ConvertedDirectory);
        Directory.CreateDirectory(outputPath);

        foreach (var converter in converters)
        {

            var resolvedPath = Path.Combine(config.UserContentDirectory, file.RelativePath);
            var ctSource = new CancellationTokenSource();
            ctSource.CancelAfter(TimeSpan.FromSeconds(30));
            var ct = ctSource.Token;
            var fileId = Guid.NewGuid();
            var result = await converter.Convert(resolvedPath, outputPath, ct);

            if (result.Skipped)
                continue;

            // If the converter replaces the original file stop running conversions and mark the file as being converted.
            if (result.QueuedReplacement)
            {
                file.IsConverting = true;
                await _dbContext.SaveChangesAsync(ct);
                break;
            }

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

    public async Task<IResult> GetGlobalFileAsResult(Guid fileId, string? label = null)
    {
        var file = await _dbContext.UploadedFile
            .Include(file => file.Usages)
            .FirstOrDefaultAsync(file =>
                file.Id == fileId && file.Usages.Any(usage => usage.Public)
            );

        if (file == null)
            return Results.NotFound();

        return await InternalGetFileResult(file, label);
    }

    public async Task<IResult> GetProjectFileAsResult(ClaimsPrincipal principal, Guid projectId, Guid fileId, string? label = null)
    {
        var file = await _dbContext.UploadedFile
            .Include(file => file.Usages)
            .FirstOrDefaultAsync(file =>
            file.Id == fileId && file.Usages.Any(usage => usage.ProjectId == projectId)
        );

        if (file == null)
            return Results.NotFound();

        var isPublicProject = await _dbContext.Project
            .Where(p => p.Id == projectId)
            .Select(p => p.Public)
            .FirstOrDefaultAsync();

        var authorizationResult = await _authorizationService.AuthorizeAsync(principal, projectId, nameof(Permission.ProjectRead));
        if (!authorizationResult.Succeeded && !isPublicProject)
            return Results.Unauthorized();

        return await InternalGetFileResult(file, label);
    }

    /*
     * Gets a file uploaded by the given user principal
     */
    public async Task<IResult> GetUserFileAsResult(ClaimsPrincipal principal, Guid fileId, string? label = null)
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
        return await InternalGetFileResult(file, label);
    }

    private async Task<IResult> InternalGetFileResult(UploadedFile file, string? label)
    {
        var (path, rawEtag, mimeType) = await ResolveConversions(file, label);

        var resolvedPath = Path.Combine(_fileConfiguration.CurrentValue.UserContentDirectory, path);
        if (!File.Exists(resolvedPath))
        {
            _logger.LogError("File exists in DB but not in file system. Path {File}", path);
            return Results.InternalServerError("File does not exist in file system, but exists in DB.");
        }

        var etag = new EntityTagHeaderValue($"\"{rawEtag}\"");
        return Results.File(Path.GetFullPath(resolvedPath), mimeType, file.Name, file.LastModified, etag, true);
    }

    private async Task<(string path, string rawEtag, string mimeType)> ResolveConversions(UploadedFile file, string? label)
    {
        var alternative = await _dbContext.ConvertedFile
            .Where(convertedFile => convertedFile.UploadedFileId == file.Id)
            .Where(convertedFile => convertedFile.Label == label)
            .SingleOrDefaultAsync();


        return alternative != null
            ? (Path.Combine(ConvertedDirectory, alternative.RelativePath), alternative.Etag, alternative.MimeType)
            :  (file.RelativePath, file.Etag, file.MimeType);
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
        var config = _fileConfiguration.CurrentValue;

        if (!Directory.Exists(config.UserContentDirectory) || !Directory.Exists(config.ConvertedContentDirectory))
            return;

        // TODO: Verify that the user content directory is not set to something "dangerous" like the root folder and we just delete every file in there
        // Possibly if the directory is too shallow?

        // Part 1, files that are not in the DB.
        var files = Directory.EnumerateFiles(config.UserContentDirectory);
        foreach (var file in files)
        {
            var dbFileFound = await _dbContext.UploadedFile.AnyAsync(x => x.RelativePath == Path.GetFileName(file));
            if (dbFileFound)
                continue;

            _logger.LogInformation("File {FilePath} was not found in the DB, but exists in the file system, deleting.", file);
            File.Delete(file);
        }

        // Part 1.5, converted files that are not in the DB.
        var convertedFiles = Directory.EnumerateFiles(Path.Combine(config.UserContentDirectory, ConvertedDirectory));
        foreach (var file in convertedFiles)
        {
            var dbFileFound = await _dbContext.ConvertedFile.AnyAsync(x => x.RelativePath.Equals(Path.GetFileName(file)));
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

        // TODO: do this for converted files as well

        // Part 3: Files that are in the DB but have no file in the file system.
        var dbFiles = await _dbContext.UploadedFile
            .Select(f => new { f.RelativePath, f.Id, f.Name }) // thats the only part we care about
            .ToListAsync();

        foreach (var dbFile in dbFiles)
        {
            if (File.Exists(Path.Combine(config.UserContentDirectory, dbFile.RelativePath)))
                continue;

            _logger.LogInformation("File {Name} has no attached file system file, deleting.", dbFile.Name);
            // Db file is a tuple, so we refetch the file from the db
            // realistically, the performance gain from querying all files and only taking 3 columns is much greater than refetching the file again.
            await DeleteFile(_dbContext.UploadedFile.First(x => x.Id == dbFile.Id));
        }
    }

    public async Task DeleteFile(Guid fileId)
    {
        var file = await _dbContext.UploadedFile.FirstOrDefaultAsync(x => x.Id == fileId);
        if (file == null)
        {
            _logger.LogError("Tried deleting a nonexistent file with id {FileId}", fileId);
            return;
        }

        await DeleteFile(file);
    }

    /// <summary>
    /// Deletes a given file.
    /// </summary>
    // TODO: Make this use Result
    public async Task DeleteFile(UploadedFile file)
    {
        var convertedFiles = await _dbContext.ConvertedFile
            .Where(usage => usage.UploadedFileId == file.Id)
            .ToListAsync();

        var config = _fileConfiguration.CurrentValue;

        foreach (var convertedFile in convertedFiles)
        {
            if (File.Exists(Path.Combine(_fileConfiguration.CurrentValue.UserContentDirectory, convertedFile.RelativePath)))
                File.Delete(Path.Combine(_fileConfiguration.CurrentValue.UserContentDirectory, convertedFile.RelativePath));
        }

        _dbContext.ConvertedFile.RemoveRange(convertedFiles);
        _dbContext.UploadedFile.Remove(file);
        await _dbContext.SaveChangesAsync();

        if (File.Exists(Path.Combine(_fileConfiguration.CurrentValue.UserContentDirectory, file.RelativePath)))
            File.Delete(Path.Combine(_fileConfiguration.CurrentValue.UserContentDirectory, file.RelativePath));

        _logger.LogInformation("Deleted file {FileName} - {Id}", file.RelativePath, file.Id);
    }
}
