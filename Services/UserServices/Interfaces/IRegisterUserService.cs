using BusifyAPI.Dto.RegistrationDtos;
using Microsoft.AspNetCore.Identity.Data;

namespace BusifyAPI.Services.UserServices.Interfaces
{
    public interface IRegisterUserService
    {
        Task<RegistrationResult> RegisterUser(RegistrationRequestDto registrationDto);
        Task<RegistrationResult> ConfirmRegistration(string token);
        Task<RegistrationResult> ResendConfirmationEmail(string email);
    }
}
