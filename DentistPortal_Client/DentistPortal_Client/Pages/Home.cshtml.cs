using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;

namespace DentistPortal_Client.Pages
{
    public class HomeModel : PageModel
    {
        [TempData]
        public string Msg { get; set; } = String.Empty;
        [TempData]
        public string Status { get; set; } = String.Empty;

        public void OnGet()
        {
        }
    }
}
