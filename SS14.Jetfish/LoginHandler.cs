using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using SS14.Jetfish.Configuration;
using SS14.Jetfish.Database;
using SS14.Jetfish.Database.Model;
using SS14.Jetfish.Helpers;

namespace SS14.Jetfish;

public class LoginHandler
{
    private readonly ApplicationDbContext _context;
    private readonly ServerConfiguration _configuration = new();

    public LoginHandler(IConfiguration configuration, ApplicationDbContext context)
    {
        _context = context;
        configuration.Bind(ServerConfiguration.Name, _configuration);
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

        if (CheckRequiredClaim(identity))
        {
            ctx.Fail("User doesn't have required claim");
            return;
        }
        
        if (await _context.User.AnyAsync(u => u.Id  == userId))
            return;

        var user = new User
        {
            Id = userId.Value,
            DisplayName = "New User"
        };

        await _context.User.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Checks if the given idenitity contains the configured required claim
    /// </summary>
    /// <returns>true when the identity is missing the required claim</returns>
    private bool CheckRequiredClaim(ClaimsIdentity identity)
    {
        if (_configuration.RequiredClaim == null)
            return false;

        var claim = identity.Claims.FirstOrDefault(c => c.Type == _configuration.RequiredClaim)?.Value;

        if (_configuration.RequiredClaimValues == null)
            return claim == null;

        return claim == null || !_configuration.RequiredClaim.Contains(claim);
    }

    public async Task HandleUserDataUpdate(UserInformationReceivedContext ctx)
    {
        var name = ctx.User.RootElement.GetProperty("name").GetString();
        if (name == null)
            return;

        var identity = ctx.Principal?.Identities.FirstOrDefault(i => i.IsAuthenticated);
        if (identity == null)
            return;
        
        var userId = identity.Claims.GetUserId();
        var user = await _context.User.SingleOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return;

        user.DisplayName = name;
        _context.Update(user);
        await _context.SaveChangesAsync();
    }
}