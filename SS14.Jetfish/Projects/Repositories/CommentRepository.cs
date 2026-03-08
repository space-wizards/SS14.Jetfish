using Microsoft.EntityFrameworkCore;
using SS14.Jetfish.Core.Repositories;
using SS14.Jetfish.Core.Services.Interfaces;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Database;
using SS14.Jetfish.Projects.Events;
using SS14.Jetfish.Projects.Model;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Projects.Repositories;

public class CommentRepository : BaseRepository<CardComment, Guid>
{
    private readonly ApplicationDbContext _context;
    private readonly IConcurrentEventBus _eventBus;

    public CommentRepository(ApplicationDbContext context, IConcurrentEventBus eventBus)
    {
        _context = context;
        _eventBus = eventBus;
    }

    public override async Task<Result<CardComment, Exception>> AddOrUpdate(CardComment record)
    {
        var card = await _context.Card
            .Include(card => card.Comments)
            .FirstAsync(card => card.Id == record.CardId);

        if (card.Comments.Any(c => c.Id == record.Id))
        {
            return await EditComment(record.Id, record.Content);
        }
        else
        {
            return await AddComment(record.CardId, record.Author, record.Content);
        }
    }

    public override async Task<CardComment?> GetAsync(Guid id)
    {
        return await _context.CardComment.FindAsync(id);
    }

    public override async Task<Result<CardComment, Exception>> Delete(CardComment record)
    {
        var result = await DeleteComment(record.Id);
        return result.IsSuccess
            ? Result<CardComment, Exception>.Success(record)
            : Result<CardComment, Exception>.Failure(result.Error);
    }

    public async Task<Result<CardComment, Exception>> AddComment(Guid cardId, User user, string text)
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

        var result = await SaveChanges(card, _context);
        if (!result.IsSuccess)
            return Result<CardComment, Exception>.Failure(result.Error);

        var returnValue = await _context.CardComment
            .AsNoTracking()
            .Include(c => c.Author)
            .Include(c => c.Card)
            .OrderBy(c => c.CreatedAt)
            .LastAsync(c => c.Author.Id == user.Id);

        await _eventBus.PublishAsync(card.Id,
            new CommentAddedEvent
            {
                Comment = returnValue,
            });

        return Result<CardComment, Exception>.Success(returnValue);
    }

    public async Task<Result<CardComment, Exception>> EditComment(Guid commentId, string newText)
    {
        var comment = await _context.CardComment
            .Include(cardComment => cardComment.Card)
            .FirstAsync(x => x.Id == commentId);

        comment.Content = newText;
        comment.UpdatedAt = DateTime.UtcNow;
        var result = await SaveChanges(comment, _context);
        if (!result.IsSuccess)
            return Result<CardComment, Exception>.Failure(result.Error);

        var untrackedComment = await _context.CardComment
            .AsNoTracking()
            .FirstAsync(x => x.Id == commentId);

        await _eventBus.PublishAsync(comment.CardId,
            new CommentEditedEvent
            {
                Comment = untrackedComment,
            });

        return Result<CardComment, Exception>.Success(untrackedComment);
    }

    public async Task<Result<Core.Types.Void, Exception>> DeleteComment(Guid commentId)
    {
        var comment = await _context.CardComment
            .Include(cardComment => cardComment.Card)
            .FirstAsync(x => x.Id == commentId);

        _context.CardComment.Remove(comment);
        var result = await SaveChanges(comment, _context);
        if (!result.IsSuccess)
            return Result<Core.Types.Void, Exception>.Failure(result.Error);

        await _eventBus.PublishAsync(comment.CardId,
            new CommentDeletedEvent()
            {
                CommentId = commentId,
            });

        return Result<Core.Types.Void, Exception>.Success(Core.Types.Void.Nothing);
    }
}
