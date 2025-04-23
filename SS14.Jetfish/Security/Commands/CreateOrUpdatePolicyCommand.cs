using SS14.Jetfish.Core.Commands;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Security.Commands;

public class CreateOrUpdatePolicyCommand : BaseCommand<Result<AccessPolicy, Exception>>
{
    public override string Name => nameof(CreateOrUpdatePolicyCommand);
    
    public readonly AccessPolicy Policy;

    public CreateOrUpdatePolicyCommand(AccessPolicy policy)
    {
        Policy = policy;
    }
}