using BusifyAPI.Services.UserServices.Interfaces;
using SendGrid.Helpers.Mail;
using SendGrid;
using System.Net;
using System.Net.Mail;

namespace BusifyAPI.Services.UserServices
{
    public class EmailService : IEmailService
    {
        private readonly string _apiKey;
        private readonly string _senderEmail;
        private readonly string _senderName;

        public EmailService(IConfiguration configuration)
        {
            _apiKey = configuration["SendGrid:ApiKey"];
            _senderEmail = configuration["SendGrid:SenderEmail"];
            _senderName = configuration["SendGrid:SenderName"];
        }

        public async Task SendConfirmationEmailAsync(string to, string subject, string confirmationLink)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress(_senderEmail, _senderName);
            var recipient = new EmailAddress(to);
            string htmlContent = $@"
            <html>
            <body style='font-family: Arial, sans-serif; text-align: center;'>
                <h2>Welcome to Busify!</h2>
                <p>Hello,</p>
                <p>Thank you for registering. Please click the button below to verify your email:</p>
                <a href='{confirmationLink}' style='display: inline-block; background-color: #007BFF; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; font-size: 16px;'>Verify Email</a>
                <p>If you did not create an account, please ignore this email.</p>
            </body>
            </html>";

            var msg = MailHelper.CreateSingleEmail(from, recipient, subject, htmlContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Email sending failed: {response.StatusCode}");
            }
        }
    }
}
