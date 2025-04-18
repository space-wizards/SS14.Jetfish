﻿using SS14.Jetfish.Core.Services;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Database;
using SS14.Jetfish.Security.Model;
using SS14.Jetfish.Security.Repositories;

namespace SS14.Jetfish.Security.Commands;

public class RoleCommandHandler : BaseCommandHandler<CreateOrUpdateRole, Result<Role, Exception>>
{
    private readonly PolicyRepository _policyRepository;

    public RoleCommandHandler(PolicyRepository policy)
    {
        _policyRepository = policy;
    }

    public override string CommandName => nameof(CreateOrUpdateRole);

    protected override async Task<CreateOrUpdateRole> Handle(CreateOrUpdateRole command)
    {
        var result  = await _policyRepository.AddOrUpdate(command.Role);
        command.Result = result;

        return command;
    }
}