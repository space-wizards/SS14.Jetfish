using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Security.Commands;

public class RoleCommandHandler : BaseCommandHandler<CreateOrUpdateRole, Role>
{
    public override string CommandName => nameof(CreateOrUpdateRole);
    protected override Task<CreateOrUpdateRole> Handle(CreateOrUpdateRole command)
    {
        throw new NotImplementedException();
    }
}