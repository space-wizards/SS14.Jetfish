using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Database;
using SS14.Jetfish.Security.Model;
using SS14.Jetfish.Security.Repositories;

namespace SS14.Jetfish.Security.Commands.Handlers;

public class AddTeamMemberCommandHandler : BaseCommandHandler<AddTeamMemberCommand, Result<TeamMember, Exception>>
{
    private readonly TeamMemberRepository _teamMemberRepository;
    private readonly ApplicationDbContext _context;

    public AddTeamMemberCommandHandler(TeamMemberRepository teamMemberRepository, ApplicationDbContext context)
    {
        _teamMemberRepository = teamMemberRepository;
        _context = context;
    }

    public override string CommandName => nameof(AddTeamMemberCommand);
    protected override async Task<AddTeamMemberCommand> Handle(AddTeamMemberCommand command)
    {
        if (!command.Model.RoleId.HasValue)
        {
            command.Result = Result<TeamMember, Exception>.Failure(new UiException("Role id is required", false));
            return command;
        }

        var memberExists = await _teamMemberRepository.Exists((command.Model.TeamId, command.Model.UserId));
        if (memberExists)
        {
            command.Result = Result<TeamMember, Exception>.Failure(new UiException("User is already a member of the team", false));
            return command;
        }
        
        var member = new TeamMember
        {
            TeamId = command.Model.TeamId,
            UserId = command.Model.UserId,
            RoleId = command.Model.RoleId.Value
        };

        command.Result = await _teamMemberRepository.AddOrUpdate(member);
        return command;
    }
}