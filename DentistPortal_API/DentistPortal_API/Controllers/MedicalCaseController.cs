using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
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
        private Cloudinary _cloudinary;

        public MedicalCaseController(WebsiteDbContext context, IConfiguration configuration)
        {
            _context = context;
            Account account = new Account(configuration.GetSection("CLOUDINARY_URL").GetSection("cloudinary_name").Value,
                                          configuration.GetSection("CLOUDINARY_URL").GetSection("my_key").Value,
                                          configuration.GetSection("CLOUDINARY_URL").GetSection("my_secret_key").Value);
            _cloudinary = new Cloudinary(account);
        }

        [HttpPost]
        [Route("api/create-medical-case"), Authorize]
        public async Task<IActionResult> AddMedicalCase([FromForm] MedicalCaseDto medicalCaseDto)
        {
            try
            {
                if (!await IsValid(medicalCaseDto) || medicalCaseDto.CasePictures.Count == 0)
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
                var uploadResult = new ImageUploadResult();
                var medicalCaseImage = new MedicalCaseImage();
                medicalCase.Id = Guid.NewGuid();
                medicalCase.PatientName = medicalCaseDto.PatientName;
                medicalCase.PatientPhone = medicalCaseDto.PatientPhone;
                medicalCase.DoctorId = medicalCaseDto.DoctorId;
                medicalCase.Description = medicalCaseDto.Description;
                medicalCase.IsActive = true;
                medicalCase.PatientAge = medicalCaseDto.PatientAge;
                medicalCase.Diagnosis = medicalCaseDto.Diagnosis;
                medicalCase.TimeCreated = DateTime.Now;
                await _context.MedicalCase.AddAsync(medicalCase);
                await _context.SaveChangesAsync();
                foreach (var image in medicalCaseDto.CasePictures)
                {
                    using (var stream = image.OpenReadStream())
                    {
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(image.Name, stream)
                        };
                        uploadResult = _cloudinary.Upload(uploadParams);
                        medicalCaseImage.Id = Guid.NewGuid();
                        medicalCaseImage.MedicalCaseId = medicalCase.Id;
                        medicalCaseImage.IsActive = true;
                        medicalCaseImage.Url = uploadResult.Uri.ToString();
                        await _context.MedicalCaseImage.AddAsync(medicalCaseImage);
                        await _context.SaveChangesAsync();
                        uploadResult = new();
                        medicalCaseImage = new();
                    }
                }

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
                if (await _context.MedicalCase.CountAsync() == 0)
                    return Ok();
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
        public async Task<IActionResult> EditMedicalCase(Guid id, [FromForm] MedicalCaseDto medicalCaseDto)
        {
            try
            {
                if (id == Guid.Empty || !await IsValid(medicalCaseDto))
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
                        var uploadResult = new ImageUploadResult();
                        var medicalCaseImage = new MedicalCaseImage();
                        if (medicalCaseDto.CasePictures.Count > 0)
                        {
                            var oldPictures = await _context.MedicalCaseImage.Where(x => x.MedicalCaseId == id && x.IsActive == true).ToListAsync();
                            foreach (var picture in oldPictures)
                            {
                                picture.IsActive = false;
                                _context.MedicalCaseImage.Update(picture);
                                await _context.SaveChangesAsync();
                            }
                            foreach (var image in medicalCaseDto.CasePictures)
                            {
                                using (var stream = image.OpenReadStream())
                                {
                                    var uploadParams = new ImageUploadParams()
                                    {
                                        File = new FileDescription(image.Name, stream)
                                    };
                                    uploadResult = _cloudinary.Upload(uploadParams);
                                    medicalCaseImage.Id = Guid.NewGuid();
                                    medicalCaseImage.MedicalCaseId = oldMedicalCase.Id;
                                    medicalCaseImage.IsActive = true;
                                    medicalCaseImage.Url = uploadResult.Uri.ToString();
                                    await _context.MedicalCaseImage.AddAsync(medicalCaseImage);
                                    await _context.SaveChangesAsync();
                                    uploadResult = new();
                                    medicalCaseImage = new();
                                }
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
        public async Task<IActionResult> FinishMedicalCase([FromForm] FinishedCaseDto finishedCaseDto)
        {
            if (finishedCaseDto.CaseId == Guid.Empty || finishedCaseDto.DoctorId == Guid.Empty || finishedCaseDto.BeforePicture is null || finishedCaseDto.AfterPicture is null || string.IsNullOrEmpty(finishedCaseDto.DoctorWork))
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
                    var uploadResult = new ImageUploadResult();
                    using (var stream = finishedCaseDto.BeforePicture.OpenReadStream())
                    {
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(finishedCaseDto.BeforePicture.Name, stream)
                        };
                        uploadResult = _cloudinary.Upload(uploadParams);
                        finishedCase.BeforePicture = uploadResult.Uri.ToString();
                    }
                    uploadResult = new();
                    using (var stream = finishedCaseDto.AfterPicture.OpenReadStream())
                    {
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(finishedCaseDto.AfterPicture.Name, stream)
                        };
                        uploadResult = _cloudinary.Upload(uploadParams);
                        finishedCase.AfterPicture = uploadResult.Uri.ToString();
                    }
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

        private async Task<bool> IsValid(MedicalCaseDto medicalCaseDto)
        {
            if (string.IsNullOrEmpty(medicalCaseDto.Description) ||
                string.IsNullOrEmpty(medicalCaseDto.PatientName) ||
                string.IsNullOrEmpty(medicalCaseDto.PatientPhone) ||
                medicalCaseDto.PatientPhone == null ||
                medicalCaseDto.DoctorId == Guid.Empty ||
                string.IsNullOrEmpty(medicalCaseDto.Diagnosis))
                return false;
            return true;
        }

        [HttpGet]
        [Route("api/get-medical-case-pics/{medicalCaseId}"), Authorize]
        public async Task<IActionResult> GetPictures(Guid medicalCaseId)
        {
            if (medicalCaseId == Guid.Empty)
                return BadRequest("Tool id cant be empty!");
            try
            {
                return Ok(await _context.MedicalCaseImage.
                    Where(x => x.IsActive == true && x.MedicalCaseId == medicalCaseId).
                    Select(x => x.Url).
                    ToListAsync());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
