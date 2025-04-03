using BusifyAPI.Data.Models;

namespace BusifyAPI.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task AddAsync(User user);
        Task<User> FindByEmailAsync(string email);
        Task<LoginUser> FindLoginUserByUsernameAsync(string username);
    }
}
