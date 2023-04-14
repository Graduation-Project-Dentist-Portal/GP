using DentistPortal_Client.DTO;
using DentistPortal_API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Net.Mime;
using System.Net.Http.Headers;

namespace DentistPortal_Client.Pages
{
    public class DentistProfileModel : PageModel
    {
        [TempData]
        public string Msg { get; set; } = String.Empty;
        [TempData]
        public string Status { get; set; } = String.Empty;
        public Dentist LoggedDentist { get; set; } = new Dentist();

        //public IFormFile? newImage { get; set; }
        //public IFormFile? uniPic { get; set; }
        //public IFormFile? idenPic { get; set; }
        public ChangeImageDto? obj { get; set; }


        IConfiguration config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .AddEnvironmentVariables()
               .Build();


        public async Task OnGet()
        {
            //var username = HttpContext.Session.GetString("username");
            var token = HttpContext.Session.GetString("token");
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var Id = jwt.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            var httpClient = HttpContext.RequestServices.GetService<IHttpClientFactory>();
            var client = httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            var jsoncategory = JsonConvert.SerializeObject(Id);
            var content = new StringContent(jsoncategory, Encoding.UTF8, "application/json");
            var request = await client.PostAsync("/api/Profile", content);

            if (request.IsSuccessStatusCode)
            {
                var stringdata = request.Content.ReadAsStringAsync().Result;
                HttpContext.Session.SetString("LoggedUser", stringdata);
                LoggedDentist = JsonConvert.DeserializeObject<Dentist>(stringdata);
            }
        }

        public async Task<IActionResult> OnPost(ChangeImageDto? obj)
        {
            var username = HttpContext.Session.GetString("username");
            var role = HttpContext.Session.GetString("role");
            if (!string.IsNullOrEmpty(role)) 
            {
                obj.username = username;

                var content = new MultipartFormDataContent();
                content = await MappingContent(content, obj);

                var httpClient = HttpContext.RequestServices.GetService<IHttpClientFactory>();
                var client = httpClient.CreateClient();
                client.BaseAddress = new Uri(config["BaseAddress"]);
                //var jsoncategory = JsonConvert.SerializeObject(changeImageDto);
                //var content = new StringContent(jsoncategory, Encoding.UTF8, "application/json");
                var request = await client.PostAsync("/api/ChangeImage", content);
                if (request.IsSuccessStatusCode)
                {
                    return RedirectToPage("/DentistProfile");
                }
                return RedirectToPage("/DentistProfile");
            }
            return RedirectToPage("/DentistProfile");
        }

        public async Task<IActionResult> OnPostUpdate(Dentist obj)
        {
            var JsonObject = HttpContext.Session.GetString("LoggedUser");
            LoggedDentist = JsonConvert.DeserializeObject<Dentist>(JsonObject);

            if (obj.Username != LoggedDentist.Username) { LoggedDentist.Username = obj.Username; }
            if (obj.FirstName != LoggedDentist.FirstName) { LoggedDentist.FirstName = obj.FirstName; }
            if (obj.LastName != LoggedDentist.LastName) { LoggedDentist.LastName = obj.LastName; }


            if (obj.PasswordHash != "password")
            {
                LoggedDentist.PasswordHash = obj.PasswordHash;
                var httpClient = HttpContext.RequestServices.GetService<IHttpClientFactory>();
                var client = httpClient.CreateClient();
                client.BaseAddress = new Uri(config["BaseAddress"]);
                var jsoncategory = JsonConvert.SerializeObject(LoggedDentist);
                var content = new StringContent(jsoncategory, Encoding.UTF8, "application/json");
                var request = await client.PostAsync("/api/EditPatientProfile", content);

                if (request.IsSuccessStatusCode)
                {
                    Status = "success";
                    Msg = "Data Updated Successfully";
                    return new JsonResult(new { Msg, Status });
                }
                else
                {
                    Status = "error";
                    Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    return new JsonResult(new { Msg, Status });
                }
            }
            else
            {
                var httpClient = HttpContext.RequestServices.GetService<IHttpClientFactory>();
                var client = httpClient.CreateClient();
                client.BaseAddress = new Uri(config["BaseAddress"]);
                var jsoncategory = JsonConvert.SerializeObject(LoggedDentist);
                var content = new StringContent(jsoncategory, Encoding.UTF8, "application/json");
                var request = await client.PostAsync("/api/EditDentistProfile", content);

                if (request.IsSuccessStatusCode) 
                {
                    Status = "success";
                    Msg = "Data Updated Successfully";
                    return new JsonResult(new { Msg, Status });
                }
                else
                {
                    Status = "error";
                    Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    return new JsonResult(new { Msg, Status });
                }
            }
        }

        private async Task<MultipartFormDataContent> MappingContent(MultipartFormDataContent multipartFormDataContent, ChangeImageDto? Obj)
        {
            multipartFormDataContent.Add(new StringContent(Obj.username, Encoding.UTF8, MediaTypeNames.Text.Plain), "username");

            if (Obj.ProfilePicture != null)
            {
                var fileContent = new StreamContent(Obj.ProfilePicture.OpenReadStream());
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(Obj.ProfilePicture.ContentType);
                multipartFormDataContent.Add(fileContent, "ProfilePicture", Obj.ProfilePicture.FileName);
            }
            if (Obj.UniversityCardPicture != null)
            {
                var fileContent = new StreamContent(Obj.UniversityCardPicture.OpenReadStream());
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(Obj.UniversityCardPicture.ContentType);
                multipartFormDataContent.Add(fileContent, "UniversityCardPicture", Obj.UniversityCardPicture.FileName);
            }
            if (Obj.IdentityCardPicture != null)
            {
                var fileContent = new StreamContent(Obj.IdentityCardPicture.OpenReadStream());
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(Obj.IdentityCardPicture.ContentType);
                multipartFormDataContent.Add(fileContent, "IdentityCardPicture", Obj.IdentityCardPicture.FileName);
            }
            return multipartFormDataContent;
        }
    }
}
