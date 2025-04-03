using BusifyAPI.Data.Models;

namespace BusifyAPI.Repositories.Interfaces
{
    public interface ILoginRepository
    {
        Task AddAsync(LoginUser loginUser);
    }
}
