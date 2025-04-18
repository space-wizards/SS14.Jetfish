using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Security.Model;
using SS14.Jetfish.Security.Repositories;

namespace SS14.Jetfish.Security.Commands;

public class DeleteRoleCommandHandler : BaseCommandHandler<DeleteRoleCommand, Result<Role, Exception>>
{
    private readonly PolicyRepository _policyRepository;

    public DeleteRoleCommandHandler(PolicyRepository policyRepository)
    {
        _policyRepository = policyRepository;
    }

    public override string CommandName => nameof(DeleteRoleCommand);

    protected override async Task<DeleteRoleCommand> Handle(DeleteRoleCommand command)
    {
        var result  = await _policyRepository.Delete(command.Role);
        command.Result = result;

        return command;
    }
}