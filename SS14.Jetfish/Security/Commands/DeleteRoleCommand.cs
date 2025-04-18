using SS14.Jetfish.Core.Commands;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Security.Commands;

public class DeleteRoleCommand : BaseCommand<Result<Role, Exception>>
{
    public readonly Role Role;

    public DeleteRoleCommand(Role role)
    {
        Role = role;
    }

    public override string Name => nameof(DeleteRoleCommand);
}