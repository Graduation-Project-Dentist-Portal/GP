using DentistPortal_API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json;

namespace DentistPortal_Client.Pages.AdminPages
{
    public class VerifyPageModel : PageModel
    {
        private IHttpClientFactory _httpClient;
        public IConfiguration config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .AddEnvironmentVariables()
               .Build();
        [TempData]
        public string Msg { get; set; } = string.Empty;
        [TempData]
        public string Status { get; set; } = string.Empty;
        public List<Dentist> Dentists = new();

        public VerifyPageModel(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task OnGet()
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(HttpContext.Session.GetString("Token"));
            //DoctorId = Guid.Parse(jwt.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            try
            {
                var request = await client.GetStringAsync("/api/get-unverified-users");
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
                        Dentists = JsonSerializer.Deserialize<List<Dentist>>(request, options);
                    }
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
    }
}
