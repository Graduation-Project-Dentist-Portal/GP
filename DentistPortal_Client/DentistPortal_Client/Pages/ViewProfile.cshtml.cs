using DentistPortal_API.Model;
using DentistPortal_Client.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DentistPortal_Client.Pages
{
    public class ViewProfileModel : PageModel
    {
        public ProfileDataDto profileData { get; set; } = new ProfileDataDto();


        IConfiguration config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .AddEnvironmentVariables()
               .Build();
        public async Task OnGet()
        {      
            try
            {
                var token = HttpContext.Session.GetString("token");
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
                var Id = jwt.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
                var httpClient = HttpContext.RequestServices.GetService<IHttpClientFactory>();
                var client = httpClient.CreateClient();
                client.BaseAddress = new Uri(config["BaseAddress"]);
                var jsoncategory = JsonConvert.SerializeObject(Id);
                var content = new StringContent(jsoncategory, Encoding.UTF8, "application/json");
                var request = await client.PostAsync("/api/DentistProfileData", content);

                if (request.IsSuccessStatusCode)
                {
                    var stringdata = request.Content.ReadAsStringAsync().Result;
                    profileData = JsonConvert.DeserializeObject<ProfileDataDto>(stringdata);
                }
            }
            catch (Exception ex) 
            {
                var st = new StackTrace(ex, true);
                var frame = st.GetFrame(0);
                var line = frame.GetFileLineNumber();

                Response.WriteAsync("something went wrong please try again");
            }
        }
    }
}
