using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace SS14.Jetfish.Components.Pages.User;

public partial class UserAvatar : ComponentBase
{
    [Parameter]
    public string Class { get; set; } = string.Empty;

    [CascadingParameter]
    public Security.Model.User? User { get; set; }

    private Guid UserProfilePicture()
    {
        //return UserOverride != null ? $"global-file/{UserOverride.ProfilePicture}" : $"global-file/{User!.ProfilePicture}";
        var profilePictureId = UserOverride?.ProfilePicture ?? User!.ProfilePicture;
        return Guid.Parse(profilePictureId);
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
