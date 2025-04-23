using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Security.Model;
using SS14.Jetfish.Security.Repositories;

namespace SS14.Jetfish.Security.Commands.Handlers;

public class DeletePolicyCommandHandler : BaseCommandHandler<DeletePolicyCommand, Result<AccessPolicy, Exception>>
{
    private readonly PolicyRepository _policyRepository;

    public DeletePolicyCommandHandler(PolicyRepository policyRepository)
    {
        _policyRepository = policyRepository;
    }

    public override string CommandName => nameof(DeletePolicyCommand);

    protected override async Task<DeletePolicyCommand> Handle(DeletePolicyCommand command)
    {
        var result = await _policyRepository.Delete(command.Policy);
        command.Result = result;
        return command;
    }
}