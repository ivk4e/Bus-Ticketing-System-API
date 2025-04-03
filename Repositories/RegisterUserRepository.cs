using BusifyAPI.Data;
using BusifyAPI.Data.Models;
using BusifyAPI.Repositories.Interfaces;
//using System.Data.Entity; -> this is causing some problems
using Microsoft.EntityFrameworkCore;

namespace BusifyAPI.Repositories
{
    public class RegisterUserRepository : IRegisterUserRepository
    {
        private readonly BusifyDbContext _context;

        public RegisterUserRepository(BusifyDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            return await _context.RegistrationUsers.AnyAsync(u => u.Email == email);
        }

        public async Task AddAsync(RegistrationUser user)
        {
            await _context.RegistrationUsers.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<RegistrationUser> FindByTokenAsync(short token)
        {
            return await _context.RegistrationUsers.FirstOrDefaultAsync(u => u.EmailConfirmationCode == token);
        }
        public async Task<RegistrationUser> FindByEmailAsync(string email)
        {
            return await _context.RegistrationUsers.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task UpdateRegistrationUserAsync(RegistrationUser user)
        {
            _context.RegistrationUsers.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
