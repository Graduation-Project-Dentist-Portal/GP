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
    public class JobController : Controller
    {

        private readonly WebsiteDbContext _context;
        private Cloudinary _cloudinary;

        public JobController(WebsiteDbContext context, IConfiguration configuration)
        {
            _context = context;
            Account account = new Account(configuration.GetSection("CLOUDINARY_URL").GetSection("cloudinary_name").Value,
                                          configuration.GetSection("CLOUDINARY_URL").GetSection("my_key").Value,
                                          configuration.GetSection("CLOUDINARY_URL").GetSection("my_secret_key").Value);
            _cloudinary = new Cloudinary(account);
        }

        [HttpGet]
        [Route("api/Display-All-Jobs")]
        public async Task<ActionResult<List<Job>>> DisplayAllJobs()
        {
            try
            {
                return Ok(await _context.Job.Where(x => x.IsActive == true).ToListAsync());
            }
            catch
            {
                return BadRequest("No available jobs or trainings, come back soon!");
            }
        }

        [HttpGet("api/Display-Job/{id}"), Authorize]
        public async Task<ActionResult> DisplayJob(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest("Cant be empty");
                }
                else
                {
                    var job = await _context.Job.FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true);
                    if (job is not null)
                    {
                        return Ok(job);
                    }
                    else
                    {
                        return BadRequest("Cant Find the job");
                    }
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("api/Add-Job")]
        public async Task<ActionResult> AddJob([FromBody] JobDto JobDto)
        {
            if (!await IsValid(JobDto))
                return BadRequest("Cant be empty!");
            var job = new Job();
            job.Id = Guid.NewGuid();
            job.Description = JobDto.Description;
            job.JobTitle = JobDto.JobTitle;
            job.Salary = JobDto.Salary;
            job.OwnerIdDoctor = JobDto.OwnerIdDoctor;
            job.ContactEmail = JobDto.ContactEmail;
            job.ContactNumber = JobDto.ContactNumber;
            job.Location = JobDto.Location;
            job.Duration = JobDto.Duration;
            job.IsActive = true;
            await _context.Job.AddAsync(job);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut]
        [Route("api/Edit-Job/{id}")]
        public async Task<ActionResult> EditJob(Guid id, [FromBody] JobDto jobDto)
        {
            if (id == Guid.Empty || !await IsValid(jobDto))
            {
                return BadRequest("Can't be empty");
            }
            else
            {
                var OldJob = await _context.Job.FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true);
                if (OldJob != null)
                {
                    OldJob.Description = jobDto.Description;
                    OldJob.JobTitle = jobDto.JobTitle;
                    OldJob.Salary = jobDto.Salary;
                    OldJob.ContactEmail = jobDto.ContactEmail;
                    OldJob.ContactNumber = jobDto.ContactNumber;
                    OldJob.Duration = jobDto.Duration;
                    OldJob.Location = jobDto.Location;
                    _context.Job.Update(OldJob);
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    return BadRequest("Can't find old job");
                }
            }
        }

        [HttpDelete]
        [Route("api/Delete-Job/{id}")]
        public async Task<ActionResult<Job>> DeleteJob(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Can't be empty");
            }
            else
            {
                var job = await _context.Job.FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true);
                if (job is not null)
                {
                    job.IsActive = false;
                    _context.Job.Update(job);
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    return BadRequest("Can't Find the job");
                }
            }
        }

        [HttpGet]
        [Route("api/My-Jobs/{id}"), Authorize]
        public async Task<IActionResult> GetMyJobs(Guid id)
        {
            try
            {
                return Ok(await _context.Job.Where(x => x.IsActive == true && x.OwnerIdDoctor == id).ToListAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task<bool> IsValid(JobDto jobDto)
        {
            if (string.IsNullOrEmpty(jobDto.JobTitle) ||
                string.IsNullOrEmpty(jobDto.Description) ||
                string.IsNullOrEmpty(jobDto.Salary) ||
                jobDto.OwnerIdDoctor == null ||
                jobDto.OwnerIdDoctor == Guid.Empty ||
                string.IsNullOrEmpty(jobDto.ContactEmail) ||
                string.IsNullOrEmpty(jobDto.Location) ||
                string.IsNullOrEmpty(jobDto.Duration))
            {
                return false;
            }
            return true;
        }
    }
}
