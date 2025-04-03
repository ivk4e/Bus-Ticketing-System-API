using BusifyAPI.Data;
using BusifyAPI.Data.Models;
using BusifyAPI.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Data;
//using System.Data.Entity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using BusifyAPI.Services.AuthServices;
using BusifyAPI.Services.AuthServices.Interfaces;

namespace BusifyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authenticationService;

        public AuthController(IAuthService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpGet("token")]
        public IActionResult GetToken()
        {
            var apiKey = Request.Headers["X-ApiKey"].FirstOrDefault();
            var apiSecret = Request.Headers["X-ApiSecret"].FirstOrDefault();
            Console.WriteLine($"Received API Key: {apiKey}, API Secret: {apiSecret}");

            if (!_authenticationService.ValidateApiKeyAndSecret(apiKey, apiSecret))
            {
                return Unauthorized("Invalid API Key or Secret");
            }

            var token = _authenticationService.Authenticate(apiKey, apiSecret);
            return Ok(new { Token = token });
        }
    }
}