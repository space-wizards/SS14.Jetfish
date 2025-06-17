using SS14.Jetfish.Core.Extensions;
using SS14.Jetfish.Helpers;
using SS14.Jetfish.Security.Repositories;

namespace SS14.Jetfish.Security;

public class PermissionsMiddleware : IMiddleware
{
    private readonly PolicyRepository _repository;

    public PermissionsMiddleware(PolicyRepository repository)
    {
        _repository = repository;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.User.Identity is not { IsAuthenticated: true })
        {
            await next(context);
            return;
        }

        var ct =  context.RequestAborted;
        var userId = context.User.Claims.GetUserId();

        if (!userId.HasValue)
        {
            await context.WriteAccessDeniedResponse("User id not present in token", ct);
            return;
        }

        // TODO: Get user resource permissions fro policy repository

        var permissions = await _repository.GetIdentityPermissions(userId.Value);

        await next(context);
    }
}
