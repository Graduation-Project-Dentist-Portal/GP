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
                return Ok(await _context.Feedback.Where(x => x.ClinicId == id && x.IsActive == true).OrderByDescending(X => X.Likes).Include(x => x.Patient).ToListAsync());
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
            if (string.IsNullOrEmpty(feedbackDto.Comment) || feedbackDto.UserId == Guid.Empty || feedbackDto.ClinicId == Guid.Empty || string.IsNullOrEmpty(feedbackDto.AiScore) || (feedbackDto.AiScore != "0" && feedbackDto.AiScore != "1" && feedbackDto.AiScore != "-1"))
                return BadRequest("Cant be empty!");
            try
            {
                Feedback feedback = new();
                feedback.Comment = feedbackDto.Comment;
                feedback.UserId = feedbackDto.UserId;
                feedback.ClinicId = feedbackDto.ClinicId;
                feedback.Id = Guid.NewGuid();
                feedback.IsActive = true;
                feedback.Likes = 0;
                feedback.Patient = null;
                feedback.AiScore = feedbackDto.AiScore;
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
                    Feedback feedback = await _context.Feedback.FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true);
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

        [HttpPost]
        [Route("api/like"), Authorize]
        public async Task<IActionResult> Like([FromBody] LikeDto likeDto)
        {
            try
            {
                if (likeDto.FeedbackId == Guid.Empty || likeDto.PatientId == Guid.Empty)
                    return BadRequest("Cant be empty");
                Like like = new();
                like.Id = Guid.NewGuid();
                like.PatientId = likeDto.PatientId;
                like.FeedbackId = likeDto.FeedbackId;
                like.IsActive = true;
                var feedback = await _context.Feedback.FirstOrDefaultAsync(x => x.Id == likeDto.FeedbackId && x.IsActive == true);
                if (feedback is null)
                    return BadRequest("Cant find feedback");
                feedback.Likes++;
                _context.Feedback.Update(feedback);
                await _context.Like.AddAsync(like);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("api/un-like"), Authorize]
        public async Task<IActionResult> UnLike([FromBody] LikeDto likeDto)
        {
            try
            {
                if (likeDto.FeedbackId == Guid.Empty || likeDto.PatientId == Guid.Empty)
                    return BadRequest("Cant be empty");
                var feedback = await _context.Feedback.FirstOrDefaultAsync(x => x.Id == likeDto.FeedbackId && x.IsActive == true); ;
                if (feedback is null)
                    return BadRequest("Cant find feedback");
                var like = await _context.Like.FirstOrDefaultAsync(x => x.FeedbackId == likeDto.FeedbackId && x.PatientId == likeDto.PatientId && x.IsActive == true);
                if (like is null)
                    return BadRequest("Cant find like");
                feedback.Likes--;
                like.IsActive = false;
                _context.Feedback.Update(feedback);
                _context.Like.Update(like);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("api/get-likes/{id}"), Authorize]
        public async Task<IActionResult> GetLikes(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Cant be empty");
            try
            {
                return Ok(await _context.Like.CountAsync(x => x.IsActive == true && x.FeedbackId == id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("api/is-liked/{feedbackId}/{PatientId}"), Authorize]
        public async Task<IActionResult> IsLiked(Guid feedbackId, Guid PatientId)
        {
            try
            {
                if (PatientId == Guid.Empty || feedbackId == Guid.Empty)
                    return BadRequest("Cant be empty");
                var like = await _context.Like.FirstOrDefaultAsync(x => x.IsActive == true && x.FeedbackId == feedbackId && x.PatientId == PatientId);
                if (like == null)
                    return Ok(false);
                else
                    return Ok(true);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
