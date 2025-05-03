using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace SS14.Jetfish.Components.Pages.User;

public partial class UserAvatar : ComponentBase
{
    [CascadingParameter]
    public Security.Model.User? User { get; set; }
    private string UserProfilePictureLink => $"global-file/{User!.ProfilePicture}";

    [Parameter]
    public Size Size { get; set; } = Size.Medium;
}