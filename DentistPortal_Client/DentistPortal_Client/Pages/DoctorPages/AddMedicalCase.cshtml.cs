using DentistPortal_API.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;

namespace DentistPortal_Client.Pages.DoctorPages
{
    public class AddMedicalCaseModel : PageModel
    {
        public string Token { get; set; }
        public MedicalCaseDto MedicalCase = new();
        public List<PictureDto> Pictures = new();
        IConfiguration config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .AddEnvironmentVariables()
               .Build();
        [TempData]
        public string Msg { get; set; } = String.Empty;
        [TempData]
        public string Status { get; set; } = String.Empty;

        public void OnGet(string token, MedicalCaseDto? medicalCase)
        {
            Token = token;
            MedicalCase = medicalCase;
        }

        public async Task<IActionResult> OnPost(MedicalCaseDto medicalCase, string token)
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            medicalCase.DoctorId = Guid.Parse(jwt.Claims.First().Value);
            var httpClient = HttpContext.RequestServices.GetService<IHttpClientFactory>();
            var client = httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            var jsonCategory = JsonSerializer.Serialize(medicalCase);
            var content = new StringContent(jsonCategory, Encoding.UTF8, "application/json");
            var request = await client.PostAsync("/api/create-medical-case", content);
            if (request.IsSuccessStatusCode)
            {
                Msg = "Successfully added medical case!";
                Status = "success";
                return RedirectToPage("/Home", new { Token });
            }
            else
            {
                Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Status = "error";
                return RedirectToPage("/DoctorPages/AddMedicalCase", new {token});
            }
        }
    }
}
