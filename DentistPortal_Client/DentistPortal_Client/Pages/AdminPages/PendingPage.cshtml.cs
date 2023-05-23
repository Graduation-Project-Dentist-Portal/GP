using DentistPortal_API.Model;
using DentistPortal_Client.DTO;
using Hangfire.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;

namespace DentistPortal_Client.Pages.AdminPages
{
    public class PendingPageModel : PageModel
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
                var request = await client.PostAsync("/api/Profile", content);

                if (request.IsSuccessStatusCode)
                {
                    var stringdata = request.Content.ReadAsStringAsync().Result;
                    HttpContext.Session.SetString("LoggedUser", stringdata);
                    LoggedDentist = JsonConvert.DeserializeObject<Dentist>(stringdata);
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

        public async Task<IActionResult> OnPost(ChangeImageDto? obj)
        {
            try
            {
                obj.verifyDentist = "false";
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
                        Msg = "Edited Successfully";
                        return Redirect($"https://localhost:7156/Home?clear={"yes"}");
                    }
                    else
                    {
                        Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        Status = "error";
                        return RedirectToPage();
                    }
                }
                return Redirect($"https://localhost:7156/Home?clear={"yes"}");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                var frame = st.GetFrame(0);
                var line = frame.GetFileLineNumber();

                return Content("something went wrong please try again");
            }
        }

        public async Task<IActionResult> OnPostUpdate(Dentist obj)
        {
            try
            {
                var JsonObject = HttpContext.Session.GetString("LoggedUser");
                LoggedDentist = JsonConvert.DeserializeObject<Dentist>(JsonObject);
                LoggedDentist.IsVerified = "false";

                if (obj.University != LoggedDentist.University) { LoggedDentist.University = obj.University; }
                if (obj.FirstName != LoggedDentist.FirstName) { LoggedDentist.FirstName = obj.FirstName; }
                if (obj.LastName != LoggedDentist.LastName) { LoggedDentist.LastName = obj.LastName; }
                if (obj.Email != LoggedDentist.Email) { LoggedDentist.Email = obj.Email; }
                if (obj.Level != LoggedDentist.Level) { LoggedDentist.Level = obj.Level; }



                if (obj.PasswordHash != "password")
                {
                    LoggedDentist.PasswordHash = obj.PasswordHash;
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
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                var frame = st.GetFrame(0);
                var line = frame.GetFileLineNumber();

                return Content("something went wrong please try again");
            }
        }

        private async Task<MultipartFormDataContent> MappingContent(MultipartFormDataContent multipartFormDataContent, ChangeImageDto? Obj)
        {
            multipartFormDataContent.Add(new StringContent(Obj.username, Encoding.UTF8, MediaTypeNames.Text.Plain), "username");
            multipartFormDataContent.Add(new StringContent(Obj.verifyDentist, Encoding.UTF8, MediaTypeNames.Text.Plain), "verifyDentist");

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
