using BusifyAPI.Services.AuthServices.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace BusifyAPI.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Authenticate(string apiKey, string apiSecret)
        {
            var validApiKey = _configuration["ApiAuthentication:ApiKey"];
            var validApiSecret = _configuration["ApiAuthentication:ApiSecret"];

            if (apiKey == validApiKey && apiSecret == validApiSecret)
            {
                return GenerateJwtToken(apiKey);
            }

            return null;
        }

        public bool ValidateApiKeyAndSecret(string apiKey, string apiSecret)
        {
            var validApiKey = _configuration["ApiAuthentication:ApiKey"];
            var validApiSecret = _configuration["ApiAuthentication:ApiSecret"];
            bool isValid = apiKey == validApiKey && apiSecret == validApiSecret;

            return isValid;
        }

        private string GenerateJwtToken(string apiKey)
        {
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSecret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Name, apiKey)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
