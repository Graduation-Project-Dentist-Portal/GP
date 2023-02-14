using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DentistPortal_Client.Pages
{
    public class HomeModel : PageModel
    {
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
    }
}
