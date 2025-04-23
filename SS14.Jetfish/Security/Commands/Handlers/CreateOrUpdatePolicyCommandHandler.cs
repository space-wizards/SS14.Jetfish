using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Security.Model;
using SS14.Jetfish.Security.Repositories;

namespace SS14.Jetfish.Security.Commands.Handlers;

public class CreateOrUpdatePolicyCommandHandler : BaseCommandHandler<CreateOrUpdatePolicyCommand, Result<AccessPolicy, Exception>>
{
    private readonly PolicyRepository _policyRepository;

    public CreateOrUpdatePolicyCommandHandler(PolicyRepository policyRepository)
    {
        _policyRepository = policyRepository;
    }

    public override string CommandName => nameof(CreateOrUpdatePolicyCommand);
    
    protected override async Task<CreateOrUpdatePolicyCommand> Handle(CreateOrUpdatePolicyCommand command)
    {
        var result  = await _policyRepository.AddOrUpdate(command.Policy);
        command.Result = result;
        return command;
    }
}