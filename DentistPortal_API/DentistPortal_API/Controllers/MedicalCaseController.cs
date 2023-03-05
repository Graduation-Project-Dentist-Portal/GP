using DentistPortal_API.Data;
using DentistPortal_API.DTO;
using DentistPortal_API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentistPortal_API.Controllers
{
    public class MedicalCaseController : Controller
    {
        private readonly WebsiteDbContext _context;

        public MedicalCaseController(WebsiteDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("api/create-medical-case"), Authorize]
        public async Task<IActionResult> AddMedicalCase([FromBody] MedicalCaseDto medicalCaseDto)
        {
            try
            {
                if (string.IsNullOrEmpty(medicalCaseDto.Description) || string.IsNullOrEmpty(medicalCaseDto.PatientName) || string.IsNullOrEmpty(medicalCaseDto.PatientPhone) || medicalCaseDto.PatientPhone == null || medicalCaseDto.DoctorId == Guid.Empty || string.IsNullOrEmpty(medicalCaseDto.CasePictures[0]) || string.IsNullOrEmpty(medicalCaseDto.Diagnosis))
                {
                    return BadRequest("Cant be empty");
                }
                MedicalCase medicalCase = await _context.MedicalCase.FirstOrDefaultAsync(x => x.PatientPhone == medicalCaseDto.PatientPhone && x.IsActive == true);
                if (medicalCase is not null)
                {
                    return BadRequest("Already Added!");
                }
                medicalCase = new();
                if (medicalCaseDto.AssignedToMe == true)
                {
                    if (await _context.MedicalCase.CountAsync(x => x.AssignedDoctorId == medicalCaseDto.DoctorId && x.CaseStatus == "Pending") >= 2)
                        return BadRequest("Cant take more than 2 cases at a time!");
                    medicalCase.AssignedDoctorId = medicalCaseDto.DoctorId;
                    medicalCase.CaseStatus = "Pending";
                }
                else
                    medicalCase.CaseStatus = "Open";
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
                    if (x != medicalCaseDto.CasePictures.Last())
                        medicalCase.PicturePaths += x + ",";
                    else
                        medicalCase.PicturePaths += x;
                }
                medicalCase.TimeCreated = DateTime.Now;
                await _context.MedicalCase.AddAsync(medicalCase);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("api/display-medical-cases"), Authorize]
        public async Task<ActionResult<List<MedicalCase>>> GetMedicalCase()
        {
            try
            {
                return Ok(await _context.MedicalCase.Where(x => x.IsActive == true && x.CaseStatus == "Open").OrderBy(x => x.TimeCreated).ToListAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("api/delete-medical-case/{id}"), Authorize]
        public async Task<IActionResult> DeleteMedicalCase(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    throw new InvalidOperationException("Cant be empty");
                else
                {
                    MedicalCase medCase = await _context.MedicalCase.FirstOrDefaultAsync(x => x.Id == id);
                    if (medCase is not null)
                    {
                        medCase.IsActive = false;
                        _context.MedicalCase.Update(medCase);
                        await _context.SaveChangesAsync();
                        return Ok();
                    }
                    else
                    {
                        return BadRequest("Cant find the medical case");
                    }
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("api/get-medical-case/{id}"), Authorize]
        public async Task<IActionResult> GetMedicalCase(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest("Cant be empty");
                }
                else
                {
                    var medicalCase = await _context.MedicalCase.FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true);
                    if (medicalCase is not null)
                    {
                        return Ok(medicalCase);
                    }
                    else
                    {
                        return BadRequest("Cant Find the medical case");
                    }
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        [Route("api/edit-medical-case/{id}"), Authorize]
        public async Task<IActionResult> EditMedicalCase(Guid id, [FromBody] MedicalCaseDto medicalCaseDto)
        {
            try
            {
                if (id == Guid.Empty || string.IsNullOrEmpty(medicalCaseDto.Description) || string.IsNullOrEmpty(medicalCaseDto.PatientName) || string.IsNullOrEmpty(medicalCaseDto.PatientPhone) || medicalCaseDto.PatientPhone == null || string.IsNullOrEmpty(medicalCaseDto.Diagnosis))
                {
                    return BadRequest("Cant be empty");
                }
                else
                {
                    var oldMedicalCase = await _context.MedicalCase.FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true);
                    if (oldMedicalCase != null)
                    {
                        oldMedicalCase.Description = medicalCaseDto.Description;
                        oldMedicalCase.PatientName = medicalCaseDto.PatientName;
                        oldMedicalCase.PatientPhone = medicalCaseDto.PatientPhone;
                        oldMedicalCase.PatientAge = medicalCaseDto.PatientAge;
                        oldMedicalCase.Diagnosis = medicalCaseDto.Diagnosis;
                        if (medicalCaseDto.CasePictures.Count > 0)
                        {
                            oldMedicalCase.PicturePaths = string.Empty;
                            foreach (var x in medicalCaseDto.CasePictures)
                            {
                                if (x != medicalCaseDto.CasePictures.Last())
                                    oldMedicalCase.PicturePaths += x + ",";
                                else
                                    oldMedicalCase.PicturePaths += x;
                            }
                        }
                        _context.MedicalCase.Update(oldMedicalCase);
                        await _context.SaveChangesAsync();
                        return Ok();
                    }
                    else
                    {
                        return BadRequest("Cant find old medical case");
                    }
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("api/my-medical-cases/{id}"), Authorize]
        public async Task<IActionResult> GetMyMedicalCases(Guid id)
        {
            try
            {
                return Ok(await _context.MedicalCase.Where(x => x.IsActive == true && x.AssignedDoctorId == id).ToListAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("api/take-medical-case/{caseId}"), Authorize]
        public async Task<IActionResult> TakeMedicalCase(Guid caseId, [FromBody] Guid userId)
        {
            try
            {
                if (await _context.MedicalCase.CountAsync(x => x.AssignedDoctorId == userId && x.CaseStatus == "Pending") >= 2)
                    return BadRequest("Cant take more than 2 cases at a time!");
                var medCase = await _context.MedicalCase.FirstOrDefaultAsync(x => x.Id == caseId);
                if (medCase == null)
                { return BadRequest("Cant find the case"); }
                else
                {
                    medCase.AssignedDoctorId = userId;
                    medCase.CaseStatus = "Pending";
                    _context.MedicalCase.Update(medCase);
                    await _context.SaveChangesAsync();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("api/leave-medical-case"), Authorize]
        public async Task<IActionResult> LeaveMedicalCase([FromBody] Guid caseId)
        {
            try
            {
                var medCase = await _context.MedicalCase.FirstOrDefaultAsync(x => x.Id == caseId);
                if (medCase == null)
                { return BadRequest("Cant find the case"); }
                else
                {
                    medCase.AssignedDoctorId = Guid.Empty;
                    medCase.CaseStatus = "Open";
                    _context.MedicalCase.Update(medCase);
                    await _context.SaveChangesAsync();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/finish-medical-case"), Authorize]
        public async Task<IActionResult> FinishMedicalCase([FromBody] FinishedCaseDto finishedCaseDto)
        {
            if (finishedCaseDto.CaseId == Guid.Empty || finishedCaseDto.DoctorId == Guid.Empty || string.IsNullOrEmpty(finishedCaseDto.BeforePicture) || string.IsNullOrEmpty(finishedCaseDto.AfterPicture) || string.IsNullOrEmpty(finishedCaseDto.DoctorWork))
                return BadRequest("Cant be empty");
            try
            {
                var medCase = await _context.MedicalCase.FirstOrDefaultAsync(x => x.Id == finishedCaseDto.CaseId);
                if (medCase == null)
                { return BadRequest("Cant find the case"); }
                else
                {
                    medCase.CaseStatus = "Closed";
                    FinishedCases finishedCase = new();
                    finishedCase.Id = Guid.NewGuid();
                    finishedCase.AfterPicture = finishedCaseDto.AfterPicture;
                    finishedCase.BeforePicture = finishedCaseDto.BeforePicture;
                    finishedCase.CaseId = finishedCaseDto.CaseId;
                    finishedCase.DoctorId = finishedCaseDto.DoctorId;
                    finishedCase.DoctorWork = finishedCaseDto.DoctorWork;
                    await _context.FinishedCases.AddAsync(finishedCase);
                    _context.MedicalCase.Update(medCase);
                    await _context.SaveChangesAsync();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
