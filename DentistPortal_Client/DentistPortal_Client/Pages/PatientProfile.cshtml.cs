using DentistPortal_API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
//using DentistPortal_API.DTO;
using DentistPortal_Client.DTO;
using System.Net.Mime;
using System.Net.Http.Headers;
using System.Diagnostics;

namespace DentistPortal_Client.Pages
{
    public class PatientProfileModel : PageModel
    {
        [TempData]
        public string Msg { get; set; } = String.Empty;
        [TempData]
        public string Status { get; set; } = String.Empty;
        public Patient Loggedpatient { get; set; } = new Patient();
        public ChangeImageDto? obj { get; set; }

        //public string ProfilePicture { get; set; }

        IConfiguration config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .AddEnvironmentVariables()
               .Build();


        public async Task OnGet()
        {
            try
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
                    Loggedpatient = JsonConvert.DeserializeObject<Patient>(stringdata);
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

        public async Task<IActionResult> OnPost(ChangeImageDto obj)
        {
            try
            {
                var username = HttpContext.Session.GetString("username");
                obj.username = username;

                var content = new MultipartFormDataContent();
                if (obj.ProfilePicture != null)
                {
                    content = await MappingContent(content, obj);
                }
                var httpClient = HttpContext.RequestServices.GetService<IHttpClientFactory>();
                var client = httpClient.CreateClient();
                client.BaseAddress = new Uri(config["BaseAddress"]);
                //var jsoncategory = JsonConvert.SerializeObject(changeImageDto);
                //var content = new StringContent(jsoncategory, Encoding.UTF8, "application/json");
                var request = await client.PostAsync("/api/ChangeImage", content);

                if (request.IsSuccessStatusCode)
                {
                    return RedirectToPage("/PatientProfile");
                }
                return RedirectToPage("/PatientProfile");
            }
            catch(Exception ex)
            {
                var st = new StackTrace(ex, true);
                var frame = st.GetFrame(0);
                var line = frame.GetFileLineNumber();

                return Content("something went wrong please try again");
            }
        }
        public async Task<IActionResult> OnPostUpdate(Patient obj)
        {
            try
            {
                var JsonObject = HttpContext.Session.GetString("LoggedUser");
                Loggedpatient = JsonConvert.DeserializeObject<Patient>(JsonObject);

                {
                    //    if (obj.Username == Loggedpatient.Username) { }
                    //    else { Loggedpatient.Username = obj.Username; }

                    //    if (obj.FirstName == Loggedpatient.FirstName) { }
                    //    else { Loggedpatient.FirstName = obj.FirstName; }

                    //    if (obj.LastName == Loggedpatient.LastName) { }
                    //    else { Loggedpatient.LastName = obj.LastName; }
                }
                if (obj.Username != Loggedpatient.Username) { Loggedpatient.Username = obj.Username; }
                if (obj.FirstName != Loggedpatient.FirstName) { Loggedpatient.FirstName = obj.FirstName; }
                if (obj.LastName != Loggedpatient.LastName) { Loggedpatient.LastName = obj.LastName; }

                if (obj.PasswordHash != "password")
                {
                    Loggedpatient.PasswordHash = obj.PasswordHash;
                    var httpClient = HttpContext.RequestServices.GetService<IHttpClientFactory>();
                    var client = httpClient.CreateClient();
                    client.BaseAddress = new Uri(config["BaseAddress"]);
                    var jsoncategory = JsonConvert.SerializeObject(Loggedpatient);
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
                    var jsoncategory = JsonConvert.SerializeObject(Loggedpatient);
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
            }
            catch(Exception ex)
            {
                var st = new StackTrace(ex, true);
                var frame = st.GetFrame(0);
                var line = frame.GetFileLineNumber();

                return Content("something went wrong please try again");
            }
        }

        private async Task<MultipartFormDataContent> MappingContent(MultipartFormDataContent multipartFormDataContent, ChangeImageDto Obj)
        {
            multipartFormDataContent.Add(new StringContent(Obj.username, Encoding.UTF8, MediaTypeNames.Text.Plain), "username");

            if (Obj.ProfilePicture != null)
            {
                var fileContent = new StreamContent(Obj.ProfilePicture.OpenReadStream());
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(Obj.ProfilePicture.ContentType);
                multipartFormDataContent.Add(fileContent, "ProfilePicture", Obj.ProfilePicture.FileName);
            }
            return multipartFormDataContent;
        }
    }
}
