using DentistPortal_API.Data;
using DentistPortal_API.DTO;
using DentistPortal_API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DentistPortal_API.Controllers
{
    public class UserController : Controller
    {
        private readonly WebsiteDbContext _context;
        private readonly IConfiguration _configuration;

        public UserController(WebsiteDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("api/login")]
        public async Task<IActionResult> Login([FromBody] UserDto user)
        {
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Cant be empty");
            }
            var loggedUserDentist = await _context.Dentist.FirstOrDefaultAsync(x => x.Username == user.Username && x.IsActive == true);
            var loggedUserPatient = await _context.Patient.FirstOrDefaultAsync(x => x.Username == user.Username && x.IsActive == true);
            if (loggedUserDentist == null && loggedUserPatient == null)
            {
                return BadRequest("User not found!");
            }
            if (loggedUserDentist != null)
            {
                var hasherDentist = new PasswordHasher<Dentist>();
                if (hasherDentist.VerifyHashedPassword(loggedUserDentist, loggedUserDentist.PasswordHash, user.Password).Equals(PasswordVerificationResult.Success))
                {
                    string token = await CreateToken(loggedUserDentist.Id, "Dentist");
                    var refreshToken = await CreateRefreshToken();
                    await SetRefreshToken(refreshToken, loggedUserDentist.Id);
                    return Ok(token);
                }
                else
                    return BadRequest("Wrong password!");
            }
            else
            {
                var hasherPatient = new PasswordHasher<Patient>();
                if (hasherPatient.VerifyHashedPassword(loggedUserPatient, loggedUserPatient.PasswordHash, user.Password).Equals(PasswordVerificationResult.Success))
                {
                    string token = await CreateToken(loggedUserPatient.Id, "Patient");
                    var refreshToken = await CreateRefreshToken();
                    await SetRefreshToken(refreshToken, loggedUserPatient.Id);
                    return Ok(token);
                }
                else
                    return BadRequest("Wrong password!");
            }
        }

        //[HttpPost]
        //[Route("api/create-user")]
        //public async Task<ActionResult> Register([FromBody] UserDto newUser)
        //{
        //    User user = await _context.User.FirstOrDefaultAsync(x => x.Username == newUser.Username);
        //    if (string.IsNullOrEmpty(newUser.Username) || string.IsNullOrEmpty(newUser.Password) || string.IsNullOrEmpty(newUser.FirstName) || string.IsNullOrEmpty(newUser.LastName) || string.IsNullOrEmpty(newUser.Role) || (newUser.Role != "Doctor" && newUser.Role != "Student" && newUser.Role != "Patient"))
        //        return BadRequest("Cant be empty");
        //    else if (user is not null)
        //    {
        //        return BadRequest("Username already taken!");
        //    }
        //    else
        //    {
        //        user = new();
        //        user.Id = Guid.NewGuid();
        //        user.IsActive = true;
        //        var hasher = new PasswordHasher<User>();
        //        user.PasswordHash = hasher.HashPassword(user, newUser.Password);
        //        user.FirstName = newUser.FirstName;
        //        user.LastName = newUser.LastName;
        //        user.Role = newUser.Role;
        //        user.Username = newUser.Username;
        //        user.ProfilePicture = newUser.ProfilePicture;
        //        await _context.User.AddAsync(user);
        //        await _context.SaveChangesAsync();
        //        return Ok();
        //    }
        //}

        private async Task<RefreshToken> CreateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                TimeExpires = DateTime.Now.AddDays(1),
                TimeCreated = DateTime.Now,
                Id = Guid.NewGuid(),
                IsActive = true
            };
            await _context.RefreshToken.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
            return refreshToken;
        }

        private async Task SetRefreshToken(RefreshToken newRT, Guid id)
        {
            var cookiesOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRT.TimeExpires
            };
            var loggedUserDentist = await _context.Dentist.FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true);
            var loggedUserPatient = await _context.Patient.FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true);
            if (loggedUserDentist != null)
            {
                RefreshToken oldRefreshToken = await _context.RefreshToken.FirstOrDefaultAsync(x => x.Id == loggedUserDentist.RefreshTokenId);
                if (oldRefreshToken != null)
                {
                    oldRefreshToken.IsActive = false;
                    await _context.SaveChangesAsync();
                }
                loggedUserDentist.RefreshTokenId = newRT.Id;
            }
            else
            {
                RefreshToken oldRefreshToken = await _context.RefreshToken.FirstOrDefaultAsync(x => x.Id == loggedUserPatient.RefreshTokenId);
                if (oldRefreshToken != null)
                {
                    oldRefreshToken.IsActive = false;
                    await _context.SaveChangesAsync();
                }
                loggedUserPatient.RefreshTokenId = newRT.Id;
            }
            await _context.SaveChangesAsync();
        }

        private async Task<string> CreateToken(Guid id, string role)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,id.ToString()),
                new Claim(ClaimTypes.Role,role.ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Token").Value));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(3),
                signingCredentials: cred,
                issuer: "Graduation Project Team",
                audience: "https://localhost:7156/"
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        [HttpPost]
        [Route("api/refresh-token/{id}"), Authorize]
        public async Task<ActionResult<string>> RefreshToken([FromBody] string rT, Guid id)
        {
            try
            {
                RefreshToken refreshToken = await _context.RefreshToken.FirstOrDefaultAsync(x => x.Token.Equals(rT) && x.IsActive == true);
                var loggedUser = await _context.Dentist.FirstOrDefaultAsync(user => user.Id == id && user.IsActive == true);
                RefreshToken userRefreshToken = await _context.RefreshToken.FirstOrDefaultAsync(x => x.Id == loggedUser.RefreshTokenId && x.IsActive == true);
                if (!loggedUser.RefreshTokenId.Equals(refreshToken.Id))
                    return Unauthorized("Invalid refresh token");
                else if (userRefreshToken.TimeExpires < DateTime.Now)
                    return Unauthorized("Token expired");
                else
                {
                    var token = await CreateToken(loggedUser.Id, "Dentist");
                    var newRT = await CreateRefreshToken();
                    await SetRefreshToken(newRT, loggedUser.Id);
                    return Ok(token);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("api/get-rt"), Authorize]
        public async Task<ActionResult<RefreshToken>> GetRefreshToken([FromBody] Guid id)
        {
            try
            {
                var hasher = new PasswordHasher<Dentist>();
                var loggedUser = await _context.Dentist.FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true);
                if (loggedUser == null)
                    return BadRequest("User not found!");
                RefreshToken refreshToken = await _context.RefreshToken.FirstOrDefaultAsync(x => x.Id == loggedUser.RefreshTokenId && x.IsActive == true);
                return Ok(refreshToken.Token);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}

