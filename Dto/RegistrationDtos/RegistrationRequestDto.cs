namespace BusifyAPI.Dto.RegistrationDtos
{
    public class RegistrationRequestDto
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public string Password { get; set; }
        public string RepeatPassword { get; set; }
    }
}
