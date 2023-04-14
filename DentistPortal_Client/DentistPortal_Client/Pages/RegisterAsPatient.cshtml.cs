using DentistPortal_API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;
using DentistPortal_Client.DTO;
using System.Net.Mime;
using System.Net.Http.Headers;

namespace DentistPortal_Client.Pages
{
    public class RegisterAsPatientModel : PageModel
    {
        public PatientDTO patient { get; set; } = new PatientDTO();

        //public IFormFile? profilePicture { get; set; }

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

        public async Task<IActionResult> OnPost(PatientDTO patient)
        {
            if (!ModelState.IsValid)
            {
                Msg = "cant be empty";
                Status = "error";
                return RedirectToPage("/registeraspatient", patient);
            }
            patient.IsActive = true;
            var httpclient = HttpContext.RequestServices.GetService<IHttpClientFactory>();
            var client = httpclient.CreateClient();
            client.BaseAddress = new Uri(config["baseaddress"]);
            //var jsoncategory = JsonSerializer.Serialize(patient);
            //var content = new StringContent(jsoncategory, Encoding.UTF8 , "application/json");
            var content = new MultipartFormDataContent();
            content = await MappingContent(content , patient);
            var request = await client.PostAsync("/api/registeraspatient", content);
            if (request.IsSuccessStatusCode)
            {
                Msg = "successfully created!";
                Status = "success";
                return RedirectToPage("/login");
            }
            else
            {
                Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Status = "error";
                return RedirectToPage("/registeraspatient");
            }
        }


        private async Task<MultipartFormDataContent> MappingContent(MultipartFormDataContent multipartFormDataContent, PatientDTO patient)
        {
            multipartFormDataContent.Add(new StringContent(patient.Username, Encoding.UTF8, MediaTypeNames.Text.Plain), "Username");
            multipartFormDataContent.Add(new StringContent(patient.FirstName, Encoding.UTF8, MediaTypeNames.Text.Plain), "FirstName");
            multipartFormDataContent.Add(new StringContent(patient.LastName, Encoding.UTF8, MediaTypeNames.Text.Plain), "LastName");
            multipartFormDataContent.Add(new StringContent(patient.PasswordHash, Encoding.UTF8, MediaTypeNames.Text.Plain), "PasswordHash");
           
                if (patient.ProfilePicture != null)
                {
                    var fileContent = new StreamContent(patient.ProfilePicture.OpenReadStream());
                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(patient.ProfilePicture.ContentType);
                    multipartFormDataContent.Add(fileContent, "ProfilePicture", patient.ProfilePicture.FileName);
                }
            return multipartFormDataContent;
        }
    }
}
