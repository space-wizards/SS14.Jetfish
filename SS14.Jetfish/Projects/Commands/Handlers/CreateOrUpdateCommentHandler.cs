using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Projects.Model;
using SS14.Jetfish.Projects.Repositories;

namespace SS14.Jetfish.Projects.Commands.Handlers;

public class CreateOrUpdateCommentHandler : BaseCommandHandler<CreateOrUpdateCommentCommand, Result<CardComment, Exception>>
{
    private readonly CommentRepository _commentRepository;

    public CreateOrUpdateCommentHandler(CommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }


    public override string CommandName => nameof(CreateOrUpdateCommentCommand);
    protected override async Task<CreateOrUpdateCommentCommand> Handle(CreateOrUpdateCommentCommand command)
    {
        if (!command.CommentId.HasValue)
        {
            command.Result = await _commentRepository.AddComment(command.CardId, command.User, command.Text);
        }
        else
        {
            command.Result = await _commentRepository.EditComment(command.CommentId.Value, command.Text);
        }

        return command;
    }
}
