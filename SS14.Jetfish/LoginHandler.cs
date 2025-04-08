using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using SS14.Jetfish.Database;
using SS14.Jetfish.Helpers;

namespace SS14.Jetfish;

public class LoginHandler
{
    private readonly ApplicationDbContext _context;

    public LoginHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task HandleTokenValidated(TokenValidatedContext ctx)
    {
        var identity = ctx.Principal?.Identities.FirstOrDefault(i => i.IsAuthenticated);

        if (identity == null)
            Debug.Fail("Unable to find identity.");

        var userId = identity.Claims.GetUserId();
        if (!userId.HasValue)
        {
            ctx.Fail("User id not present in principal");
            return;
        }

        /*if (await _context.User.AnyAsync(u => u.OpenIdUserId  == userId))
            return;

        var user = new User
        {
            OpenIdUserId = userId.Value,
            DisplayName = "New User"
        };

        await _context.User.AddAsync(user);
        await _context.SaveChangesAsync();*/
    }
}