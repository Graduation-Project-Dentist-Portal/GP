namespace DentistPortal_API.DTO
{
    public class ProfileDataDto
    {
        public DentistDto dentist { get; set; }

        public List<FinishedCaseDto> cases { get; set; }
    }
}
