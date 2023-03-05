using DentistPortal_API.Model;
using DentistPortal_Client.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;

namespace DentistPortal_Client.Pages.DoctorPages.MedicalCases
{
    public class EditDeleteMedicalCaseModel : PageModel
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
        public MedicalCase MedicalCase = new();
        public string[] Pictures { get; set; }

        public EditDeleteMedicalCaseModel(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task OnGet(Guid id)
        {
            //LoginModel model = new LoginModel(_httpClient);
            //await model.GetNewToken(HttpContext.Session.GetString("Token"), HttpContext);
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(HttpContext.Session.GetString("Token"));
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            var request = await client.GetStringAsync($"/api/get-medical-case/{id}");
            if (request is not null)
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
                };
                MedicalCase = JsonSerializer.Deserialize<MedicalCase>(request, options);
            }
            else
            {
                Msg = request.ToString();
                Status = "error";
            }
        }

        public async Task<IActionResult> OnPostDelete(Guid id)
        {
            var token = HttpContext.Session.GetString("Token");
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var request = await client.DeleteAsync($"api/delete-medical-case/{id}");
            if (request.IsSuccessStatusCode)
            {
                Msg = "Deleted successfully!";
                Status = "success";
                return RedirectToPage("/DoctorPages/MedicalCases/DisplayMedicalCases");
            }
            else
            {
                Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Status = "error";
                return RedirectToPage("");
            }
        }
        public async Task<IActionResult> OnPostEdit(MedicalCaseDto medicalCaseDto, List<IFormFile>? files, Guid id)
        {
            if (medicalCaseDto.PatientAge <= 0 || medicalCaseDto.PatientAge >= 100 || medicalCaseDto.PatientAge != (int)medicalCaseDto.PatientAge)
            {
                Msg = "Wrong input for Patient Age!";
                Status = "error";
                return RedirectToPage("", new { id });
            }
            if (files != null)
            {
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await file.CopyToAsync(ms);
                            var fileBytes = ms.ToArray();
                            string s = Convert.ToBase64String(fileBytes);
                            // act on the Base64 data
                            medicalCaseDto.CasePictures.Add(s);
                        }
                    }
                }
            }

            var token = HttpContext.Session.GetString("Token");
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var jsonCategory = JsonSerializer.Serialize(medicalCaseDto);
            var content = new StringContent(jsonCategory, Encoding.UTF8, "application/json");
            var request = await client.PutAsync($"api/edit-medical-case/{id}", content);
            if (request.IsSuccessStatusCode)
            {
                Msg = "Edited successfully!";
                Status = "success";
                return RedirectToPage("/DoctorPages/MedicalCases/DisplayMedicalCases");
            }
            else
            {
                Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Status = "error";
                return RedirectToPage("", new { id });
            }
        }
    }
}
