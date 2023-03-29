using DentistPortal_API.Data;
using DentistPortal_API.DTO;
using DentistPortal_API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentistPortal_API.Controllers
{
    public class ToolController : Controller
    {
        private readonly WebsiteDbContext _context;

        public ToolController(WebsiteDbContext context)
        {
            _context = context;

        }

        [HttpPost]
        [Route("api/create-tool"), Authorize]
        public async Task<IActionResult> AddTool([FromBody] ToolDto toolDto)
        {
            if (!await IsValid(toolDto) || toolDto.PicturePaths.Count == 0)
                return BadRequest("Cant be empty!");
            try
            {
                var tool = new Tool();
                tool.Dentist = null;
                tool.Id = Guid.NewGuid();
                tool.ToolName = toolDto.ToolName;
                tool.Description = toolDto.Description;
                tool.ToolPrice = toolDto.ToolPrice;
                tool.SellerIdDoctor = toolDto.SellerIdDoctor;
                tool.ToolStatus = "Available";
                tool.SellerLocation = toolDto.SellerLocation;
                tool.ContactNumber = toolDto.ContactNumber;
                foreach (var x in toolDto.PicturePaths)
                {
                    if (x != toolDto.PicturePaths.Last())
                        tool.PicturePaths += x + ",";
                    else
                        tool.PicturePaths += x;
                }
                tool.IsActive = true;
                await _context.Tool.AddAsync(tool);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("api/display-tools"), Authorize]
        public async Task<ActionResult<List<Tool>>> DisplayTools()
        {
            try
            {
                if (await _context.Tool.CountAsync() == 0)
                    return Ok();
                return Ok(await _context.Tool
                    .Where(c => c.IsActive == true && c.ToolStatus == "Available").Include(en => en.Dentist)
                    .ToListAsync());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("api/display-my-tools/{id}"), Authorize]
        public async Task<ActionResult<List<Tool>>> DisplayMyTools(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Id cant be empty!");
            try
            {
                return Ok(await _context.Tool
                    .Where(c => c.IsActive == true && c.SellerIdDoctor == id).Include(en => en.Dentist)
                    .ToListAsync());

            }
            catch
            {
                return BadRequest("There are no Tools to show");
            }
        }

        [HttpGet]
        [Route("api/get-tool/{id}"), Authorize]
        public async Task<IActionResult> GetTool(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Id cant be empty!");
            try
            {
                var tool = await _context.Tool.FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true);
                if (tool is not null)
                {
                    return Ok(tool);
                }
                else { return BadRequest("no tool"); }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("api/delete-tool/{id}"), Authorize]
        public async Task<IActionResult> DeleteTool(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Id cant be empty!");
            var tool = await _context.Tool.FirstOrDefaultAsync(c => c.Id == id && c.IsActive == true);
            if (tool is null)
            {
                return BadRequest("Cant find the tool you want to delete!");
            }
            tool.IsActive = false;
            _context.Tool.Update(tool);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut]
        [Route("api/edit-tool/{id}"), Authorize]
        public async Task<IActionResult> EditToolForSale(Guid id, [FromBody] ToolDto toolDto)
        {
            if (id == Guid.Empty || !await IsValid(toolDto) || string.IsNullOrEmpty(toolDto.ToolStatus))
                return BadRequest("Cant be empty!");
            try
            {
                var tool = await _context.Tool.FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true);
                if (tool is not null)
                {
                    tool.ToolName = toolDto.ToolName;
                    tool.Description = toolDto.Description;
                    tool.ToolPrice = toolDto.ToolPrice;
                    tool.SellerLocation = toolDto.SellerLocation;
                    tool.ContactNumber = toolDto.ContactNumber;
                    tool.ToolStatus = toolDto.ToolStatus;
                    if (toolDto.PicturePaths.Count > 0)
                    {
                        tool.PicturePaths = string.Empty;
                        foreach (var x in toolDto.PicturePaths)
                        {
                            if (x != toolDto.PicturePaths.Last())
                                tool.PicturePaths += x + ",";
                            else
                                tool.PicturePaths += x;
                        }
                    }
                    _context.Tool.Update(tool);
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                else
                    return BadRequest("Cant find tool");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private async Task<bool> IsValid(ToolDto toolDto)
        {
            if (string.IsNullOrEmpty(toolDto.ToolName) ||
                string.IsNullOrEmpty(toolDto.Description) ||
                string.IsNullOrEmpty(toolDto.SellerLocation) ||
                string.IsNullOrEmpty(toolDto.ContactNumber) ||
                toolDto.ToolPrice == 0 ||
                toolDto.SellerIdDoctor == Guid.Empty)
                return false;
            else
                return true;
        }
    }
}
