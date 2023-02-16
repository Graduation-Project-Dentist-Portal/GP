using DentistPortal_API.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;

namespace DentistPortal_Client.Pages
{
    public class RegisterModel : PageModel
    {
        public UserDto User { get; set; } = new();
        IConfiguration config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .AddEnvironmentVariables()
               .Build();
        [TempData]
        public string Msg { get; set; }
        [TempData]
        public string Status { get; set; }

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
            var request = await client.PostAsync("/api/create-user", content);
            if (request.IsSuccessStatusCode)
            {
                Msg = "Successfully Created!";
                Status = "success";
                return RedirectToPage("/Home");
            }
            else
            {
                Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Status = "error";
                return RedirectToPage("/Register");
            }
        }
    }
}
