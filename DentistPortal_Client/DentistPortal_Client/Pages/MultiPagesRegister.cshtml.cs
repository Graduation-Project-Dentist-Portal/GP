using DentistPortal_Client.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace DentistPortal_Client.Pages
{
    public class MultiPagesRegisterModel : PageModel
    {
        public DentistDto Dentist { get; set; } = new DentistDto();
        IConfiguration config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .AddEnvironmentVariables()
               .Build();

        [TempData]
        public string Msg { get; set; } = string.Empty;
        [TempData]
        public string Status { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(DentistDto dentist)
        {
            if (!ModelState.IsValid)
            {
                Msg = "Cant be empty";
                Status = "error";
                return RedirectToPage("/RegisterAsDoctor", dentist);
            }
            dentist.IsActive = true;
            if (dentist.Graduated)
            {
                dentist.Level = 0;
            }
            var httpClient = HttpContext.RequestServices.GetService<IHttpClientFactory>();
            var client = httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            var content = new MultipartFormDataContent();
            content = await MappingContent(content, dentist);
            var request = await client.PostAsync("/api/RegisterAsDoctor", content);
            if (request.IsSuccessStatusCode)
            {
                Msg = "Successfully Created! \n Please wait until we verify your information.";
                Status = "success";
                return RedirectToPage("/Home");
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
            multipartFormDataContent.Add(new StringContent(dentist.Email, Encoding.UTF8, MediaTypeNames.Text.Plain), "Email");

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

