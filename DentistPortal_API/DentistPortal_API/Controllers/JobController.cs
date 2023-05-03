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
        public async Task<ActionResult> AddJob([FromBody] JobDto Jobdto)
        {
            var job = new Job();
            job.Id = Guid.NewGuid();
            job.Description = Jobdto.Description;
            job.JobTitle = Jobdto.JobTitle;
            job.Salary = Jobdto.Salary;
            job.OwnerIdDoctor = Jobdto.OwnerIdDoctor;
            job.ContactEmail = Jobdto.ContactEmail;
            job.ContactNumber = Jobdto.ContactNumber;
            job.Location = Jobdto.Location;
            job.Duration = Jobdto.Duration;
            job.IsActive = true;


            await _context.Job.AddAsync(job);
            await _context.SaveChangesAsync();


            return Ok();
        }




        [HttpPut]
        [Route("api/Edit-Job/{id}")]
        public async Task<ActionResult> EditJob(Guid id, [FromBody] JobDto jobdto)
        {
            if (id == Guid.Empty || string.IsNullOrEmpty(jobdto.JobTitle) || string.IsNullOrEmpty(jobdto.Description) ||
                string.IsNullOrEmpty(jobdto.ContactEmail) || string.IsNullOrEmpty(jobdto.Salary) || string.IsNullOrEmpty(jobdto.Location)
                || string.IsNullOrEmpty(jobdto.Duration))
            {
                return BadRequest("Can't be empty");
            }
            else
            {
                var OldJob = await _context.Job.FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true);
                if (OldJob != null)
                {
                    OldJob.Description = jobdto.Description;
                    OldJob.JobTitle = jobdto.JobTitle;
                    OldJob.Salary = jobdto.Salary;
                    OldJob.ContactEmail = jobdto.ContactEmail;
                    OldJob.ContactNumber = jobdto.ContactNumber;
                    OldJob.Duration = jobdto.Duration;
                    OldJob.Location = jobdto.Location;


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

    }
}
