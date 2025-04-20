using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using SS14.Jetfish.Core.Repositories;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Database;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Security.Repositories;

public class UserRepository : BaseRepository<User, Guid>
{
    private readonly ApplicationDbContext _dbContext;

    public UserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override Task<Result<User, Exception>> AddOrUpdate(User record)
    {
        throw new NotImplementedException();
    }

    public override bool TryGet(Guid id, [NotNullWhen(true)] out User? result)
    {
        throw new NotImplementedException();
    }

    public override async Task<User?> GetAsync(Guid id)
    {
        return await _dbContext.User
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public override Task<Result<User, Exception>> Delete(User record)
    {
        throw new NotImplementedException();
    }
}