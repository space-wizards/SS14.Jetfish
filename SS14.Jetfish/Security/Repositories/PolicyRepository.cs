using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SS14.Jetfish.Core.Repositories;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Database;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Security.Repositories;

public class PolicyRepository : IRepository<Role, Guid>
{
    public const string ConcurrencyError = "Concurrency version error";

    private readonly ApplicationDbContext _context;

    public PolicyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Result<Role, Exception> AddOrUpdate(Role record)
    {
        _context.Entry(record).State = record.Id != Guid.Empty ?
            EntityState.Modified : EntityState.Added;

        try
        {
            _context.SaveChanges();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            if (pgEx.MessageText != ConcurrencyError)
                throw;

            return Result<Role, Exception>.Failure(pgEx);
        }

        return Result<Role, Exception>.Success(record);
    }

    public bool TryGet(Guid id, [NotNullWhen(true)] out Role? result)
    {
        throw new NotImplementedException();
    }

    public Task<Role?> GetAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Role> GetAllGlobal()
    {
        return _context.Role
            .Include(role => role.Policies)
            .ThenInclude(resourcePolicy => resourcePolicy.AccessPolicy)
            .Where(x => x.Policies.Count == 0 || x.Policies.Any(y => y.ResourceId == null || y.Global))
            .ToList();
    }
}