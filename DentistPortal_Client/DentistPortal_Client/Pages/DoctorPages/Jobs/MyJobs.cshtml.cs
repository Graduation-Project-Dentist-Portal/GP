using DentistPortal_API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace DentistPortal_Client.Pages.DoctorPages.Jobs
{
    public class MyJobsModel : PageModel
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
        public List<Job> Jobs = new();

        public MyJobsModel(IHttpClientFactory httpClient)
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
                var request = await client.GetStringAsync($"/api/My-Jobs/{id}");
                if (request is not null)
                {
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
                    };
                    Jobs = JsonSerializer.Deserialize<List<Job>>(request, options);
                }
                else
                {
                    Msg = request.ToString();
                    Status = "error";
                }
            }
            else
            {
                Response.Redirect($"https://localhost:7156/Login?url={"DoctorPages/Jobs/MyJobs"}");
            }

        }

    }
}
