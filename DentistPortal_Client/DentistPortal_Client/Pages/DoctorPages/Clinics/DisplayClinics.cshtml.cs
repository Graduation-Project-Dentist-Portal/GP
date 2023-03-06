using DentistPortal_Client.DTO;
using DentistPortal_Client.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace DentistPortal_Client.Pages.DoctorPages.Clinics
{
    public class DisplayClinicsModel : PageModel
    {
        private IHttpClientFactory _httpClient;
        public Guid DoctorId;
        public IConfiguration config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .AddEnvironmentVariables()
               .Build();
        [TempData]
        public string Msg { get; set; } = String.Empty;
        [TempData]
        public string Status { get; set; } = String.Empty;
        public List<Clinic> Clinics = new();
        public string[] Pictures { get; set; }

        public DisplayClinicsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory;
        }

        public async Task OnGet()
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(HttpContext.Session.GetString("Token"));
            DoctorId = Guid.Parse(jwt.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            try
            {
                var request = await client.GetStringAsync("/api/get-clinics");
                if (request is not null)
                {
                    if (request.Length > 0)
                    {
                        var options = new JsonSerializerOptions
                        {
                            WriteIndented = true,
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
                        };
                        Clinics = JsonSerializer.Deserialize<List<Clinic>>(request, options);
                    }
                }
                else
                {
                    Msg = request.ToString();
                    Status = "error";
                }
            }
            catch (Exception e)
            {
                Msg = e.Message;
                Status = "error";
            }
        }

        public async Task<IActionResult> OnPost(ClinicDto clinicDto, List<IFormFile> files)
        {
            if (clinicDto.OpenTime <= 0 || clinicDto.OpenTime >= 12 || clinicDto.OpenTime != (int)clinicDto.OpenTime || clinicDto.CloseTime <= 0 || clinicDto.CloseTime >= 12 || clinicDto.CloseTime != (int)clinicDto.CloseTime)
            {
                Msg = "Wrong input for Clinic Open/Close Time!";
                Status = "error";
                return RedirectToPage("DisplayClinics");
            }
            if (files.Count > 0)
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
                            clinicDto.CasePictures.Add(s);
                        }
                    }
                }
            }
            var token = HttpContext.Session.GetString("Token");
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            clinicDto.DoctorId = Guid.Parse(jwt.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var jsonCategory = JsonSerializer.Serialize(clinicDto);
            var content = new StringContent(jsonCategory, Encoding.UTF8, "application/json");
            var request = await client.PostAsync("/api/create-clinic", content);
            if (request.IsSuccessStatusCode)
            {
                Msg = "Successfully added clinic!";
                Status = "success";
                return RedirectToPage("DisplayClinics");
            }
            else
            {
                Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Status = "error";
                return RedirectToPage("DisplayClinics");
            }
        }
    }
}
