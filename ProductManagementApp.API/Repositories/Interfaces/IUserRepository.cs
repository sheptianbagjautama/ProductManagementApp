using ProductManagementApp.API.Models;

namespace ProductManagementApp.API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task AddAsync(User user);
        Task<bool> SaveChangesAsync();
    }
}
