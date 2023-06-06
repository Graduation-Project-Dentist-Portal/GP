using DentistPortal_API.Data;
using DentistPortal_API.DTO;
using DentistPortal_API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace DentistPortal_API.Controllers
{
    public class ClinicController : Controller
    {
        private readonly WebsiteDbContext _context;
        private Cloudinary _cloudinary;

        public ClinicController(WebsiteDbContext context, IConfiguration configuration)
        {
            _context = context;
            Account account = new Account(configuration.GetSection("CLOUDINARY_URL").GetSection("cloudinary_name").Value,
                                          configuration.GetSection("CLOUDINARY_URL").GetSection("my_key").Value,
                                          configuration.GetSection("CLOUDINARY_URL").GetSection("my_secret_key").Value);
            _cloudinary = new Cloudinary(account);
        }

        [HttpGet]
        [Route("api/get-clinics"), Authorize]
        public async Task<IActionResult> GetClinics()
        {
            try
            {
                if (await _context.Clinic.CountAsync() == 0)
                    return Ok();
                return Ok(await _context.Clinic.Where(x => x.IsActive == true).ToListAsync());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("api/create-clinic"), Authorize]
        public async Task<IActionResult> AddClinic([FromForm] ClinicDto clinicDto)
        {
            try
            {
                if (!await IsValid(clinicDto) || clinicDto.CasePictures.Count == 0)
                {
                    return BadRequest("Cant be empty");
                }
                var clinic = await _context.Clinic.FirstOrDefaultAsync(x => x.Name == clinicDto.Name && x.IsActive == true);
                if (clinic is not null)
                {
                    return BadRequest("Clinic Name Already Taken!");
                }
                var uploadResult = new ImageUploadResult();
                var clinicImage = new ClinicImage();
                clinic = new();
                clinic.Id = Guid.NewGuid();
                clinic.Name = clinicDto.Name;
                clinic.ClinicPhone = clinicDto.ClinicPhone;
                clinic.DoctorId = clinicDto.DoctorId;
                clinic.ClinicDescription = clinicDto.ClinicDescription;
                clinic.IsActive = true;
                clinic.OpenTime = clinicDto.OpenTime;
                clinic.CloseTime = clinicDto.CloseTime;
                clinic.Address = clinicDto.Address;
                clinic.Score = 0;
                await _context.Clinic.AddAsync(clinic);
                await _context.SaveChangesAsync();
                foreach (var image in clinicDto.CasePictures)
                {
                    using (var stream = image.OpenReadStream())
                    {
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(image.Name, stream)
                        };
                        uploadResult = _cloudinary.Upload(uploadParams);
                        clinicImage.Id = Guid.NewGuid();
                        clinicImage.ClinicId = clinic.Id;
                        clinicImage.IsActive = true;
                        clinicImage.Url = uploadResult.Uri.ToString();
                        await _context.ClinicImage.AddAsync(clinicImage);
                        await _context.SaveChangesAsync();
                        uploadResult = new();
                        clinicImage = new();
                    }
                }
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        [Route("api/delete-clinic/{id}"), Authorize]
        public async Task<IActionResult> DeleteClinic(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    throw new InvalidOperationException("Cant be empty");
                else
                {
                    var clinic = await _context.Clinic.FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true);
                    if (clinic is not null)
                    {
                        clinic.IsActive = false;
                        _context.Clinic.Update(clinic);
                        await _context.SaveChangesAsync();
                        return Ok();
                    }
                    else
                    {
                        return BadRequest("Cant find the clinic");
                    }
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        [Route("api/edit-clinic/{id}"), Authorize]
        public async Task<IActionResult> EditClinic(Guid id, [FromForm] ClinicDto clinicDto)
        {
            try
            {
                if (id == Guid.Empty || !await IsValid(clinicDto))
                {
                    return BadRequest("Cant be empty");
                }
                else
                {
                    var oldClinic = await _context.Clinic.FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true);
                    if (oldClinic != null)
                    {
                        var uploadResult = new ImageUploadResult();
                        var clinicImage = new ClinicImage();
                        oldClinic.ClinicDescription = clinicDto.ClinicDescription;
                        oldClinic.Name = clinicDto.Name;
                        oldClinic.ClinicPhone = clinicDto.ClinicPhone;
                        oldClinic.Address = clinicDto.Address;
                        oldClinic.OpenTime = clinicDto.OpenTime;
                        oldClinic.CloseTime = clinicDto.CloseTime;
                        if (clinicDto.CasePictures.Count > 0)
                        {
                            var oldPictures = await _context.ClinicImage.Where(x => x.ClinicId == id && x.IsActive == true).ToListAsync();
                            foreach (var picture in oldPictures)
                            {
                                picture.IsActive = false;
                                _context.ClinicImage.Update(picture);
                                await _context.SaveChangesAsync();
                            }
                            foreach (var image in clinicDto.CasePictures)
                            {
                                using (var stream = image.OpenReadStream())
                                {
                                    var uploadParams = new ImageUploadParams()
                                    {
                                        File = new FileDescription(image.Name, stream)
                                    };
                                    uploadResult = _cloudinary.Upload(uploadParams);
                                    clinicImage.Id = Guid.NewGuid();
                                    clinicImage.ClinicId = oldClinic.Id;
                                    clinicImage.IsActive = true;
                                    clinicImage.Url = uploadResult.Uri.ToString();
                                    await _context.ClinicImage.AddAsync(clinicImage);
                                    await _context.SaveChangesAsync();
                                    uploadResult = new();
                                    clinicImage = new();
                                }
                            }
                        }
                        _context.Clinic.Update(oldClinic);
                        await _context.SaveChangesAsync();
                        return Ok();
                    }
                    else
                    {
                        return BadRequest("Cant find old clinic");
                    }
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("api/get-clinic/{id}"), Authorize]
        public async Task<IActionResult> GetClinic(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest("Cant be empty");
                }
                else
                {
                    var clinic = await _context.Clinic.FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true);
                    if (clinic is not null)
                    {
                        return Ok(clinic);
                    }
                    else
                    {
                        return BadRequest("Cant Find the clinic");
                    }
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private async Task<bool> IsValid(ClinicDto clinicDto)
        {
            if (string.IsNullOrEmpty(clinicDto.Name) ||
                string.IsNullOrEmpty(clinicDto.Address) ||
                string.IsNullOrEmpty(clinicDto.ClinicPhone) ||
                clinicDto.DoctorId == Guid.Empty ||
                string.IsNullOrEmpty(clinicDto.ClinicDescription) ||
                clinicDto.OpenTime == DateTime.MinValue ||
                clinicDto.CloseTime == DateTime.MinValue)
                return false;
            return true;
        }

        [HttpGet]
        [Route("api/get-clinic-pics/{clinicId}"), Authorize]
        public async Task<IActionResult> GetPictures(Guid clinicId)
        {
            if (clinicId == Guid.Empty)
                return BadRequest("Clinic id cant be empty!");
            try
            {
                return Ok(await _context.ClinicImage.
                    Where(x => x.IsActive == true && x.ClinicId == clinicId).
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
