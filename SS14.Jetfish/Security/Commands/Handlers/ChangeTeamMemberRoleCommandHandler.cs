using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Database;
using SS14.Jetfish.Security.Model;
using SS14.Jetfish.Security.Repositories;

namespace SS14.Jetfish.Security.Commands.Handlers;

public class ChangeTeamMemberRoleCommandHandler : BaseCommandHandler<ChangeTeamMemberRoleCommand, Result<TeamMember, Exception>>
{
    private readonly TeamMemberRepository _teamMemberRepository;
    private readonly ApplicationDbContext _context;

    public ChangeTeamMemberRoleCommandHandler(TeamMemberRepository teamMemberRepository, ApplicationDbContext context)
    {
        _teamMemberRepository = teamMemberRepository;
        _context = context;
    }

    public override string CommandName => nameof(ChangeTeamMemberRoleCommand);
    protected override async Task<ChangeTeamMemberRoleCommand> Handle(ChangeTeamMemberRoleCommand command)
    {
        _context.ChangeTracker.Clear();
        var member = await _teamMemberRepository.GetAsync((command.TeamId, command.UserId));
        if (member == null)
        {
            command.Result = Result<TeamMember, Exception>.Failure(new ArgumentException("Team member not found"));
            return command;
        }
        
        member.RoleId = command.RoleId;
        command.Result = await _teamMemberRepository.AddOrUpdate(member);
        return command;
    }
}