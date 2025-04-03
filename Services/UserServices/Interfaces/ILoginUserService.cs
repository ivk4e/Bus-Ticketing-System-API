namespace BusifyAPI.Services.UserServices.Interfaces
{
    public interface ILoginUserService
    {
        Task<(bool Success, string Token)> LoginAsync(string username, string password);
    }
}
