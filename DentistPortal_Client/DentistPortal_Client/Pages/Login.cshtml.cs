using CurrieTechnologies.Razor.SweetAlert2;
using DentistPortal_Client.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Hangfire;
using Hangfire.Common;

namespace DentistPortal_Client.Pages
{
    public class LoginModel : PageModel
    {
        private IHttpClientFactory _httpClient;

        public LoginModel(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }
        public UserDto User = new();
        IConfiguration config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .AddEnvironmentVariables()
               .Build();
        [TempData]
        public string Msg { get; set; } = String.Empty;
        [TempData]
        public string Status { get; set; } = String.Empty;

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(UserDto user)
        {
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            var request = await client.PostAsJsonAsync("/api/login", user);
            if (request.IsSuccessStatusCode)
            {
                Msg = "Successfully logged in!";
                Status = "success";
                string token = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
                var role = jwt.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role).Value;

                HttpContext.Session.SetString("Token", token);
                HttpContext.Session.SetString("username", user.Username);
                HttpContext.Session.SetString("token", token);
                HttpContext.Session.SetString("role", role);
                //var timer = new System.Threading.Timer(async (e) =>
                //{
                //    await GetNewToken(_h);
                //}, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
                return RedirectToPage("/Home");
            }
            else
            {
                Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Status = "error";
                return RedirectToPage("/Login");
            }
        }

        public async Task GetNewToken(HttpContext context)
        {
            var token = context.Session.GetString("Token");
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var id = Guid.Parse(jwt.Claims.First().Value);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var req = await client.PostAsJsonAsync("/api/get-rt", id);
            if (req.IsSuccessStatusCode)
            {
                var rT = req.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var newTokenRequest = await client.PostAsJsonAsync($"api/refresh-token/{id}", rT);
                if (newTokenRequest.IsSuccessStatusCode)
                {
                    string newToken = newTokenRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    context.Session.Clear();
                    context.Session.SetString("Token", newToken);
                }
                else
                {
                    Msg = newTokenRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    Status = "error";
                    RedirectToPage("");
                }
            }
            else
            {
                Msg = req.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Status = "error";
                RedirectToPage("");
            }
        }
    }
}
