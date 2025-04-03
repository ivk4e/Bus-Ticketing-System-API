using BusifyAPI.Services.UserServices.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BusifyAPI.Controllers.UsersControllers
{
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequest request)
        {
            await _emailService.SendConfirmationEmailAsync(request.To, request.Subject, request.Body);
            return Ok("Email sent successfully.");
        }
    }

    public class EmailRequest
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
