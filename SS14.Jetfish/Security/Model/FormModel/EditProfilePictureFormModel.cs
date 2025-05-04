using Microsoft.AspNetCore.Components.Forms;

namespace SS14.Jetfish.Security.Model.FormModel;

/// <summary>
/// This form model is responsible for editing the profile picture of a user.
/// Either <see cref="DefaultPicture"/> or <see cref="UploadedPicture"/> must be set for the form to be valid.
/// </summary>
public class EditProfilePictureFormModel
{
    public User User { get; set; } = null!;

    public string? DefaultPicture { get; set; }
    public IBrowserFile? UploadedPicture { get; set; }
}
