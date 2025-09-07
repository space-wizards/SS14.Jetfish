using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SS14.Jetfish.Core.Repositories;
using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Services.Interfaces;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Database;
using SS14.Jetfish.Helpers;
using SS14.Jetfish.Projects.Events;
using SS14.Jetfish.Projects.Model;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Projects.Repositories;

public class ProjectRepository : BaseRepository<Project, Guid>, IResourceRepository<Project, Guid>
{
    private readonly ApplicationDbContext _context;
    private readonly IConcurrentEventBus _eventBus;

    public ProjectRepository(ApplicationDbContext context, IConcurrentEventBus eventBus)
    {
        _context = context;
        _eventBus = eventBus;
    }

    public override async Task<Result<Project, Exception>> AddOrUpdate(Project record)
    {
        _context.Entry(record).State = record.Id != Guid.Empty ?
            EntityState.Modified : EntityState.Added;

        return await SaveChanges(record, _context);
    }

    public override async Task<Project?> GetAsync(Guid id)
    {
        return await _context.Project.FindAsync(id);
    }

    public override async Task<Result<Project, Exception>> Delete(Project record)
    {
        await _context.FileUsage.Where(usage => usage.ProjectId == record.Id).ExecuteDeleteAsync();

        _context.Project.Remove(record);
        return await SaveChanges(record, _context);
    }

    public async Task<ICollection<Project>> Search(Guid? teamId, string? search, int? limit = null, int? offset = null, CancellationToken ct = default)
    {
        IQueryable<Project> query;

        // Rider doesn't like this being a ternary when using efcore functions later on
        if (teamId != null)
        {
            query = _context.Team.Where(x => x.Id == teamId).SelectMany(x => x.Projects);
        }
        else
        {
            query = _context.Project.AsQueryable();
        }

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => EF.Functions.ILike(x.Name, $"{search}%"));

        if (offset.HasValue)
            query = query.Skip(offset.Value);

        if (limit.HasValue)
            query = query.Take(limit.Value);

