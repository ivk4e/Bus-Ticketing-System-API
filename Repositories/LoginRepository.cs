using BusifyAPI.Data;
using BusifyAPI.Data.Models;
using BusifyAPI.Repositories.Interfaces;

namespace BusifyAPI.Repositories
{
    public class LoginRepository : ILoginRepository
    {
        private readonly BusifyDbContext _context;

        public LoginRepository(BusifyDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(LoginUser loginUser)
        {
            await _context.LoginUsers.AddAsync(loginUser);
            await _context.SaveChangesAsync();
        }
    }
}
