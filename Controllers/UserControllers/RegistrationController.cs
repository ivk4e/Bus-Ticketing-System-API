using BusifyAPI.Dto.RegistrationDtos;
using BusifyAPI.Services.UserServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BusifyAPI.Controllers.UsersControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IRegisterUserService _registerUserService;
        private readonly IEmailService _emailService;

        public RegistrationController(IRegisterUserService registerUserService, IEmailService emailService)
        {
            _registerUserService = registerUserService;
            _emailService = emailService;
        }

        [HttpPost("register-client")]
        public async Task<IActionResult> RegisterUser([FromBody] RegistrationRequestDto registrationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _registerUserService.RegisterUser(registrationDto);

            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        [HttpGet("confirm-registration/{token}")]
        public async Task<IActionResult> ConfirmRegistration([FromRoute] string token)
        {
            var result = await _registerUserService.ConfirmRegistration(token);

            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        [HttpPost("resend-confirmation")]
        public async Task<IActionResult> ResendConfirmationEmail([FromBody] EmailRequestDto request)
        {
            var result = await _registerUserService.ResendConfirmationEmail(request.Email);

            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
    }
}
