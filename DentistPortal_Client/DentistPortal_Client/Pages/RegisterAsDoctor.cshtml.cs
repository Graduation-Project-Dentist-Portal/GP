using DentistPortal_API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;
using System.Reflection;
using DentistPortal_Client.DTO;
using System.Net.Mime;
using System.Net.Http.Headers;

namespace DentistPortal_Client.Pages
{
    public class RegisterAsDoctorModel : PageModel
    {
        public DentistDto Dentist { get; set; } = new DentistDto();

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

        public async Task<IActionResult> OnPost(DentistDto dentist)
        {
            if (!ModelState.IsValid) 
            {
                Msg = "Cant be empty";
                Status = "error";
                return RedirectToPage("/RegisterAsDoctor" , dentist);
            }
            dentist.IsActive= true;
            if (dentist.Graduated)
            {
                dentist.Level = 0;
            }
            var httpClient = HttpContext.RequestServices.GetService<IHttpClientFactory>();
            var client = httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            //var jsonCategory = JsonSerializer.Serialize(dentist);
            //var content = new StringContent(jsonCategory, Encoding.UTF8, "application/json");
            var content = new MultipartFormDataContent();
            content = await MappingContent(content, dentist);
            var request = await client.PostAsync("/api/RegisterAsDoctor", content);
            if (request.IsSuccessStatusCode)
            {
                Msg = "Successfully Created!";
                Status = "success";
                return RedirectToPage("/Login");
            }
            else
            {
                Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Status = "error";
                return RedirectToPage("/RegisterAsDoctor");
            }
        }

        private async Task<MultipartFormDataContent> MappingContent(MultipartFormDataContent multipartFormDataContent, DentistDto dentist)
        {
            var graduated = Convert.ToString(dentist.Graduated);
            var level = Convert.ToString(dentist.Level);
            multipartFormDataContent.Add(new StringContent(dentist.Username, Encoding.UTF8, MediaTypeNames.Text.Plain), "Username");
            multipartFormDataContent.Add(new StringContent(dentist.FirstName, Encoding.UTF8, MediaTypeNames.Text.Plain), "FirstName");
            multipartFormDataContent.Add(new StringContent(dentist.LastName, Encoding.UTF8, MediaTypeNames.Text.Plain), "LastName");
            multipartFormDataContent.Add(new StringContent(dentist.PasswordHash, Encoding.UTF8, MediaTypeNames.Text.Plain), "PasswordHash");
            multipartFormDataContent.Add(new StringContent(dentist.University, Encoding.UTF8, MediaTypeNames.Text.Plain), "University");
            multipartFormDataContent.Add(new StringContent(graduated, Encoding.UTF8, MediaTypeNames.Text.Plain), "Graduated");
            multipartFormDataContent.Add(new StringContent(level, Encoding.UTF8, MediaTypeNames.Text.Plain), "Level");

            if (dentist.ProfilePicture != null)
            {
                var fileContent = new StreamContent(dentist.ProfilePicture.OpenReadStream());
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(dentist.ProfilePicture.ContentType);
                multipartFormDataContent.Add(fileContent, "ProfilePicture", dentist.ProfilePicture.FileName);
            }
            if (dentist.IdentityCardPicture != null)
            {
                var fileContent = new StreamContent(dentist.IdentityCardPicture.OpenReadStream());
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(dentist.IdentityCardPicture.ContentType);
                multipartFormDataContent.Add(fileContent, "IdentityCardPicture", dentist.IdentityCardPicture.FileName);
            }
            if (dentist.UniversityCardPicture != null)
            {
                var fileContent = new StreamContent(dentist.UniversityCardPicture.OpenReadStream());
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(dentist.UniversityCardPicture.ContentType);
                multipartFormDataContent.Add(fileContent, "UniversityCardPicture", dentist.UniversityCardPicture.FileName);
            }
            return multipartFormDataContent;
        }
    }
}


