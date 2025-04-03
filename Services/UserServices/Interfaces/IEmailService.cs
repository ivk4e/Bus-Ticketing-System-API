namespace BusifyAPI.Services.UserServices.Interfaces
{
    public interface IEmailService
    {
        Task SendConfirmationEmailAsync(string to, string subject, string confirmationLink);
    }
}
