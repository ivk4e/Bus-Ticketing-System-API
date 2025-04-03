using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BusifyAPI.Data.Models;
using BusifyAPI.Dto.RegistrationDtos;
using BusifyAPI.Repositories.Interfaces;
using BusifyAPI.Services.UserServices.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using BusifyAPI.Configurations;

namespace BusifyAPI.Services.UserServices
{
    public class RegisterUserService : IRegisterUserService
    {
        private readonly IRegisterUserRepository _registerUserRepository;
        private readonly IEmailService _emailService;
        private readonly ILoginRepository _loginRepository;
        private readonly IUserRepository _userRepository;
        private readonly string _jwtSecret;

        public RegisterUserService(
            IRegisterUserRepository registerUserRepository,
            ILoginRepository loginRepository,
            IUserRepository userRepository,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _registerUserRepository = registerUserRepository;
            _loginRepository = loginRepository;
            _userRepository = userRepository;
            _emailService = emailService;

            _jwtSecret = configuration["JwtSettings:Secret"];
        }

        public async Task<RegistrationResult> RegisterUser(RegistrationRequestDto registrationDto)
        {
            if (await _registerUserRepository.CheckEmailExistsAsync(registrationDto.Email))
            {
                return new RegistrationResult { Success = false, Message = "Email already registered." };
            }

            if (!Regex.IsMatch(registrationDto.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase))
            {
                return new RegistrationResult { Success = false, Message = "Invalid email format." };
            }

            if (registrationDto.Password != registrationDto.RepeatPassword)
            {
                return new RegistrationResult { Success = false, Message = "Passwords do not match." };
            }

            if (!Regex.IsMatch(registrationDto.Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{6,}$"))
            {
                return new RegistrationResult { Success = false, Message = "Password must be at least 6 characters long and include a number, an uppercase letter, a lowercase letter, and a special character." };
            }

            if (!Regex.IsMatch(registrationDto.FirstName, @"^[a-zA-Z]+$") || !Regex.IsMatch(registrationDto.LastName, @"^[a-zA-Z]+$"))
            {
                return new RegistrationResult { Success = false, Message = "Names must contain only letters." };
            }

            if (registrationDto.Username.Length > 20 || !Regex.IsMatch(registrationDto.Username, @"^\w+$"))
            {
                return new RegistrationResult { Success = false, Message = "Username must be up to 20 alphanumeric characters or underscores." };
            }

            var registrationUser = new RegistrationUser
            {
                Email = registrationDto.Email,
                FirstName = registrationDto.FirstName,
                LastName = registrationDto.LastName,
                Birthday = registrationDto.Birthday,
                Username = registrationDto.Username,
                PasswordHash = PasswordHasher.HashPassword(registrationDto.Password),
                IsEmailConfirmed = false
            };

            await _registerUserRepository.AddAsync(registrationUser);

            var confirmationLink = CreateConfirmationLink(registrationUser);
            await _emailService.SendConfirmationEmailAsync(registrationDto.Email, "Confirm your email", confirmationLink);

            return new RegistrationResult { Success = true, Message = "Registration successful. Please confirm your email." };
        }

        public async Task<RegistrationResult> ConfirmRegistration(string token)
        {
            var principal = GetPrincipalFromToken(token);
            if (principal == null)
            {
                return new RegistrationResult { Success = false, Message = "Invalid token." };
            }

            var email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var registrationUser = await _registerUserRepository.FindByEmailAsync(email);
            if (registrationUser == null)
            {
                return new RegistrationResult { Success = false, Message = "User not found." };
            }

            if (registrationUser.IsEmailConfirmed)
            {
                return new RegistrationResult { Success = false, Message = "Email has already been confirmed." };
            }

            var user = new User
            {
                RoleId = 5,
                Name = $"{registrationUser.FirstName} {registrationUser.LastName}",
                Email = registrationUser.Email,
                Birthday = registrationUser.Birthday,
                RegistrationDate = DateTime.UtcNow
            };
            await _userRepository.AddAsync(user);

            var createdUser = await _userRepository.FindByEmailAsync(user.Email);

            var loginUser = new LoginUser
            {
                UserId = createdUser.UserId,
                Username = registrationUser.Username,
                PasswordHash = registrationUser.PasswordHash
            };

            await _loginRepository.AddAsync(loginUser);

            registrationUser.IsEmailConfirmed = true;
            await _registerUserRepository.UpdateRegistrationUserAsync(registrationUser);

            return new RegistrationResult { Success = true, Message = "Registration confirmed successfully." };
        }

        public async Task<RegistrationResult> ResendConfirmationEmail(string email)
        {
            var registrationUser = await _registerUserRepository.FindByEmailAsync(email);
            if (registrationUser == null)
            {
                return new RegistrationResult { Success = false, Message = "Email not registered." };
            }

            if (registrationUser.IsEmailConfirmed)
            {
                return new RegistrationResult { Success = false, Message = "Email already confirmed." };
            }

            var confirmationLink = CreateConfirmationLink(registrationUser);
            await _emailService.SendConfirmationEmailAsync(email, "Confirm your email", confirmationLink);

            return new RegistrationResult { Success = true, Message = "Confirmation email resent. Please check your email." };
        }

        private string CreateConfirmationLink(RegistrationUser user)
        {
            var token = GenerateTokenForUser(user);
            return $"http://localhost:5097/api/Registration/confirm-registration/{token}";
        }

        private string GenerateTokenForUser(RegistrationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);
            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out _);

                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
