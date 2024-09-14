﻿using MediatR;
using TokenManager.Common.Models;
using TokenManager.Domain.Entities;

namespace TokenManager.Application.Queries
{
    public record GetAllUsersQuery(string Tenant) : IRequest<Result<IEnumerable<User>>>;
}
