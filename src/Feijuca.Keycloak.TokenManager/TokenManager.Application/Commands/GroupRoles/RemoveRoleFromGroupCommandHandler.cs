﻿using Common.Errors;
using Common.Models;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.GroupRoles;

public class RemoveRoleFromGroupCommandHandler(IGroupRepository groupRepository, IGroupRolesRepository roleGroupRepository, IRoleRepository roleRepository) : IRequestHandler<RemoveRoleFromGroupCommand, Result<bool>>
{
    private readonly IGroupRepository _groupRepository = groupRepository;
    private readonly IGroupRolesRepository _roleGroupRepository = roleGroupRepository;
    private readonly IRoleRepository _roleRepository = roleRepository;

    public async Task<Result<bool>> Handle(RemoveRoleFromGroupCommand command, CancellationToken cancellationToken)
    {
        var groupsResult = await _groupRepository.GetAllAsync(command.Tenant);
        if (groupsResult.IsSuccess && groupsResult.Response.Any(x => x.Id == command.GroupId))
        {
            var rolesResult = await _roleRepository.GetRolesForClientAsync(command.Tenant, command.RemoveRoleFromGroupRequest.ClientId);
            var existingRule = rolesResult.Response.FirstOrDefault(x => x.Id == command.RemoveRoleFromGroupRequest.RoleId);
            if (rolesResult.IsSuccess && existingRule != null)
            {
                await _roleGroupRepository.RemoveRoleFromGroupAsync(command.Tenant,
                    command.RemoveRoleFromGroupRequest.ClientId,
                    command.GroupId,
                    existingRule.Id,
                    existingRule.Name);

                return Result<bool>.Success(true);
            }
        }

        return Result<bool>.Failure(GroupRolesErrors.RemovingRoleFromGroupError);
    }
}
