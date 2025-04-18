using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Security.Model;
using SS14.Jetfish.Security.Repositories;

namespace SS14.Jetfish.Security.Commands.Handlers;

public class DeleteRoleCommandHandler : BaseCommandHandler<DeleteRoleCommand, Result<Role, Exception>>
{
    private readonly RoleRepository _roleRepository;

    public DeleteRoleCommandHandler(RoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public override string CommandName => nameof(DeleteRoleCommand);

    protected override async Task<DeleteRoleCommand> Handle(DeleteRoleCommand command)
    {
        var result  = await _roleRepository.Delete(command.Role);
        command.Result = result;

        return command;
    }
}