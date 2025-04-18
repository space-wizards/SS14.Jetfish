using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Database;
using SS14.Jetfish.Security.Model;
using SS14.Jetfish.Security.Repositories;

namespace SS14.Jetfish.Security.Commands;

public class RoleAddOrRemoveCommandHandler : BaseCommandHandler<CreateOrUpdateRoleCommand, Result<Role, Exception>>
{
    private readonly PolicyRepository _policyRepository;

    public RoleAddOrRemoveCommandHandler(PolicyRepository policy)
    {
        _policyRepository = policy;
    }

    public override string CommandName => nameof(CreateOrUpdateRoleCommand);

    protected override async Task<CreateOrUpdateRoleCommand> Handle(CreateOrUpdateRoleCommand command)
    {
        var result  = await _policyRepository.AddOrUpdate(command.Role);
        command.Result = result;

        return command;
    }
}