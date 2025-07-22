using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using MudBlazor;
using SS14.Jetfish.Configuration;
using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Services.Interfaces;
using SS14.Jetfish.FileHosting;
using SS14.Jetfish.Security.Commands;
using SS14.Jetfish.Security.Model.FormModel;

namespace SS14.Jetfish.Components.Pages.User.Dialogs;

public partial class EditProfilePictureDialog : ComponentBase
{
    [CascadingParameter]
    private IMudDialogInstance Dialog { get; set; } = null!;

    private EditForm _form = null!;
    private EditProfilePictureFormModel _model = null!;
    private string _fileError = "";

    [Parameter]
    public required Security.Model.User User { get; set; }

    [Inject]
    private IOptions<UserConfiguration> UserConfiguration { get; set; } = null!;
    [Inject]
    private IOptions<FileConfiguration> FileConfiguration { get; set; } = null!;
    [Inject]
    private ConfigurationStoreService ConfigurationStoreService { get; set; } = null!;
    [Inject]
    private ICommandService CommandService { get; set; } = null!;
    [Inject]
    private UiErrorService UiErrorService { get; set; } = null!;
    [Inject]
    private ISnackbar Snackbar { get; set; } = null!;

    private Dictionary<string, string> _defaultProfilePictures = new Dictionary<string, string>();

    protected override void OnParametersSet()
    {
        _model = new EditProfilePictureFormModel
        {
            User = User,
        };

        foreach (var defaultPicture in UserConfiguration.Value.DefaultProfilePictures)
        {
            var storeId = StartupAssetHelper.GetDbIdentifier(defaultPicture.Key);
            var fileId = ConfigurationStoreService.Get(storeId);

            _defaultProfilePictures.Add(defaultPicture.Key, fileId);

            if (User.ProfilePicture == fileId)
                _model.DefaultPicture = fileId;
        }
    }

    private void SelectPicture(string id)
    {
        _model.DefaultPicture = id;
        _model.UploadedPicture = null;
        StateHasChanged();
    }

    private bool CheckFileSize()
    {
        if (_model.UploadedPicture == null || _model.UploadedPicture.Size <= FileConfiguration.Value.MaxUploadSize)
            return true;

        _fileError = $"Maximum upload size of {FileConfiguration.Value.MaxUploadSize} exceeded!";
        StateHasChanged();
        return false;
    }

    private void FilesChanges()
    {
        if (CheckFileSize())
            _fileError = string.Empty;

        if (_model.UploadedPicture != null)
            _model.DefaultPicture = null;
    }

    private void Cancel() => Dialog.Close(false);

    private async Task Save()
    {
        if (_model.UploadedPicture != null && !CheckFileSize())
            return; // Not valid

        if (_model.UploadedPicture == null && _model.DefaultPicture == null)
            return;

        var command = new EditUserProfilePictureCommand(_model);
        var commandResult = await CommandService.Run(command);

        if (!commandResult!.Result!.IsSuccess)
        {
            await UiErrorService.HandleUiError(commandResult.Result.Error);
            return;
        }

        Snackbar.Add("Changes Saved!", Severity.Success);
        Dialog.Close(true);
    }
}

