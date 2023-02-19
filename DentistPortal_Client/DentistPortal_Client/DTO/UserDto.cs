namespace DentistPortal_API.DTO
{
    public class UserDto
    {
        public string Username { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public string RepeatedPassword { get; set; } = String.Empty;
        public string FirstName { get; set; } = String.Empty;
        public string LastName { get; set; } = String.Empty;
        public string ProfilePicture { get; set; } = String.Empty;
        public string Role { get; set; } = String.Empty;
    }
}
