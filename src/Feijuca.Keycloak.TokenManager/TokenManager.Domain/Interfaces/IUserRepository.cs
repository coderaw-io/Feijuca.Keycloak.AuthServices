﻿using TokenManager.Common.Models;
using TokenManager.Domain.Entities;

namespace TokenManager.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<Result<IEnumerable<User>>> GetAllAsync(string tenant);
        Task<Result<bool>> DeleteAsync(string tenant, Guid id);
        Task<Result<bool>> CreateAsync(string tenant, User user);
        Task<Result<User>> GetAsync(string tenant, string userName);
        Task<Result<bool>> ResetPasswordAsync(string tenant, string userId, string password);
        Task<Result> SendEmailVerificationAsync(string tenant, string userId);
    }
}
