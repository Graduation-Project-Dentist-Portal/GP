using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
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
        private Cloudinary _cloudinary;

        public UserController(WebsiteDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            Account account = new Account(configuration.GetSection("CLOUDINARY_URL").GetSection("cloudinary_name").Value,
                              configuration.GetSection("CLOUDINARY_URL").GetSection("my_key").Value,
                              configuration.GetSection("CLOUDINARY_URL").GetSection("my_secret_key").Value);
            _cloudinary = new Cloudinary(account);
        }

        [HttpPost]
        [Route("api/login")]
        public async Task<IActionResult> Login([FromBody] UserDto user)
        {
            if (user.Username == "Admin")
            {
                var loggedUserAdmin = await _context.Admin.FirstOrDefaultAsync(x => x.IsActive == true && x.Username == user.Username);
                if (loggedUserAdmin != null)
                {
                    var hasherAdmin = new PasswordHasher<Admin>();
                    if (hasherAdmin.VerifyHashedPassword(loggedUserAdmin, loggedUserAdmin.PasswordHash, user.Password).Equals(PasswordVerificationResult.Success))
                    {
                        string token = await CreateToken(loggedUserAdmin.Id, "Admin", "Admin");
                        var refreshToken = await CreateRefreshToken();
                        await SetRefreshToken(refreshToken, loggedUserAdmin.Id);
                        return Ok(token);
                    }
                    else
                        return BadRequest("Wrong password!");
                }
                else
                    return BadRequest("User not found!");
            }
            else
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
                        string token = await CreateToken(loggedUserDentist.Id, "Dentist", loggedUserDentist.ProfilePicture);
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
                        string token = await CreateToken(loggedUserPatient.Id, "Patient", loggedUserPatient.ProfilePicture);
                        var refreshToken = await CreateRefreshToken();
                        await SetRefreshToken(refreshToken, loggedUserPatient.Id);
                        return Ok(token);
                    }
                    else
                        return BadRequest("Wrong password!");
                }
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

        [HttpPost]
        [Route("api/RegisterAsDoctor")]
        public async Task<IActionResult> RegisterAsDoctor([FromForm] DentistDto newDentist)
        {
            var loggedUserDentist = await _context.Dentist.FirstOrDefaultAsync(x => x.Username == newDentist.Username);
            var loggedUserPatient = await _context.Patient.FirstOrDefaultAsync(x => x.Username == newDentist.Username);
            if (string.IsNullOrEmpty(newDentist.Username) || string.IsNullOrEmpty(newDentist.PasswordHash) || string.IsNullOrEmpty(newDentist.LastName) || string.IsNullOrEmpty(newDentist.FirstName) || string.IsNullOrEmpty(newDentist.University))
            {
                return BadRequest("Cant be empty");
            }
            else if (loggedUserDentist != null || loggedUserPatient != null)
            {
                return BadRequest("Already used Username");
            }
            Dentist dentist = new Dentist();
            dentist.Id = Guid.NewGuid();
            dentist.IsActive = true;
            dentist.Username = newDentist.Username;
            dentist.FirstName = newDentist.FirstName;
            dentist.LastName = newDentist.LastName;
            var uploadResult = new ImageUploadResult();
            if (newDentist.ProfilePicture != null)
            {
                using (var stream = newDentist.ProfilePicture.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(newDentist.ProfilePicture.Name, stream)
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                    dentist.ProfilePicture = uploadResult.Uri.ToString();
                }
            }
            if (newDentist.IdentityCardPicture != null)
            {
                using (var stream = newDentist.IdentityCardPicture.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(newDentist.IdentityCardPicture.Name, stream)
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                    dentist.IdentityCardPicture = uploadResult.Uri.ToString();
                }
            }
            if (newDentist.UniversityCardPicture != null)
            {
                using (var stream = newDentist.UniversityCardPicture.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(newDentist.UniversityCardPicture.Name, stream)
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                    dentist.UniversityCardPicture = uploadResult.Uri.ToString();
                }
            }
            dentist.IsVerified = "false";
            dentist.Level = newDentist.Level;
            dentist.Graduated = newDentist.Graduated;
            dentist.University = newDentist.University;
            var hasher = new PasswordHasher<Dentist>();
            dentist.PasswordHash = hasher.HashPassword(dentist, newDentist.PasswordHash);
            await _context.Dentist.AddAsync(dentist);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        [Route("api/RegisterAsPatient")]
        public async Task<IActionResult> RegisterAsPatient([FromForm] PatientDTO newPatient)
        {
            var loggedUserDentist = await _context.Dentist.FirstOrDefaultAsync(x => x.Username == newPatient.Username);
            var loggedUserPatient = await _context.Patient.FirstOrDefaultAsync(x => x.Username == newPatient.Username);
            if (string.IsNullOrEmpty(newPatient.Username) || string.IsNullOrEmpty(newPatient.PasswordHash) || string.IsNullOrEmpty(newPatient.LastName) || string.IsNullOrEmpty(newPatient.FirstName))
            {
                return BadRequest("Cant be empty");
            }
            else if (loggedUserDentist != null || loggedUserPatient != null)
            {
                return BadRequest("Already used Username");
            }
            Patient patient = new Patient();
            patient.Id = Guid.NewGuid();
            patient.IsActive = true;
            patient.Username = newPatient.Username;
            patient.FirstName = newPatient.FirstName;
            patient.LastName = newPatient.LastName;
            var hasher = new PasswordHasher<Patient>();
            patient.PasswordHash = hasher.HashPassword(patient, newPatient.PasswordHash);
            var uploadResult = new ImageUploadResult();
            if (newPatient.ProfilePicture != null)
            {
                using (var stream = newPatient.ProfilePicture.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(newPatient.ProfilePicture.Name, stream)
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                    patient.ProfilePicture = uploadResult.Uri.ToString();
                }
            }
            await _context.Patient.AddAsync(patient);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        [Route("api/Profile")]
        public async Task<IActionResult> Profile([FromBody] string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return BadRequest("Cant be empty");
            }
            var GuId = Guid.Parse(Id);
            var loggedUserDentist = await _context.Dentist.FirstOrDefaultAsync(x => x.Id == GuId && x.IsActive == true);
            var loggedUserPatient = await _context.Patient.FirstOrDefaultAsync(x => x.Id == GuId && x.IsActive == true);
            if (loggedUserDentist == null && loggedUserPatient == null)
            {
                return BadRequest("User not found!");
            }
            else if (loggedUserDentist != null)
            {
                return Ok(loggedUserDentist);
            }
            return Ok(loggedUserPatient);
        }

        [HttpPost]
        [Route("api/EditPatientProfile")]
        public async Task<IActionResult> EditPatientProfile([FromBody] Patient updatedPatient)
        {
            if (updatedPatient == null)
            {
                return BadRequest("ERROR");
            }
            var loggedUserPatient = await _context.Patient.FirstOrDefaultAsync(x => x.Id == updatedPatient.Id && x.IsActive == true);

            if (loggedUserPatient == null)
            {
                return BadRequest("ERROR");
            }
            if (updatedPatient.Username == loggedUserPatient.Username) { }
            else
            {
                var loggedUserPatientSameUserName = await _context.Patient.FirstOrDefaultAsync(x => x.Username == updatedPatient.Username);
                var loggedUserDentistSameUserName = await _context.Dentist.FirstOrDefaultAsync(x => x.Username == updatedPatient.Username);
                if (loggedUserDentistSameUserName != null || loggedUserPatientSameUserName != null)
                {
                    return BadRequest("Already used Username");
                }
                loggedUserPatient.Username = updatedPatient.Username;
            }
            if (updatedPatient.FirstName == loggedUserPatient.FirstName) { }
            else
            {
                loggedUserPatient.FirstName = updatedPatient.FirstName;
            }
            if (updatedPatient.LastName == loggedUserPatient.LastName) { }
            else
            {
                loggedUserPatient.LastName = updatedPatient.LastName;
            }
            if (updatedPatient.PasswordHash == loggedUserPatient.PasswordHash)
            {
                await _context.SaveChangesAsync();
                return Ok();
            }
            else
            {
                var hasher = new PasswordHasher<Patient>();
                loggedUserPatient.PasswordHash = hasher.HashPassword(loggedUserPatient, updatedPatient.PasswordHash);
                await _context.SaveChangesAsync();
                return Ok();
            }
        }

        [HttpPost]
        [Route("api/EditDentistProfile")]
        public async Task<IActionResult> EditDentistProfile([FromBody] Dentist updatedDentist)
        {
            if (updatedDentist == null)
            {
                return BadRequest("ERROR");
            }
            var loggedUserDentist = await _context.Dentist.FirstOrDefaultAsync(x => x.Id == updatedDentist.Id && x.IsActive == true);

            if (loggedUserDentist == null)
            {
                return BadRequest("ERROR");
            }
            if (updatedDentist.Username == loggedUserDentist.Username) { }
            else
            {
                var loggedUserPatientSameUserName = await _context.Patient.FirstOrDefaultAsync(x => x.Username == updatedDentist.Username);
                var loggedUserDentistSameUserName = await _context.Dentist.FirstOrDefaultAsync(x => x.Username == updatedDentist.Username);
                if (loggedUserDentistSameUserName != null || loggedUserPatientSameUserName != null)
                {
                    return BadRequest("Already used Username");
                }
                loggedUserDentist.Username = updatedDentist.Username;
            }
            if (updatedDentist.FirstName == loggedUserDentist.FirstName) { }
            else
            {
                loggedUserDentist.FirstName = updatedDentist.FirstName;
            }
            if (updatedDentist.LastName == loggedUserDentist.LastName) { }
            else
            {
                loggedUserDentist.LastName = updatedDentist.LastName;
            }
            if (updatedDentist.Level == loggedUserDentist.Level) { }
            else
            {
                loggedUserDentist.Level = updatedDentist.Level;
            }
            if (updatedDentist.University == loggedUserDentist.University) { }
            else
            {
                loggedUserDentist.University = updatedDentist.University;
            }
            if (updatedDentist.Graduated == loggedUserDentist.Graduated) { }
            else
            {
                loggedUserDentist.Graduated = updatedDentist.Graduated;
            }
            if (updatedDentist.PasswordHash == loggedUserDentist.PasswordHash)
            {
                await _context.SaveChangesAsync();
                return Ok();
            }
            else
            {
                var hasher = new PasswordHasher<Dentist>();
                loggedUserDentist.PasswordHash = hasher.HashPassword(loggedUserDentist, updatedDentist.PasswordHash);
                await _context.SaveChangesAsync();
                return Ok();
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

        [HttpPost]
        [Route("api/ChangeImage")]
        public async Task<IActionResult> ChangeImage([FromForm] ChangeImageDto Obj)
        {
            var loggedDentist = await _context.Dentist.FirstOrDefaultAsync(x => x.Username == Obj.username && x.IsActive == true);
            var loggedPatient = await _context.Patient.FirstOrDefaultAsync(x => x.Username == Obj.username && x.IsActive == true);

            var uploadResult = new ImageUploadResult();
            if (loggedDentist == null)
            {
                if (Obj.ProfilePicture != null)
                {
                    using (var stream = Obj.ProfilePicture.OpenReadStream())
                    {
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(Obj.ProfilePicture.Name, stream)
                        };
                        uploadResult = _cloudinary.Upload(uploadParams);
                        loggedPatient.ProfilePicture = uploadResult.Uri.ToString();
                    }
                }
                await _context.SaveChangesAsync();
                return Ok();
            }
            if (Obj.UniversityCardPicture != null)
            {
                using (var stream = Obj.UniversityCardPicture.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(Obj.UniversityCardPicture.Name, stream)
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                    loggedDentist.UniversityCardPicture = uploadResult.Uri.ToString();
                }
            }
            if (Obj.IdentityCardPicture != null)
            {
                using (var stream = Obj.IdentityCardPicture.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(Obj.UniversityCardPicture.Name, stream)
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                    loggedDentist.UniversityCardPicture = uploadResult.Uri.ToString();
                }
            }
            if (Obj.ProfilePicture != null)
            {
                using (var stream = Obj.ProfilePicture.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(Obj.ProfilePicture.Name, stream)
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                    loggedDentist.ProfilePicture = uploadResult.Uri.ToString();
                }
            }
            await _context.SaveChangesAsync();
            return Ok();
        }


        [HttpPost]
        [Route("api/DentistProfileData")]
        public async Task<IActionResult> DentistProfileData([FromBody] string Id)
        {
            if (Id == null)
            {
                return BadRequest("Can not be empty");
            }
            var GuId = Guid.Parse(Id);
            var Dentist = await _context.Dentist.FirstOrDefaultAsync(x => x.Id == GuId && x.IsActive == true);
            if (Dentist == null)
            {
                return BadRequest("no doctor");
            }
            List<FinishedCases> Dentistcases = await _context.FinishedCases.Where(x => x.DoctorId == GuId).ToListAsync();
            object obj = new
            {
                dentist = Dentist,
                dentistcases = Dentistcases
            };
            return Ok(obj);
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

        private async Task SetRefreshToken(RefreshToken newRT, Guid id)
        {
            var cookiesOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRT.TimeExpires
            };
            var loggedUserDentist = await _context.Dentist.FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true);
            var loggedUserPatient = await _context.Patient.FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true);
            var loggedUserAdmin = await _context.Admin.FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true);
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
            else if (loggedUserPatient != null)
            {
                RefreshToken oldRefreshToken = await _context.RefreshToken.FirstOrDefaultAsync(x => x.Id == loggedUserPatient.RefreshTokenId);
                if (oldRefreshToken != null)
                {
                    oldRefreshToken.IsActive = false;
                    await _context.SaveChangesAsync();
                }
                loggedUserPatient.RefreshTokenId = newRT.Id;
            }
            else
            {
                RefreshToken oldRefreshToken = await _context.RefreshToken.FirstOrDefaultAsync(x => x.Id == loggedUserAdmin.RefreshTokenId);
                if (oldRefreshToken != null)
                {
                    oldRefreshToken.IsActive = false;
                    await _context.SaveChangesAsync();
                }
                loggedUserAdmin.RefreshTokenId = newRT.Id;
            }
            await _context.SaveChangesAsync();
        }

        private async Task<string> CreateToken(Guid id, string role, string profilePicture)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,id.ToString()),
                new Claim(ClaimTypes.Role,role.ToString()),
                new Claim(ClaimTypes.Uri,profilePicture.ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Token").Value));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: cred,
                issuer: "Graduation Project Team",
                audience: "https://localhost:7156/"
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        [HttpPost]
        [Route("api/refresh-token/{id}")]
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
                    var token = await CreateToken(loggedUser.Id, "Dentist", loggedUser.ProfilePicture);
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
        [Route("api/get-rt")]
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

