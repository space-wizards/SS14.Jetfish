using SS14.Jetfish.Core.Commands;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Projects.Model;

namespace SS14.Jetfish.Projects.Commands;

public class DeleteCommentCommand : BaseCommand<Result<CardComment, Exception>>
{
    public override string Name => nameof(DeleteCommentCommand);

    public Guid CommentId { get; set; }

    public DeleteCommentCommand(Guid commentId)
    {
        CommentId = commentId;
    }
}
