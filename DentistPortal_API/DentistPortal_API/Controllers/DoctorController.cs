using DentistPortal_API.Data;
using DentistPortal_API.DTO;
using DentistPortal_API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentistPortal_API.Controllers
{
    public class DoctorController : Controller
    {
        private readonly WebsiteDbContext _context;
        private readonly IConfiguration _configuration;

        public DoctorController(WebsiteDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("api/create-medical-case")]
        public async Task<IActionResult> AddMedicalCase([FromBody] MedicalCaseDto medicalCase)
        {
            if (string.IsNullOrEmpty(medicalCase.Description) || string.IsNullOrEmpty(medicalCase.PatientName) || string.IsNullOrEmpty(medicalCase.PatientPhone) || string.IsNullOrEmpty(medicalCase.PatientAge) || medicalCase.DoctorId == null)
            {
                return BadRequest("Cant be empty");
            }
            MedicalCase medCase = await _context.MedicalCase.FirstOrDefaultAsync(x => x.PatientPhone == medicalCase.PatientPhone && x.IsActive == true);
            if (medCase is not null)
            {
                return BadRequest("Already Added!");
            }
            medCase = new();
            medCase.Id = Guid.NewGuid();
            medCase.PatientName = medicalCase.PatientName;
            medCase.PatientPhone = medicalCase.PatientPhone;
            medCase.DoctorId = medicalCase.DoctorId;
            medCase.Description = medicalCase.Description;
            medCase.IsActive = true;
            medCase.PatientAge = medicalCase.PatientAge;
            await _context.MedicalCase.AddAsync(medCase);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
