using CurrieTechnologies.Razor.SweetAlert2;
using DentistPortal_API.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace DentistPortal_Client.Pages
{
    public class LoginModel : PageModel
    {
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
            var httpClient = HttpContext.RequestServices.GetService<IHttpClientFactory>();
            var client = httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            var jsonCategory = JsonSerializer.Serialize(user);
            var content = new StringContent(jsonCategory, Encoding.UTF8, "application/json");
            var request = await client.PostAsync("/api/login", content);
            if (request.IsSuccessStatusCode)
            {
                Msg = "Successfully logged in!";
                Status = "success";
                string token = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return RedirectToPage("/Home", new { token });
            }
            else
            {
                Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Status = "error";
                return RedirectToPage("/Login");
            }
        }
    }
}
