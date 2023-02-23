using DentistPortal_API.Data;
using DentistPortal_API.DTO;
using DentistPortal_API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentistPortal_API.Controllers
{
    public class MedicalCaseController : Controller
    {
        private readonly WebsiteDbContext _context;
        private readonly IConfiguration _configuration;

        public MedicalCaseController(WebsiteDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("api/create-medical-case")]
        public async Task<IActionResult> AddMedicalCase([FromBody] MedicalCaseDto medicalCaseDto)
        {
            if (string.IsNullOrEmpty(medicalCaseDto.Description) || string.IsNullOrEmpty(medicalCaseDto.PatientName) || string.IsNullOrEmpty(medicalCaseDto.PatientPhone) || string.IsNullOrEmpty(medicalCaseDto.PatientAge) || medicalCaseDto.DoctorId == null || string.IsNullOrEmpty(medicalCaseDto.CasePictures[0]))
            {
                return BadRequest("Cant be empty");
            }
            MedicalCase medicalCase = await _context.MedicalCase.FirstOrDefaultAsync(x => x.PatientPhone == medicalCaseDto.PatientPhone && x.IsActive == true);
            if (medicalCase is not null)
            {
                return BadRequest("Already Added!");
            }
            medicalCase = new();
            medicalCase.Id = Guid.NewGuid();
            medicalCase.PatientName = medicalCaseDto.PatientName;
            medicalCase.PatientPhone = medicalCaseDto.PatientPhone;
            medicalCase.DoctorId = medicalCaseDto.DoctorId;
            medicalCase.Description = medicalCaseDto.Description;
            medicalCase.IsActive = true;
            medicalCase.PatientAge = medicalCaseDto.PatientAge;
            medicalCase.Diagnosis = medicalCaseDto.Diagnosis;
            foreach (var x in medicalCaseDto.CasePictures)
            {
                medicalCase.PicturePaths += x + ",";
            }
            await _context.MedicalCase.AddAsync(medicalCase);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [Route("api/display-medical-cases")]
        public async Task<ActionResult<List<MedicalCase>>> GetMedicalCase()
        {
            try
            {
                return Ok(await _context.MedicalCase.ToListAsync());
            }
            catch
            {
                return BadRequest("There are no cases to show");
            }
        }
    }
}
