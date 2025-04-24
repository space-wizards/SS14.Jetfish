using Microsoft.EntityFrameworkCore;
using SS14.Jetfish.Core.Repositories;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Database;
using SS14.Jetfish.FileHosting.Model;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.FileHosting.Repositories;

public class FileRepository : BaseRepository<UploadedFile, Guid>, IResourceRepository<UploadedFile, Guid>
{
    private readonly ApplicationDbContext _context;

    public FileRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public override Task<Result<UploadedFile, Exception>> AddOrUpdate(UploadedFile record)
    {
        throw new NotImplementedException();
    }

    public override async Task<UploadedFile?> GetAsync(Guid id)
    {
        return await _context.UploadedFile.Where(file => file.Id == id).SingleOrDefaultAsync();
    }

    public override Task<Result<UploadedFile, Exception>> Delete(UploadedFile record)
    {
        throw new NotImplementedException();
    }

    public Task<ICollection<UploadedFile>> ListByPolicy(Guid userId, Permission policy, int? limit = null, int? offset = null)
    {
        throw new NotImplementedException();
    }

    public async Task<ICollection<UploadedFile>> GetMultiple(IEnumerable<Guid> ids)
    {
        return await _context.UploadedFile.Where(x => ids.Contains(x.Id)).ToListAsync();
    }
}