using BusifyAPI.Dto.LoginDtos;
using BusifyAPI.Services.UserServices.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BusifyAPI.Controllers.UsersControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginUserService _loginUserService;

        public LoginController(ILoginUserService loginUserService)
        {
            _loginUserService = loginUserService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Username and password are required.");
            }

            var (Success, Token) = await _loginUserService.LoginAsync(request.Username, request.Password);
            if (Success)
            {
                return Ok(new { Token });
            }
            else
            {
                return Unauthorized("Invalid username or password.");
            }
        }
    }
}
