using SS14.Jetfish.Core.Commands;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Projects.Model;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Projects.Commands;

public class CreateOrUpdateCommentCommand : BaseCommand<Result<CardComment, Exception>>
{
    public override string Name => nameof(CreateOrUpdateCommentCommand);

    public Guid CardId { get; set; }
    public Guid? CommentId { get; set; }

    public User User { get; set; }
    public string Text { get; set; }


    public CreateOrUpdateCommentCommand(Guid cardId,  string text, User user, Guid? commentId = null)
    {
        Text = text;
        User = user;
        CardId = cardId;
        CommentId = commentId;
    }
}
