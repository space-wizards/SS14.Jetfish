using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace SS14.Jetfish.Components.Pages.User;

public partial class UserAvatar : ComponentBase
{
    [CascadingParameter]
    public Security.Model.User? User { get; set; }

    private string UserProfilePictureLink()
    {
        return UserOverride != null ? $"global-file/{UserOverride.ProfilePicture}" : $"global-file/{User!.ProfilePicture}";
    }

    private string GetTooltip()
    {
        return UserOverride != null ? UserOverride.DisplayName : User!.DisplayName;
    }

    [Parameter]
    public Size Size { get; set; } = Size.Medium;

    /// <summary>
    /// If set, this user is used instead of <see cref="User"/>
    /// </summary>
    [Parameter]
    public Security.Model.User? UserOverride { get; set; }

    [Parameter]
    public bool Tooltip { get; set; } = false;
}
