using SS14.Jetfish.Core.Commands;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Security.Commands;

public class DeletePolicyCommand : BaseCommand<Result<AccessPolicy, Exception>>
{
    public override string Name => nameof(DeletePolicyCommand);
    
    public readonly AccessPolicy Policy;
    
    public DeletePolicyCommand(AccessPolicy policy)
    {
        Policy = policy;
    }
}