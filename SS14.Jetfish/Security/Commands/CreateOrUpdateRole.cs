using SS14.Jetfish.Core.Commands;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Security.Commands;

public class CreateOrUpdateRole : BaseCommand<Result<Role, Exception>>
{
    public readonly Role Role;

    public CreateOrUpdateRole(Role role)
    {
        Role = role;
    }

    public override string Name => nameof(CreateOrUpdateRole);
}