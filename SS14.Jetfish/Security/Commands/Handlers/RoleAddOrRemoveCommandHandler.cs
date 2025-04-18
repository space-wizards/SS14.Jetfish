using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Security.Model;
using SS14.Jetfish.Security.Repositories;

namespace SS14.Jetfish.Security.Commands.Handlers;

public class RoleAddOrRemoveCommandHandler : BaseCommandHandler<CreateOrUpdateRoleCommand, Result<Role, Exception>>
{
    private readonly RoleRepository _roleRepository;

    public RoleAddOrRemoveCommandHandler(RoleRepository role)
    {
        _roleRepository = role;
    }

    public override string CommandName => nameof(CreateOrUpdateRoleCommand);

    protected override async Task<CreateOrUpdateRoleCommand> Handle(CreateOrUpdateRoleCommand command)
    {
        var result  = await _roleRepository.AddOrUpdate(command.Role);
        command.Result = result;

        return command;
    }
}