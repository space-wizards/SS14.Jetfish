using Microsoft.Extensions.Options;
using SS14.Jetfish.Configuration;
using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Database;
using SS14.Jetfish.FileHosting;
using SS14.Jetfish.FileHosting.Services;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Security.Commands.Handlers;

public class EditUserProfilePictureCommandHandler : BaseCommandHandler<EditUserProfilePictureCommand, Result<User, Exception>>
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly FileService _fileService;
    private readonly ConfigurationStoreService _configurationStoreService;
    private readonly IOptions<UserConfiguration> _userConfiguration;

    public EditUserProfilePictureCommandHandler(ApplicationDbContext context, FileService fileService, ConfigurationStoreService configurationStoreService, IOptions<UserConfiguration> userConfiguration)
    {
        _applicationDbContext = context;
        _fileService = fileService;
        _configurationStoreService = configurationStoreService;
        _userConfiguration = userConfiguration;
    }

    public override string CommandName => nameof(EditUserProfilePictureCommand);

    protected override async Task<EditUserProfilePictureCommand> Handle(EditUserProfilePictureCommand command)
    {
        await using var transaction = await _applicationDbContext.Database.BeginTransactionAsync();

        if (command.Model.DefaultPicture == null && command.Model.UploadedPicture == null)
        {
            command.Result = Result<User, Exception>.Failure(new Exception("No uploaded picture specified and no default selected either."));
            return command;
        }

        var previousPicture = PreviousPictureMode.Custom;

        foreach (var defaultPicture in _userConfiguration.Value.DefaultProfilePictures)
        {
            var storeId = StartupAssetHelper.GetDbIdentifier(defaultPicture.Key);
            var fileId = _configurationStoreService.Get(storeId);

            if (command.Model.User.ProfilePicture == fileId)
                previousPicture = PreviousPictureMode.Default;
        }

        if (previousPicture == PreviousPictureMode.Custom)
        {
            // User had a previous custom pfp, delete it before continuing.
            await _fileService.DeleteFile(Guid.Parse(command.Model.User.ProfilePicture));
        }

        if (command.Model.UploadedPicture != null)
        {
            var fileResult = await _fileService.UploadGlobalFileUser(command.Model.UploadedPicture, command.Model.User.Id);
            if (!fileResult.IsSuccess)
            {
                await transaction.RollbackAsync();
                command.Result = Result<User, Exception>.Failure(fileResult.Error);
                return command;
            }

            command.Model.User.ProfilePicture = fileResult.Value.Id.ToString();
        }
        else if (command.Model.DefaultPicture != null)
        {
            command.Model.User.ProfilePicture = command.Model.DefaultPicture;
        }

        _applicationDbContext.Update(command.Model.User);
        await _applicationDbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        command.Result = Result<User, Exception>.Success(command.Model.User);
        return command;
    }

    private enum PreviousPictureMode
    {
        Custom,
        Default,
    }
}
