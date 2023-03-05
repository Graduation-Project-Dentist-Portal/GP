using CurrieTechnologies.Razor.SweetAlert2;
using DentistPortal_Client.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Blazored.LocalStorage;
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
            //var httpClient = HttpContext.RequestServices.GetService<IHttpClientFactory>();
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            var request = await client.PostAsJsonAsync("/api/login", user);
            if (request.IsSuccessStatusCode)
            {
                Msg = "Successfully logged in!";
                Status = "success";
                string token = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
                HttpContext.Session.SetString("Token", token);
                //_token = token;
                //var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
                //var id = Guid.Parse(jwt.Claims.First().Value);
                //var timer = new Timer(async (e) =>
                //{
                //    await GetNewToken(id.ToString(), _token);
                //}, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
                return RedirectToPage("/Home");
            }
            else
            {
                Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Status = "error";
                return RedirectToPage("/Login");
            }
        }

        public async Task GetNewToken(string token, HttpContext context)
        {
            //var httpClient = HttpContext.RequestServices.GetService<IHttpClientFactory>();
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var id = Guid.Parse(jwt.Claims.First().Value);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var req = await client.PostAsJsonAsync("/api/get-rt", id);
            if (req.IsSuccessStatusCode)
            {
                var rT = req.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                string idString = id.ToString().Replace("\"", "");
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
