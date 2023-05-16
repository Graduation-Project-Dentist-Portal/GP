using DentistPortal_API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json;

namespace DentistPortal_Client.Pages.DoctorPages.MedicalCases
{
    public class MyMedicalCasesModel : PageModel
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
        public List<MedicalCase> MedicalCases = new();
        public string[] Pictures { get; set; }


        public MyMedicalCasesModel(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task OnGet()
        {
            if (HttpContext.Session.GetString("role") == "Dentist")
            {
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(HttpContext.Session.GetString("Token"));
                var id = Guid.Parse(jwt.Claims.First().Value);
                var client = _httpClient.CreateClient();
                client.BaseAddress = new Uri(config["BaseAddress"]);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                try
                {
                    var request = await client.GetStringAsync($"/api/my-medical-cases/{id}");
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
                catch (HttpRequestException ex)
                {
                    if (ex.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        LoginModel loginModel = new LoginModel(_httpClient);
                        await loginModel.GetNewToken(HttpContext);
                        await OnGet();
                    }
                }
                catch (Exception e)
                {
                    Msg = e.Message;
                    Status = "error";
                }
            }
            else
            {
                Response.Redirect($"https://localhost:7156/Login?url={"DoctorPages/MedicalCases/MyMedicalCases"}");
                Response.WriteAsync("redirecting......");
            }
        }

        public async Task<IActionResult> OnPostCancel(Guid id)
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(HttpContext.Session.GetString("Token"));
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            var request = await client.PutAsJsonAsync($"/api/leave-medical-case", id);
            if (request.IsSuccessStatusCode)
            {
                Msg = "Canceled successfully";
                Status = "success";
                return RedirectToPage("");
            }
            else
            {
                Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Status = "error";
                return RedirectToPage("");
            }
        }
    }
}
