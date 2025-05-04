using JetBrains.Annotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using SS14.Jetfish.Components.Pages.User.Dialogs;

namespace SS14.Jetfish.Components.Pages.User;

[UsedImplicitly]
public partial class Settings : ComponentBase
{
    [CascadingParameter]
    public Security.Model.User? User { get; set; }

    [Inject]
    private IDialogService DialogService { get; set; } = null!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    private async Task EditProfilePicture()
    {
        var parameters = new DialogParameters<EditProfilePictureDialog> {
        {
            x => x.User, User
        }};

        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
        };

        var dialogResult = await DialogService.ShowAsync<EditProfilePictureDialog>("Change Profile Picture", parameters, options);
        var result = await dialogResult.Result;
        if (result?.Data == null)
            return;

        if (!(bool)result.Data)
            return;

        // We saved, now reload.
        await DialogService.ShowMessageBox(
            "Notice",
            "Changes have been saved, please refresh the site for the changes to take affect.",
            yesText:"OK");

        NavigationManager.Refresh(true);
    }
}
