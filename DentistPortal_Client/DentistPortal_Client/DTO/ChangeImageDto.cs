namespace DentistPortal_Client.DTO
{
    public class ChangeImageDto
    {
        public string username { get; set; }
        public IFormFile? ProfilePicture { get; set; }
        public IFormFile? IdentityCardPicture { get; set; }
        public IFormFile? UniversityCardPicture { get; set; }
    }
}
