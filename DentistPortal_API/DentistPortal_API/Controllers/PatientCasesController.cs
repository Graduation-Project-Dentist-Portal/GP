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
    public class PatientCasesController : Controller
    {
        private readonly WebsiteDbContext _context;
        private Cloudinary _cloudinary;

        public PatientCasesController(WebsiteDbContext context, IConfiguration configuration)
        {
            _context = context;
            Account account = new Account(configuration.GetSection("CLOUDINARY_URL").GetSection("cloudinary_name").Value,
                                          configuration.GetSection("CLOUDINARY_URL").GetSection("my_key").Value,
                                          configuration.GetSection("CLOUDINARY_URL").GetSection("my_secret_key").Value);
            _cloudinary = new Cloudinary(account);
        }


        [HttpPost]
        [Route("api/create-patient-case")]
        [Authorize]
        public async Task<IActionResult> AddMyCase([FromForm] PatientCaseDto patientcaseDto)
        {
            try
            {

            var NewPatientCase = new PatientCase();
                var patientcaseimage = new PatientCaseImage();
                var uploadResult = new ImageUploadResult();
            
            NewPatientCase.Patient = null;
            NewPatientCase.IsActive = true;
            NewPatientCase.Id = Guid.NewGuid();
            NewPatientCase.CaseStatus = "Open";
            NewPatientCase.PatientPhone = patientcaseDto.PatientPhone;
            NewPatientCase.PatientAge = patientcaseDto.PatientAge;
            NewPatientCase.Description = patientcaseDto.Description;
            NewPatientCase.PatientId = patientcaseDto.PatientId;
            //NewPatientCase.AssignedDoctorId = patientcaseDto.AssignedDoctorId;
            NewPatientCase.TimeCreated = DateTime.Now;
            await _context.PatientCase.AddAsync(NewPatientCase);
            await _context.SaveChangesAsync();
            foreach (var image in patientcaseDto.PatientCasePictures)
            {
                using (var stream = image.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(image.Name, stream)
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                    patientcaseimage.Id = Guid.NewGuid();
                    patientcaseimage.PatientCaseId = NewPatientCase.Id;
                    patientcaseimage.IsActive = true;
                    patientcaseimage.Url = uploadResult.Uri.ToString();
                    await _context.PatientCaseImage.AddAsync(patientcaseimage);
                    await _context.SaveChangesAsync();
                    uploadResult = new();
                    patientcaseimage = new();
                }
            }




            return Ok();

        }
         catch (Exception e)
            {
                return BadRequest(e.Message);
    }


}




        [HttpPut]
        [Route("api/take-case-all/{caseId}") ]
        [Authorize]
        public async Task<IActionResult> TakeCaseAll(Guid caseId, [FromBody] Guid userId)
        {
            try
            {
                var medicalCase = await _context.MedicalCase.FirstOrDefaultAsync(x => x.Id == caseId);
                if (medicalCase != null)
                {
                    // Take the medical case
                    if (await _context.MedicalCase.CountAsync(x => x.AssignedDoctorId == userId && x.CaseStatus == "Pending") >= 2)
                        return BadRequest("Cant take more than 2 cases at a time!");
                    medicalCase.AssignedDoctorId = userId;
                    medicalCase.CaseStatus = "Pending";
                    _context.MedicalCase.Update(medicalCase);
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                
                    var patientCase = await _context.PatientCase.FirstOrDefaultAsync(x => x.Id == caseId);
                    if (patientCase != null)
                    {
                        // Take the patient case
                        if (await _context.PatientCase.CountAsync(x => x.AssignedDoctorId == userId && x.CaseStatus == "Pending") >= 2)
                            return BadRequest("Cant take more than 2 cases at a time!");
                        patientCase.AssignedDoctorId = userId;
                        patientCase.CaseStatus = "Pending";
                        _context.PatientCase.Update(patientCase);
                        await _context.SaveChangesAsync();
                        return Ok();
                    }
                
               

                return BadRequest("Cant find the case");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("api/leave-case-all")]
        [Authorize]

        public async Task<IActionResult> LeaveCaseAll([FromBody] Guid caseId)
        {
            try
            {
                var patientcase = await _context.PatientCase.FirstOrDefaultAsync(x => x.Id == caseId);
                if (patientcase != null)
                {
                    patientcase.AssignedDoctorId = Guid.Empty;
                    patientcase.CaseStatus = "open";
                    _context.PatientCase.Update(patientcase);
                    await _context.SaveChangesAsync();
                    return Ok();
                }

                var medCase = await _context.MedicalCase.FirstOrDefaultAsync(x => x.Id == caseId);
                if (medCase != null)
                {
                    medCase.AssignedDoctorId = Guid.Empty;
                    medCase.CaseStatus = "Open";
                    _context.MedicalCase.Update(medCase);
                    await _context.SaveChangesAsync();
                    return Ok();
                }

                return BadRequest("Cant find the case");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("api/finish-patient-case")]
        [Authorize]
        public async Task<IActionResult> FinishPatientCase([FromForm] FinishedCaseDto finishedCaseDto)
        {

            if (finishedCaseDto.CaseId == Guid.Empty || finishedCaseDto.DoctorId == Guid.Empty || finishedCaseDto.BeforePicture is null || finishedCaseDto.AfterPicture is null || string.IsNullOrEmpty(finishedCaseDto.DoctorWork))
                return BadRequest("Cant be empty");

            try
            {
                var patientCase = await _context.PatientCase.FirstOrDefaultAsync(x => x.Id == finishedCaseDto.CaseId);
                if (patientCase != null)
                {
                    patientCase.CaseStatus = "Closed";
                    var finishedCase = new FinishedCases();
                    finishedCase.Id = Guid.NewGuid();
                    finishedCase.CaseId = finishedCaseDto.CaseId;
                    finishedCase.DoctorId = finishedCaseDto.DoctorId;
                    finishedCase.DoctorWork = finishedCaseDto.DoctorWork;
                    finishedCase.MedicalCaseId = null;
                    //finishedCase.PatientCase = null;
                    //finishedCase.MedicalCase = null;
                    finishedCase.IsActive = true;

                    finishedCase.PatientCaseId = patientCase.Id;

                    var uploadResult = new ImageUploadResult();
                    using (var beforeStream = finishedCaseDto.BeforePicture.OpenReadStream())
                    {
                        var beforeUploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(finishedCaseDto.BeforePicture.Name, beforeStream)
                        };
                        uploadResult = _cloudinary.Upload(beforeUploadParams);
                        finishedCase.BeforePicture = uploadResult.Uri.ToString();
                    }

                    uploadResult = new();
                    using (var afterStream = finishedCaseDto.AfterPicture.OpenReadStream())
                    {
                        var afterUploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(finishedCaseDto.AfterPicture.Name, afterStream)
                        };
                        uploadResult = _cloudinary.Upload(afterUploadParams);
                        finishedCase.AfterPicture = uploadResult.Uri.ToString();
                    }

                    await _context.FinishedCases.AddAsync(finishedCase);
                    _context.PatientCase.Update(patientCase);

                }
                else
                {
                    var medCase = await _context.MedicalCase.FirstOrDefaultAsync(x => x.Id == finishedCaseDto.CaseId);
                    if (medCase != null)
                    {
                        medCase.CaseStatus = "Closed";
                        var finishedCase = new FinishedCases();
                        finishedCase.Id = Guid.NewGuid();
                        finishedCase.CaseId = finishedCaseDto.CaseId;
                        finishedCase.DoctorId = finishedCaseDto.DoctorId;
                        finishedCase.DoctorWork = finishedCaseDto.DoctorWork;
                        finishedCase.MedicalCaseId = medCase.Id;
                        finishedCase.IsActive = true;

                        finishedCase.PatientCaseId = null;
                        //finishedCase.PatientCase = null;
                        //finishedCase.MedicalCase = null;


                        var uploadResult = new ImageUploadResult();
                        using (var beforeStream = finishedCaseDto.BeforePicture.OpenReadStream())
                        {
                            var beforeUploadParams = new ImageUploadParams()
                            {
                                File = new FileDescription(finishedCaseDto.BeforePicture.Name, beforeStream)
                            };
                            uploadResult = _cloudinary.Upload(beforeUploadParams);
                            finishedCase.BeforePicture = uploadResult.Uri.ToString();
                        }

                        uploadResult = new();
                        using (var afterStream = finishedCaseDto.AfterPicture.OpenReadStream())
                        {
                            var afterUploadParams = new ImageUploadParams()
                            {
                                File = new FileDescription(finishedCaseDto.AfterPicture.Name, afterStream)
                            };
                            uploadResult = _cloudinary.Upload(afterUploadParams);
                            finishedCase.AfterPicture = uploadResult.Uri.ToString();
                        }
                        
                        await _context.FinishedCases.AddAsync(finishedCase);
                        _context.MedicalCase.Update(medCase);

                    }
                    else
                    {
                        return BadRequest("There is no case");
                    }
                }

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Get Patient Case by Id For Edit Function

        [HttpGet]
        [Route("api/get-patient-case/{id}")]
        [Authorize]
        public async Task<IActionResult> GetPatientCase(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest("Cant be empty");
                }
                else
                {
                    var patientCase = await _context.PatientCase.FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true);
                    if (patientCase is not null)
                    {
                        return Ok(patientCase);
                    }
                    else
                    {
                        return BadRequest("Cant Find patient case");
                    }
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        [Route("api/edit-patient-case/{id}")]
        [Authorize]
        public async Task<IActionResult> EditPatientCase(Guid id, [FromForm] PatientCaseDto patientCaseDto)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest("Can't Be empty");
                }

                else
                {
                    var oldPatientCase = await _context.PatientCase.FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true);
                    if (oldPatientCase != null)
                    {
                        oldPatientCase.Description = patientCaseDto.Description;
                        oldPatientCase.PatientPhone = patientCaseDto.PatientPhone;
                        oldPatientCase.PatientAge = patientCaseDto.PatientAge;
                        var uploadResult = new ImageUploadResult();
                        var patientCaseImage = new PatientCaseImage();
                        if (patientCaseDto.PatientCasePictures.Count > 0)
                        {
                            var oldPictures = await _context.PatientCaseImage.Where(x => x.PatientCaseId == id && x.IsActive == true).ToListAsync();
                            foreach (var picture in oldPictures)
                            {
                                picture.IsActive = false;
                                _context.PatientCaseImage.Update(picture);
                                await _context.SaveChangesAsync();
                            }
                            foreach (var image in patientCaseDto.PatientCasePictures)
                            {
                                using (var stream = image.OpenReadStream())
                                {
                                    var uploadParams = new ImageUploadParams()
                                    {
                                        File = new FileDescription(image.Name, stream)
                                    };
                                    uploadResult = _cloudinary.Upload(uploadParams);
                                    patientCaseImage.Id = Guid.NewGuid();
                                    patientCaseImage.PatientCaseId = oldPatientCase.Id;
                                    patientCaseImage.IsActive = true;
                                    patientCaseImage.Url = uploadResult.Uri.ToString();
                                    await _context.PatientCaseImage.AddAsync(patientCaseImage);
                                    await _context.SaveChangesAsync();
                                    uploadResult = new();
                                    patientCaseImage = new();
                                }
                            }
                        }
                        _context.PatientCase.Update(oldPatientCase);
                        await _context.SaveChangesAsync();
                        return Ok();
                    }
                    else
                    {
                        return BadRequest("Can't find old patient case");
                    }
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        [Route("api/delete-patient-case/{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePatientCase(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    throw new InvalidOperationException("Can't be empty");
                else
                {
                    PatientCase patientCase = await _context.PatientCase.FirstOrDefaultAsync(x => x.Id == id);
                    if (patientCase is not null)
                    {
                        patientCase.IsActive = false;
                        _context.PatientCase.Update(patientCase);
                        await _context.SaveChangesAsync();
                        return Ok();
                    }
                    else
                    {
                        return BadRequest("Cant find patient case");
                    }
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpGet]
        [Route("api/get-patient-case-pics/{patientCaseId}")]
        [Authorize]

        public async Task<IActionResult> GetPictures(Guid patientCaseId)
        {
            if (patientCaseId == Guid.Empty)
                return BadRequest("Tool id cant be empty!");
            try
            {
                return Ok(await _context.PatientCaseImage.
                    Where(x => x.IsActive == true && x.PatientCaseId == patientCaseId).
                    Select(x => x.Url).
                    ToListAsync());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }



        //My Medical Cases Pages 
        [HttpGet]
        [Route("api/my-cases-doctor/{id}")]
        [Authorize]
        public async Task<IActionResult> DisplayCases(Guid id)
        {
            var patientCases = await _context.PatientCase.Where(x => x.IsActive == true && x.AssignedDoctorId == id).ToListAsync();
            var medCases = await _context.MedicalCase.Where(x => x.IsActive == true && x.AssignedDoctorId == id).ToListAsync();
            var combinedList = new CombinedCasesDto
            {
                MedicalCases = medCases,
                PatientCases = patientCases
            };

            return Ok(combinedList);

        }
       
        
        // Display Medical Pages 
        [HttpGet]
        [Route("api/All-cases-doctor")]
        [Authorize]
        public async Task<IActionResult> DisplayAllCases()
        {
            var patientCases = await _context.PatientCase.Where(x => x.IsActive == true && x.CaseStatus == "open").ToListAsync();
            var medCases = await _context.MedicalCase.Where(x => x.IsActive == true && x.CaseStatus == "open").ToListAsync();
            var combinedList = new CombinedCasesDto
            {
                MedicalCases = medCases,
                PatientCases = patientCases
            };

            return Ok(combinedList);

        }

        // Display My Cases Patient Page
        [HttpGet]
        [Route("api/display-my-cases-pateint/{id}")]
        [Authorize]
        public async Task<ActionResult<List<PatientCase>>> DisplayMyCases(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Id cant be empty!");
            try
            {
                return Ok(await _context.PatientCase
                    .Where(c => c.IsActive == true && c.PatientId == id)
                    .ToListAsync());

            }
            catch
            {
                return BadRequest("There are no Cases to show");
            }
        }














        //[HttpGet]
        //[Route("api/my-patients-cases/{id}")]
        ////[Authorize]
        //public async Task<IActionResult> GetMyMedicalCases(Guid id)
        //{
        //    try
        //    {
        //        return Ok(await _context.PatientCase.Where(x => x.IsActive == true && x.AssignedDoctorId == id).ToListAsync());
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        [HttpGet]
        [Route("api/display-patient-cases")]
        //[Authorize]
        public async Task<ActionResult<List<PatientCase>>> DisplayPatientCases()
        {

            try
            {
                if (await _context.PatientCase.CountAsync() == 0)

                    return Ok();
                return Ok(await _context.PatientCase.Where(x => x.IsActive == true && x.CaseStatus == "open")
               .ToListAsync());

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        //[HttpPut]
        //[Route("api/take-patient-case/{CaseId}")]
        //[Authorize]
        //public async Task<IActionResult> TakePatientCase(Guid CaseId ,[FromBody]Guid UserId)
        //{
        //    try
        //    {
        //        if (await _context.PatientCase.CountAsync(x => x.AssignedDoctorId == UserId && x.CaseStatus == "Pending") >= 2)
        //            return BadRequest("Cant take more than 2 cases at a time!");
        //        var patientcase = await _context.PatientCase.FirstOrDefaultAsync(x => x.Id == CaseId);
        //        if (patientcase == null)
        //        { return BadRequest("Cant find the case"); }
        //        else
        //        {
        //            patientcase.AssignedDoctorId = UserId;
        //            patientcase.CaseStatus = "Pending";
        //            _context.PatientCase.Update(patientcase);
        //            await _context.SaveChangesAsync();
        //            return Ok();
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}




    }



}








