using BusifyAPI.Data;
using BusifyAPI.Data.Models;
using BusifyAPI.Repositories.Interfaces;
using System;
//using System.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace BusifyAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly BusifyDbContext _context;

        public UserRepository(BusifyDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<LoginUser> FindLoginUserByUsernameAsync(string username)
        {
            var loginUser = await _context.LoginUsers
                .Include(lu => lu.User)
                .FirstOrDefaultAsync(lu => lu.Username == username);

            return loginUser;
        }
    }
}
