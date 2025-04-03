using BusifyAPI.Data.Models;

namespace BusifyAPI.Repositories.Interfaces
{
    public interface IRegisterUserRepository
    {
        Task<bool> CheckEmailExistsAsync(string email);
        Task<RegistrationUser> FindByTokenAsync(short token);
        Task<RegistrationUser> FindByEmailAsync(string email);
        Task UpdateRegistrationUserAsync(RegistrationUser user);
        Task AddAsync(RegistrationUser user);
    }
}
