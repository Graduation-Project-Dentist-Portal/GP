using DentistPortal_API.Data;
using DentistPortal_API.DTO;
using DentistPortal_API.Model;
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
            var loggedUser = await _context.User.FirstOrDefaultAsync(x => x.Username == user.Username && x.IsActive == true);
            var hasher = new PasswordHasher<User>();
            if (loggedUser == null)
                return BadRequest("User not found!");
            if (hasher.VerifyHashedPassword(loggedUser, loggedUser.PasswordHash, user.Password).Equals(PasswordVerificationResult.Success))
            {
                string token = await CreateToken(loggedUser.Id);
                var refreshToken = await CreateRefreshToken();
                await SetRefreshToken(refreshToken, loggedUser.Id);
                return Ok(token);
            }
            else
                return BadRequest("Wrong password!");
        }

        [HttpPost]
        [Route("api/create-user")]
        public async Task<ActionResult> Register([FromBody] User newUser)
        {
            User user = await _context.User.FirstOrDefaultAsync(x => x.Username == newUser.Username);
            if (string.IsNullOrEmpty(newUser.Username) || string.IsNullOrEmpty(newUser.PasswordHash))
                return BadRequest("Cant be empty");
            else if (user is not null)
            {
                return BadRequest("Username already taken!");
            }
            else
            {
                newUser.Id = Guid.NewGuid();
                newUser.IsActive = true;
                var hasher = new PasswordHasher<User>();
                newUser.PasswordHash = hasher.HashPassword(newUser, newUser.PasswordHash);
                await _context.User.AddAsync(newUser);
                await _context.SaveChangesAsync();
                return Ok();
            }
        }

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

        async private Task SetRefreshToken(RefreshToken newRT, Guid id)
        {
            var cookiesOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRT.TimeExpires
            };
            User loggedUser = await _context.User.FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true);
            RefreshToken oldRefreshToken = await _context.RefreshToken.FirstOrDefaultAsync(x => x.Id == loggedUser.RefreshTokenId);
            if (oldRefreshToken != null)
            {
                oldRefreshToken.IsActive = false;
                await _context.SaveChangesAsync();
            }
            loggedUser.RefreshTokenId = newRT.Id;
            await _context.SaveChangesAsync();
        }

        private async Task<string> CreateToken(Guid id)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,id.ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Token").Value));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(3),
                signingCredentials: cred,
                issuer: "Graduation Project Team",
                audience: "https://localhost:7264"
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}

