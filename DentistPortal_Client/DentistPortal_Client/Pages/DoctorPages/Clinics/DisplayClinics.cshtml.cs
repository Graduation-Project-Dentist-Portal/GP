using DentistPortal_Client.DTO;
using DentistPortal_Client.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace DentistPortal_Client.Pages.DoctorPages.Clinics
{
    public class DisplayClinicsModel : PageModel
    {
        private IHttpClientFactory _httpClient;
        public Guid DoctorId;
        public IConfiguration config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .AddEnvironmentVariables()
               .Build();
        [TempData]
        public string Msg { get; set; } = String.Empty;
        [TempData]
        public string Status { get; set; } = String.Empty;
        public List<Clinic> Clinics = new();
        public string[] Pictures { get; set; }

        public DisplayClinicsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory;
        }

        public async Task OnGet()
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(HttpContext.Session.GetString("Token"));
            DoctorId = Guid.Parse(jwt.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            try
            {
                var request = await client.GetStringAsync("/api/get-clinics");
                if (request is not null)
                {
                    if (request.Length > 0)
                    {
                        var options = new JsonSerializerOptions
                        {
                            WriteIndented = true,
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
                        };
                        Clinics = JsonSerializer.Deserialize<List<Clinic>>(request, options);
                    }
                }
                else
                {
                    Msg = request.ToString();
                    Status = "error";
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == HttpStatusCode.Unauthorized)
                {
                    LoginModel loginModel = new LoginModel(_httpClient);
                    await loginModel.GetNewToken(HttpContext);
                    await OnGet();
                }
            }
            catch (Exception e)
            {
                Msg = e.Message;
                Status = "error";
            }
        }

        public async Task<IActionResult> OnPost(ClinicDto clinicDto)
        {
            var token = HttpContext.Session.GetString("Token");
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            clinicDto.DoctorId = Guid.Parse(jwt.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var multipartContent = new MultipartFormDataContent();
            multipartContent = await MappingContent(multipartContent, clinicDto);
            var request = await client.PostAsync("/api/create-clinic", multipartContent);
            if (request.IsSuccessStatusCode)
            {
                Msg = "Successfully added clinic!";
                Status = "success";
                return RedirectToPage("DisplayClinics");
            }
            else
            {
                Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Status = "error";
                return RedirectToPage("DisplayClinics");
            }
        }

        public async Task<IActionResult> OnPostDelete(Guid id)
        {
            var token = HttpContext.Session.GetString("Token");
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var request = await client.DeleteAsync($"api/delete-clinic/{id}");
            if (request.IsSuccessStatusCode)
            {
                Msg = "Deleted successfully!";
                Status = "success";
                return RedirectToPage("/DoctorPages/Clinics/DisplayClinics");
            }
            else
            {
                Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Status = "error";
                return RedirectToPage("");
            }
        }

        public async Task<IActionResult> OnPostEdit(ClinicDto clinicDto, Guid id)
        {
            var token = HttpContext.Session.GetString("Token");
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var multipartContent = new MultipartFormDataContent();
            multipartContent = await MappingContent(multipartContent, clinicDto);
            var request = await client.PutAsync($"api/edit-clinic/{id}", multipartContent);
            if (request.IsSuccessStatusCode)
            {
                Msg = "Edited successfully!";
                Status = "success";
                return RedirectToPage("/DoctorPages/Clinics/DisplayClinics");
            }
            else
            {
                Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Status = "error";
                return RedirectToPage("", new { id });
            }
        }

        private async Task<MultipartFormDataContent> MappingContent(MultipartFormDataContent multipartFormDataContent, ClinicDto clinicDto)
        {
            multipartFormDataContent.Add(new StringContent(clinicDto.Address, Encoding.UTF8, MediaTypeNames.Text.Plain), "Address");
            multipartFormDataContent.Add(new StringContent(clinicDto.Name, Encoding.UTF8, MediaTypeNames.Text.Plain), "Name");
            multipartFormDataContent.Add(new StringContent(clinicDto.ClinicDescription, Encoding.UTF8, MediaTypeNames.Text.Plain), "ClinicDescription");
            multipartFormDataContent.Add(new StringContent(clinicDto.DoctorId.ToString(), Encoding.UTF8, MediaTypeNames.Text.Plain), "DoctorId");
            multipartFormDataContent.Add(new StringContent(clinicDto.ClinicPhone, Encoding.UTF8, MediaTypeNames.Text.Plain), "ClinicPhone");
            multipartFormDataContent.Add(new StringContent(clinicDto.OpenTime.ToString(), Encoding.UTF8, MediaTypeNames.Text.Plain), "OpenTime");
            multipartFormDataContent.Add(new StringContent(clinicDto.CloseTime.ToString(), Encoding.UTF8, MediaTypeNames.Text.Plain), "CloseTime");
            if (clinicDto.CasePictures.Count > 0)
            {
                foreach (var file in clinicDto.CasePictures)
                {
                    var fileContent = new StreamContent(file.OpenReadStream());
                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);
                    multipartFormDataContent.Add(fileContent, "CasePictures", file.FileName);
                }
            }
            return multipartFormDataContent;
        }
    }
}
