using SS14.Jetfish.Core.Commands;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Security.Model;
using SS14.Jetfish.Security.Model.FormModel;

namespace SS14.Jetfish.Security.Commands;

public class EditUserProfilePictureCommand : BaseCommand<Result<User, Exception>>
{
    public override string Name => nameof(EditUserProfilePictureCommand);

    public EditProfilePictureFormModel Model;

    public EditUserProfilePictureCommand(EditProfilePictureFormModel model)
    {
        Model = model;
    }
}