        return await query.OrderBy(x => x.Id).ToListAsync(ct);
    }

    public async Task<int> CountByPolicyAndTeam(Guid userId, Guid teamId, Permission policy)
    {
        var teamQuery = PolicyTeamQuery(userId, policy)
            .Where(project => _context.Team
                .Where(team => team.Id == teamId)
                .Any(team => team.Projects.Any(x => x.Id == project.Id)));

        var userQuery = PolicyUserQuery(userId, policy)
            .Where(project => _context.Team
                .Where(team => team.Id == teamId)
                .Any(team => team.Projects.Any(x => x.Id == project.Id)));

        var query = userQuery.Union(teamQuery);
        return await query.CountAsync();
    }

    public async Task<ICollection<Project>> ListByPolicyAndTeam(
        Guid userId,
        Guid teamId,
        Permission policy,
        int? limit = null,
        int? offset = null)
    {
        var teamQuery = PolicyTeamQuery(userId, policy)
            .Where(project => _context.Team
                .Where(team => team.Id == teamId)
                .Any(team => team.Projects.Any(x => x.Id == project.Id)));

        var userQuery = PolicyUserQuery(userId, policy)
            .Where(project => _context.Team
                .Where(team => team.Id == teamId)
                .Any(team => team.Projects.Any(x => x.Id == project.Id)));

        var query = userQuery.Union(teamQuery);

        if (offset.HasValue)
            query = query.Skip(offset.Value);

        if (limit.HasValue)
            query = query.Take(limit.Value);

        return await query.OrderBy(x => x.Id).ToListAsync();
    }

    public async Task<ICollection<Project>> ListByPolicy(ClaimsPrincipal user, Permission policy, int? limit = null, int? offset = null)
    {
        var userId = user.Claims.GetUserId()!.Value;

        var teamQuery = PolicyTeamQuery(userId, policy);
        var userQuery = PolicyUserQuery(userId, policy);

        var query = userQuery.Union(teamQuery);

        if (offset.HasValue)
            query = query.Skip(offset.Value);

        if (limit.HasValue)
            query = query.Take(limit.Value);

        return await query.OrderBy(x => x.Id).ToListAsync();
    }

    private IQueryable<Project> PolicyUserQuery(Guid userId, Permission policy)
    {
        var userQuery = _context.Project.Where(project => _context.User
            .Where(user => user.Id == userId)
            .Any(user => user.ResourcePolicies
                .Where(resourcePolicy => resourcePolicy.ResourceId == project.Id || resourcePolicy.Global)
                .Any(resourcePolicy => resourcePolicy.AccessPolicy.Permissions.Contains(policy))));
        return userQuery;
    }

    private IQueryable<Project> PolicyTeamQuery(Guid userId, Permission policy)
    {
        var teamQuery = _context.Project.Where(project => _context.TeamMember
            .Include(member => member.Role)
            .Where(member => member.User.Id == userId).Any(member => member.Role.Policies
                .Where(resourcePolicy => resourcePolicy.ResourceId == project.Id || resourcePolicy.Global)
                .Any(resourcePolicy => resourcePolicy.AccessPolicy.Permissions.Contains(policy))));
        return teamQuery;
    }

    public async Task<ICollection<Project>> GetMultiple(IEnumerable<Guid> ids)
    {
        return await _context.Project.Where(x => ids.Contains(x.Id)).ToListAsync();
    }

    /// <summary>
    /// Adds a new lane to a Project and returns it.
    /// </summary>
    public async Task<Lane> AddLane(Guid projectId, string name)
    {
        if (_context.List.Any(list => list.ProjectId == projectId && list.Title == name))
            throw new InvalidOperationException("List with the same name already exists");

        var project = await _context.Project
            .AsNoTracking()
            .Include(project => project.Lists)
            .FirstAsync(x => x.Id == projectId);

        var order = 1f;

        if (project.Lists.Count != 0)
            order = project.Lists.OrderBy(x => x.Order).Last().Order + 1;

        var listToAdd = new Lane()
        {
            Title = name,
            ProjectId = projectId,
            Order = order,
            Cards = [],
            ListId = await GetListId(projectId),
        };

        _context.List.Add(listToAdd);
        await _context.SaveChangesAsync();

        var returnList = await _context.List
            .AsNoTracking()
            .FirstAsync(x => x.ProjectId == projectId && x.ListId == listToAdd.ListId);

        await _eventBus.PublishAsync(projectId,
            new LaneCreatedEvent()
            {
                Title = returnList.Title,
                ListId = returnList.ListId,
            });

        return returnList;
    }

    /// <summary>
    /// Gets an available listId to use.
    /// </summary>
    private async Task<int> GetListId(Guid projectId)
    {
        var maxListId = await _context.List
            .Where(l => l.ProjectId == projectId)
            .MaxAsync(x => (int?)x.ListId);

        return maxListId + 1 ?? 1;
    }

    /// <summary>
    /// Adds a card... Yep.
    /// </summary>
    public async Task<Card> AddCard(Guid projectId, int laneId, string name, Guid userId)
    {
        var lane = await _context.List
            .Include(list => list.Cards)
            .FirstAsync(list => list.ProjectId == projectId && list.ListId == laneId);

        var order = 1f;
        if (lane.Cards.Count != 0)
            order = lane.Cards.OrderBy(x => x.Order).Last().Order + 1;

        var card = new Card()
        {
            Title = name,
            Order = order,
            ProjectId = projectId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ListId = laneId,
            AuthorId = userId,
        };
        lane.Cards.Add(card);
        await _context.SaveChangesAsync();

        var result = _context.Card
            .AsNoTracking() // Dont wanna change it on accident.
            .Include(x => x.Lane)
            .First(x => x.Id == card.Id);

        await _eventBus.PublishAsync(projectId, new CardCreatedEvent()
        {
            Card = result,
        });

        return result;
    }

    /// <summary>
    /// Updates a cards position
    /// </summary>
    public async Task UpdateCardPosition(Guid projectId, string laneName, Guid userId, Guid cardId, int zoneIndex)
    {
        _context.ChangeTracker.Clear();

        var card = await _context.Card
            .FirstAsync(x => x.Id == cardId);

        var targetLane = await _context.List
            .AsNoTracking()
            .FirstAsync(x => x.Title == laneName && x.ProjectId == projectId);

        var cardsInLane = await _context.Card
            .Where(c => c.ListId == targetLane.ListId && c.Id != cardId)
            .OrderBy(c => c.Order)
            .ToListAsync();

        zoneIndex = Math.Max(0, Math.Min(zoneIndex, cardsInLane.Count));

        cardsInLane.Insert(zoneIndex, card);

        // reorder all cards
        for (var i = 0; i < cardsInLane.Count; i++)
        {
            cardsInLane[i].Order = i;
        }

        var previousList = card.ListId;

        card.ListId = targetLane.ListId;
        card.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();


        var updatedCard = await _context.Card
            .AsNoTracking()
            .Include(x => x.Lane)
            .FirstAsync(x => x.Id == cardId);

        var orders = await _context.Card
            .AsNoTracking()
            .Where(c => c.ListId == updatedCard.ListId && c.ProjectId == updatedCard.ProjectId)
            .Select(x => new
            {
                x.Id,
                x.Order,
            })
            .ToDictionaryAsync(x => x.Id, x => x.Order);

        var @event = new CardMovedEvent()
        {
            CardId = updatedCard.Id,
            NewListId = updatedCard.ListId,
            OldListId = previousList,
            Orders = orders,
        };
        await _eventBus.PublishAsync(cardId, @event);
        await _eventBus.PublishAsync(projectId, @event);
    }

    /// <summary>
    /// Gets a card including the lane its on and its author
    /// </summary>
    public async Task<Card?> GetCard(Guid cardId)
    {
        return await _context.Card
            .AsNoTracking()
            .Include(x => x.Author)
            .Include(x => x.Lane)
            .Include(x => x.Comments)
            .ThenInclude(x => x.Author)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == cardId);
    }

    /// <summary>
    /// Deletes a section from a project. Also deletes all cards in the section.
    /// </summary>
    public async Task DeleteLane(Guid projectId, int backingLaneId)
    {
        var lane = await _context.List
            .Include(list => list.Cards)
            .Where(x => x.ProjectId == projectId && x.ListId == backingLaneId)
            .FirstAsync();

        _context.List.Remove(lane);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        await _eventBus.PublishAsync(projectId,
            new LaneRemovedEvent()
            {
                ListId = lane.ListId,
            });
    }

    public async Task SetLaneName(Guid projectId, int laneId, string newTitle)
    {
        var lane = await _context.List
            .FirstAsync(list => list.ProjectId == projectId && list.ListId == laneId);

        lane.Title = newTitle;
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        await _eventBus.PublishAsync(projectId,
            new LaneUpdatedEvent()
            {
                Title = newTitle,
                LaneId = laneId,
            });
    }

    public async Task<List<Lane>> GetLanes(Guid projectId)
    {
        return await _context.List
            .AsNoTracking()
            .Where(list => list.ProjectId == projectId)
            .OrderBy(list => list.Order)
            .ToListAsync();
    }

    public async Task<Core.Types.Void> UpdateCardLite(Card toUpdate)
    {
        var card = await _context.Card
            .FirstAsync(x => x.Id == toUpdate.Id);

        // this sucks and i dont care
        card.Title = toUpdate.Title;
        card.Description = toUpdate.Description;
        card.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        var @event = new CardUpdatedEvent
        {
            CardId = card.Id,
            Title = card.Title,
            Description = card.Description,
            UpdatedAt = card.UpdatedAt,
        };

        await _eventBus.PublishAsync(toUpdate.Id, @event);
        await _eventBus.PublishAsync(toUpdate.ProjectId, @event);

        return Core.Types.Void.Nothing;
    }

    public async Task<Core.Types.Void> AddComment(Guid cardId, User user, string text)
    {
        var card = await _context.Card
            .FirstAsync(card => card.Id == cardId);

        card.Comments.Add(new CardComment()
        {
            Content = text,
            CreatedAt = DateTime.UtcNow,
            Author = user,
            CardId = cardId,
        });

        await _context.SaveChangesAsync();

        var returnValue = await _context.CardComment
            .AsNoTracking()
            .Include(c => c.Author)
            .OrderBy(c => c.CreatedAt)
            .LastAsync(c => c.Author.Id == user.Id);

        await _eventBus.PublishAsync(card.Id,
            new CommentAddedEvent
            {
                Comment = returnValue,
            });

        return Core.Types.Void.Nothing;
    }

    public async Task<Core.Types.Void> EditComment(Guid commentId, string newText)
    {
        var comment = await _context.CardComment
            .Include(cardComment => cardComment.Card)
            .FirstAsync(x => x.Id == commentId);

        comment.Content = newText;
        comment.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var untrackedComment = await _context.CardComment
            .AsNoTracking()
            .FirstAsync(x => x.Id == commentId);

        await _eventBus.PublishAsync(comment.CardId,
            new CommentEditedEvent
            {
                Comment = untrackedComment,
            });

        return Core.Types.Void.Nothing;
    }

    public async Task<Core.Types.Void> DeleteComment(Guid commentId)
    {
        var comment = await _context.CardComment
            .Include(cardComment => cardComment.Card)
            .FirstAsync(x => x.Id == commentId);

        _context.CardComment.Remove(comment);
        await _context.SaveChangesAsync();

        await _eventBus.PublishAsync(comment.CardId,
            new CommentDeletedEvent()
            {
                CommentId = commentId,
            });

        return Core.Types.Void.Nothing;
    }
}
