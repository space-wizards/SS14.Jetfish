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

    public override async Task<User?> GetAsync(Guid id)
    {
        return await _dbContext.User
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public override Task<Result<User, Exception>> Delete(User record)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<User>> FindByName(string? search, int limit = 0, int offset = 0, CancellationToken ct = new())
    {
        var query = _dbContext.User.AsQueryable();

        if (!string.IsNullOrEmpty(search))
            query = query.Where(user => EF.Functions.ILike(user.DisplayName, $"{search}%"));

        query = query.OrderBy(user => user.DisplayName);
        
        var skipTakeQuery = query.Skip(offset);

        if (limit != 0)
            skipTakeQuery = skipTakeQuery.Take(limit);

        return await skipTakeQuery.ToListAsync(ct);
    }
}