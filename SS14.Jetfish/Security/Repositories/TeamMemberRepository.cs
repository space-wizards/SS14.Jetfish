using Microsoft.EntityFrameworkCore;
using SS14.Jetfish.Core.Repositories;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Database;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Security.Repositories;

public class TeamMemberRepository : BaseRepository<TeamMember, (Guid, Guid)>
{
    private readonly ApplicationDbContext _context;

    public TeamMemberRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public override async Task<Result<TeamMember, Exception>> AddOrUpdate(TeamMember record)
    {
        var id = record.Id;
        // Checking if the team member exists in the db is necessary because the primary key is a composite key
        var exists = await Exists(record.Id);
        _context.Entry(record).State = exists ? EntityState.Modified : EntityState.Added;
        return await SaveChanges(record, _context);
    }
    
    public override async Task<TeamMember?> GetAsync((Guid, Guid) id)
    {
        return await _context.TeamMember.SingleOrDefaultAsync(t => t.TeamId == id.Item1 && t.UserId == id.Item2);
    }
    
    public async Task<bool> Exists((Guid, Guid) id)
    {
        return await _context.TeamMember
            .AsNoTracking()
            .AnyAsync(t => t.TeamId == id.Item1 && t.UserId == id.Item2);
    }

    public override async Task<Result<TeamMember, Exception>> Delete(TeamMember record)
    {
        _context.TeamMember.Remove(record);
        return await SaveChanges(record, _context);
    }
}