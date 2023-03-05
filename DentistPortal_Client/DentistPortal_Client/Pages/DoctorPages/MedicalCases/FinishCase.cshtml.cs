using DentistPortal_Client.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;

namespace DentistPortal_Client.Pages.DoctorPages.MedicalCases
{
    public class FinishCaseModel : PageModel
    {
        private IHttpClientFactory _httpClient;
        public IConfiguration config = new ConfigurationBuilder()
                       .AddJsonFile("appsettings.json")
                       .AddEnvironmentVariables()
                       .Build();
        [TempData]
        public string Msg { get; set; } = String.Empty;
        [TempData]
        public string Status { get; set; } = String.Empty;
        public FinishedCaseDto FinishedCase { get; set; }
        public IFormFile Before { get; set; }
        public IFormFile After { get; set; }

        public FinishCaseModel(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }

        public void OnGet(Guid id, FinishedCaseDto? finishedCaseDto)
        {
            if (!string.IsNullOrEmpty(finishedCaseDto.DoctorWork))
            {
                FinishedCase = finishedCaseDto;
                FinishedCase.CaseId = finishedCaseDto.CaseId;
            }
            else
            {
                FinishedCase = new();
                FinishedCase.CaseId = id;
            }
        }

        public async Task<IActionResult> OnPost(FinishedCaseDto finishedCaseDto, IFormFile before, IFormFile after)
        {
            if (before is not null || after is not null)
            {
                using (var ms = new MemoryStream())
                {
                    await before.CopyToAsync(ms);
                    var fileBytes = ms.ToArray();
                    finishedCaseDto.BeforePicture = Convert.ToBase64String(fileBytes);
                    // act on the Base64 data
                }
                using (var ms = new MemoryStream())
                {
                    await after.CopyToAsync(ms);
                    var fileBytes = ms.ToArray();
                    finishedCaseDto.AfterPicture = Convert.ToBase64String(fileBytes);
                    // act on the Base64 data
                }
            }
            var token = HttpContext.Session.GetString("Token");
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            finishedCaseDto.DoctorId = Guid.Parse(jwt.Claims.First().Value);
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var jsonCategory = JsonSerializer.Serialize(finishedCaseDto);
            var content = new StringContent(jsonCategory, Encoding.UTF8, "application/json");
            var request = await client.PostAsync("/api/finish-medical-case", content);
            if (request.IsSuccessStatusCode)
            {
                Msg = "Successfully finished!";
                Status = "success";
                return RedirectToPage("/DoctorPages/MedicalCases/MyMedicalCases");
            }
            else
            {
                Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Status = "error";
                return RedirectToPage("", finishedCaseDto);
            }
        }
    }
}
