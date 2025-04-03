using BusifyAPI.Configurations;
using BusifyAPI.Data.Models;
using BusifyAPI.Repositories.Interfaces;
using BusifyAPI.Services.UserServices.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BusifyAPI.Services.UserServices
{
    public class LoginUserService : ILoginUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public LoginUserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<(bool Success, string Token)> LoginAsync(string username, string password)
        {
            var loginUser = await _userRepository.FindLoginUserByUsernameAsync(username);
            
            if (loginUser != null && VerifyPasswordHash(password, loginUser.PasswordHash) && loginUser.User.IsDeleted == false)
            {
                var token = GenerateJwtToken(loginUser);
                return (true, token);
            }
            return (false, null);
        }

        private bool VerifyPasswordHash(string password, string storedHash)
        {
            var usedPassword = PasswordHasher.HashPassword(password);
            if (usedPassword == storedHash)
            {
                return true;
            }

            return false;
        }

        private string GenerateJwtToken(LoginUser user)
        {
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
