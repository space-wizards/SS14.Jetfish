using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Projects.Model;
using SS14.Jetfish.Projects.Repositories;

namespace SS14.Jetfish.Projects.Commands.Handlers;

public class DeleteCommentCommandHandler : BaseCommandHandler<DeleteCommentCommand, Result<CardComment, Exception>>
{
    private readonly CommentRepository _commentRepository;

    public DeleteCommentCommandHandler(CommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public override string CommandName => nameof(DeleteCommentCommand);
    protected override async Task<DeleteCommentCommand> Handle(DeleteCommentCommand command)
    {
        command.Result = await _commentRepository.DeleteComment(command.CommentId);
        return command;
    }
}
