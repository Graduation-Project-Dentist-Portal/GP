using DentistPortal_API.Data;
using DentistPortal_API.DTO;
using DentistPortal_API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentistPortal_API.Controllers
{
    public class ClinicController : Controller
    {
        private readonly WebsiteDbContext _context;

        public ClinicController(WebsiteDbContext context)
        {
            _context = context;
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
        public async Task<IActionResult> AddClinic([FromBody] ClinicDto clinicDto)
        {
            try
            {
                if (string.IsNullOrEmpty(clinicDto.Name) || string.IsNullOrEmpty(clinicDto.Address) || string.IsNullOrEmpty(clinicDto.ClinicPhone) || clinicDto.DoctorId == Guid.Empty || string.IsNullOrEmpty(clinicDto.ClinicDescription) || clinicDto.OpenTime == DateTime.MinValue || clinicDto.CloseTime == DateTime.MinValue)
                {
                    return BadRequest("Cant be empty");
                }
                Clinic clinic = await _context.Clinic.FirstOrDefaultAsync(x => x.Name == clinicDto.Name && x.IsActive == true);
                if (clinic is not null)
                {
                    return BadRequest("Already Added!");
                }
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
                foreach (var x in clinicDto.CasePictures)
                {
                    if (x != clinicDto.CasePictures.Last())
                        clinic.PicturePaths += x + ",";
                    else
                        clinic.PicturePaths += x;
                }
                await _context.Clinic.AddAsync(clinic);
                await _context.SaveChangesAsync();
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
                    Clinic clinic = await _context.Clinic.FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true);
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
        public async Task<IActionResult> EditClinic(Guid id, [FromBody] ClinicDto clinicDto)
        {
            try
            {
                if (id == Guid.Empty || string.IsNullOrEmpty(clinicDto.Name) || string.IsNullOrEmpty(clinicDto.Address) || string.IsNullOrEmpty(clinicDto.ClinicPhone) || clinicDto.DoctorId == Guid.Empty || string.IsNullOrEmpty(clinicDto.ClinicDescription) || clinicDto.OpenTime == DateTime.MinValue || clinicDto.CloseTime == DateTime.MinValue)
                {
                    return BadRequest("Cant be empty");
                }
                else
                {
                    var oldClinic = await _context.Clinic.FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true);
                    if (oldClinic != null)
                    {
                        oldClinic.ClinicDescription = clinicDto.ClinicDescription;
                        oldClinic.Name = clinicDto.Name;
                        oldClinic.ClinicPhone = clinicDto.ClinicPhone;
                        oldClinic.Address = clinicDto.Address;
                        oldClinic.OpenTime = clinicDto.OpenTime;
                        oldClinic.CloseTime = clinicDto.CloseTime;
                        if (clinicDto.CasePictures.Count > 0)
                        {
                            oldClinic.PicturePaths = string.Empty;
                            foreach (var x in clinicDto.CasePictures)
                            {
                                if (x != clinicDto.CasePictures.Last())
                                    oldClinic.PicturePaths += x + ",";
                                else
                                    oldClinic.PicturePaths += x;
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
    }
}
