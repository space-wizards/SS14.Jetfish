using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Security.Model;
using SS14.Jetfish.Security.Repositories;

namespace SS14.Jetfish.Security.Commands.Handlers;

public sealed class CreateTeamCommandHandler : BaseCommandHandler<CreateTeamCommand, Result<Team, Exception>>
{
    private readonly TeamRepository _repository;

    public CreateTeamCommandHandler(TeamRepository repository)
    {
        _repository = repository;
    }


    public override string CommandName => nameof(CreateTeamCommand);
    protected override async Task<CreateTeamCommand> Handle(CreateTeamCommand command)
    {
        var team = new Team
        {
            Name = command.Model.Name
        };

        var createResult = await _repository.AddOrUpdate(team);
        if (!createResult.IsSuccess || command.Model.MemberRoleName == null && command.Model.ManagerRoleName == null)
        {
            command.Result = createResult;
            return command;
        }
        
        team = createResult.Value;
        team.Roles = new List<Role>();
        Role? managerRole = null;

        if (command.Model.ManagerRoleName != null)
        {
            managerRole = new Role
            {
                DisplayName = command.Model.ManagerRoleName,
                TeamId = team.Id,
            };
            
            team.Roles.Add(managerRole);
        }

        if (command.Model.MemberRoleName != null)
        {

            var memberRole = new Role
            {
                DisplayName = command.Model.MemberRoleName,
                TeamId = team.Id
            };
            
            team.Roles.Add(memberRole);
        }

        if (command.Model.AddSelf && managerRole != null)
        {
            var member = new TeamMember
            {
                TeamId = team.Id,
                Role = managerRole,
                UserId = command.Model.UserId
            };
         
            team.TeamMembers = new List<TeamMember>();
            team.TeamMembers.Add(member);
        }
        
        command.Result = await _repository.AddOrUpdate(team);
        return command;
    }
}