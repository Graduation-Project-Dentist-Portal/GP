using DentistPortal_API.Data;
using DentistPortal_API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentistPortal_API.Controllers
{
    public class AdminController : Controller
    {
        private readonly WebsiteDbContext _context;

        public AdminController(WebsiteDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("api/get-unverified-users"), Authorize]
        public async Task<IActionResult> GetUnverifiedUsers()
        {
            try
            {
                if (await _context.Dentist.CountAsync() == 0)
                    return Ok();
                return Ok(await _context.Dentist.Where(x => x.IsActive == true && x.IsVerified == "false").ToListAsync());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        [Route("api/verify-user"), Authorize]
        public async Task<IActionResult> VerifyUser([FromBody] Guid id)
        {
            try
            {
                var dentist = await _context.Dentist.FindAsync(id);
                if (dentist == null)
                    return BadRequest("Cant find the selected dentist");
                dentist.IsVerified = "true";
                _context.Dentist.Update(dentist);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        [Route("api/un-verify-user/{msg}"), Authorize]
        public async Task<IActionResult> UnVerifyUser([FromBody] Guid id, string msg)
        {
            try
            {
                var dentist = await _context.Dentist.FindAsync(id);
                if (dentist == null)
                    return BadRequest("Cant find the selected dentist");
                dentist.IsVerified = "pending";
                dentist.VerfiyMessage = msg;
                _context.Dentist.Update(dentist);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
