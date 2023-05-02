using DentistPortal_API.Model;
using DentistPortal_Client.Model;

namespace DentistPortal_Client.DTO
{
    public class ProfileDataDto
    {
        public Dentist dentist { get; set; }

        public List<FinishedCase> dentistcases { get; set; }
    }
}
