using DentistPortal_API.Data;
using DentistPortal_API.DTO;
using DentistPortal_API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentistPortal_API.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly WebsiteDbContext _context;

        public FeedbackController(WebsiteDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("api/get-feedbacks/{id}"), Authorize]
        public async Task<IActionResult> GetFeedbacks(Guid id)
        {
            try
            {
                if (await _context.Feedback.CountAsync(x => x.ClinicId == id && x.IsActive == true) == 0)
                    return Ok();
                return Ok(await _context.Feedback.Where(x => x.ClinicId == id && x.IsActive == true).ToListAsync());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("api/add-feedback"), Authorize]
        public async Task<IActionResult> AddFeedback([FromBody] FeedbackDto feedbackDto)
        {
            if (string.IsNullOrEmpty(feedbackDto.Comment) || feedbackDto.UserId == Guid.Empty || feedbackDto.ClinicId == Guid.Empty)
                return BadRequest("Cant be empty!");
            try
            {
                Feedback feedback = new();
                feedback.Comment = feedbackDto.Comment;
                feedback.UserId = feedbackDto.UserId;
                feedback.ClinicId = feedbackDto.ClinicId;
                feedback.Id = Guid.NewGuid();
                feedback.IsActive = true;
                await _context.Feedback.AddAsync(feedback);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        [Route("api/delete-feedback/{id}"), Authorize]
        public async Task<IActionResult> DeleteFeedback(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    throw new InvalidOperationException("Cant be empty");
                else
                {
                    Feedback feedback = await _context.Feedback.FirstOrDefaultAsync(x => x.Id == id);
                    if (feedback is not null)
                    {
                        feedback.IsActive = false;
                        _context.Feedback.Update(feedback);
                        await _context.SaveChangesAsync();
                        return Ok();
                    }
                    else
                    {
                        return BadRequest("Cant find the feedback");
                    }
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        [Route("api/edit-feedback/{id}"), Authorize]
        public async Task<IActionResult> EditFeedback(Guid id, [FromBody] FeedbackDto feedbackDto)
        {
            try
            {
                if (id == Guid.Empty || string.IsNullOrEmpty(feedbackDto.Comment))
                {
                    return BadRequest("Cant be empty");
                }
                else
                {
                    var oldFeedback = await _context.Feedback.FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true);
                    if (oldFeedback != null)
                    {
                        oldFeedback.Comment = feedbackDto.Comment;
                        _context.Feedback.Update(oldFeedback);
                        await _context.SaveChangesAsync();
                        return Ok();
                    }
                    else
                    {
                        return BadRequest("Cant find old feedback");
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
