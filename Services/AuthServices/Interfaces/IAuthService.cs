namespace BusifyAPI.Services.AuthServices.Interfaces
{
    public interface IAuthService
    {
        string Authenticate(string apiKey, string apiSecret);

        bool ValidateApiKeyAndSecret(string apiKey, string apiSecret);
    }
}
