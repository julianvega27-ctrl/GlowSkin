using AlaiaStore.Domain.Entities;

namespace AlaiaStore.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetWithRolesByEmailAsync(string email);
    Task AssignRoleAsync(int userId, string roleName);
}
