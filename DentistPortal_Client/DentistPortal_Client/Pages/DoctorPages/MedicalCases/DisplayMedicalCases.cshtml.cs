using DentistPortal_Client.DTO;
using DentistPortal_API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace DentistPortal_Client.Pages.DoctorPages
{
    public class DisplayMedicalCasesModel : PageModel
    {
        private IHttpClientFactory _httpClient;
        public Guid DoctorId;
        public Guid SelectedId=new();

        public DisplayMedicalCasesModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory;
        }

        public IConfiguration config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .AddEnvironmentVariables()
               .Build();
        [TempData]
        public string Msg { get; set; } = String.Empty;
        [TempData]
        public string Status { get; set; } = String.Empty;
        public List<MedicalCase> MedicalCases = new();
        public string[] Pictures { get; set; }
        public async Task OnGet()
        {
            LoginModel model = new LoginModel(_httpClient);
            await model.GetNewToken(HttpContext.Session.GetString("Token"), HttpContext);
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(HttpContext.Session.GetString("Token"));
            DoctorId = Guid.Parse(jwt.Claims.First().Value);
            //var httpClient = HttpContext.RequestServices.GetService<IHttpClientFactory>();
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            var request = await client.GetStringAsync("/api/display-medical-cases");
            if (request is not null)
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
                };
                MedicalCases = JsonSerializer.Deserialize<List<MedicalCase>>(request, options);
            }
            else
            {
                Msg = request.ToString();
                Status = "error";
            }
        }

        public async Task<IActionResult> OnPost(MedicalCaseDto medicalCase, List<IFormFile> files)
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
                        medicalCase.CasePictures.Add(s);
                    }
                }
            }
            var token = HttpContext.Session.GetString("Token");
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            medicalCase.DoctorId = Guid.Parse(jwt.Claims.First().Value);
            //var httpClient = HttpContext.RequestServices.GetService<IHttpClientFactory>();
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var jsonCategory = JsonSerializer.Serialize(medicalCase);
            var content = new StringContent(jsonCategory, Encoding.UTF8, "application/json");
            var request = await client.PostAsync("/api/create-medical-case", content);
            if (request.IsSuccessStatusCode)
            {
                Msg = "Successfully added medical case!";
                Status = "success";
                return RedirectToPage("/DoctorPages/MedicalCases/DisplayMedicalCases");
            }
            else
            {
                Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Status = "error";
                return RedirectToPage("/DoctorPages/MedicalCases/DisplayMedicalCases");
            }
        }
    }
}
