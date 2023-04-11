using DentistPortal_Client.DTO;
using DentistPortal_API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Net.Mime;
using System.Net.Http.Headers;
using System.Net;

namespace DentistPortal_Client.Pages.DoctorPages
{
    public class DisplayMedicalCasesModel : PageModel
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
        public List<MedicalCase> MedicalCases = new();
        public string[] Pictures { get; set; }
        public MedicalCaseDto MedicalCaseDto { get; set; }

        public DisplayMedicalCasesModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory;
        }

        public async Task OnGet()
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(HttpContext.Session.GetString("Token"));
            DoctorId = Guid.Parse(jwt.Claims.First().Value);
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            try
            {
                var request = await client.GetStringAsync("/api/display-medical-cases");
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
                        MedicalCases = JsonSerializer.Deserialize<List<MedicalCase>>(request, options);
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

        public async Task<IActionResult> OnPost(MedicalCaseDto medicalCaseDto)
        {
            if (medicalCaseDto.PatientAge <= 0 || medicalCaseDto.PatientAge >= 100 || medicalCaseDto.PatientAge != (int)medicalCaseDto.PatientAge)
            {
                Msg = "Wrong input for Patient Age!";
                Status = "error";
                return RedirectToPage("DisplayMedicalCases");
            }
            var token = HttpContext.Session.GetString("Token");
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            medicalCaseDto.DoctorId = Guid.Parse(jwt.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var multipartContent = new MultipartFormDataContent();
            multipartContent = await MappingContent(multipartContent, medicalCaseDto);
            var request = await client.PostAsync("/api/create-medical-case", multipartContent);
            if (request.IsSuccessStatusCode)
            {
                Msg = "Successfully added medical case!";
                Status = "success";
                return RedirectToPage("DisplayMedicalCases");
            }
            else
            {
                Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Status = "error";
                return RedirectToPage("DisplayMedicalCases");
            }
        }

        public async Task<IActionResult> OnPostTakeCase(Guid caseId)
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(HttpContext.Session.GetString("Token"));
            var doctorId = Guid.Parse(jwt.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            var request = await client.PutAsJsonAsync($"/api/take-medical-case/{caseId}", doctorId);
            if (request.IsSuccessStatusCode)
            {
                Msg = "Added successfully";
                Status = "success";
                return RedirectToPage("MyMedicalCases");
            }
            else
            {
                Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Status = "error";
                return RedirectToPage("");
            }
        }

        public async Task<IActionResult> OnPostDelete(Guid id)
        {
            var token = HttpContext.Session.GetString("Token");
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var request = await client.DeleteAsync($"api/delete-medical-case/{id}");
            if (request.IsSuccessStatusCode)
            {
                Msg = "Deleted successfully!";
                Status = "success";
                return RedirectToPage("/DoctorPages/MedicalCases/DisplayMedicalCases");
            }
            else
            {
                Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Status = "error";
                return RedirectToPage("");
            }
        }

        public async Task<IActionResult> OnPostEdit(MedicalCaseDto medicalCaseDto, Guid id)
        {
            if (medicalCaseDto.PatientAge <= 0 || medicalCaseDto.PatientAge >= 100 || medicalCaseDto.PatientAge != (int)medicalCaseDto.PatientAge)
            {
                Msg = "Wrong input for Patient Age!";
                Status = "error";
                return RedirectToPage("", new { id });
            }
            var token = HttpContext.Session.GetString("Token");
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var multipartContent = new MultipartFormDataContent();
            multipartContent = await MappingContent(multipartContent, medicalCaseDto);
            var request = await client.PutAsync($"api/edit-medical-case/{id}", multipartContent);
            if (request.IsSuccessStatusCode)
            {
                Msg = "Edited successfully!";
                Status = "success";
                return RedirectToPage("/DoctorPages/MedicalCases/DisplayMedicalCases");
            }
            else
            {
                Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Status = "error";
                return RedirectToPage("", new { id });
            }
        }

        private async Task<MultipartFormDataContent> MappingContent(MultipartFormDataContent multipartFormDataContent, MedicalCaseDto medicalCaseDto)
        {
            multipartFormDataContent.Add(new StringContent(medicalCaseDto.Description, Encoding.UTF8, MediaTypeNames.Text.Plain), "Description");
            multipartFormDataContent.Add(new StringContent(medicalCaseDto.PatientName, Encoding.UTF8, MediaTypeNames.Text.Plain), "PatientName");
            multipartFormDataContent.Add(new StringContent(medicalCaseDto.PatientPhone, Encoding.UTF8, MediaTypeNames.Text.Plain), "PatientPhone");
            multipartFormDataContent.Add(new StringContent(medicalCaseDto.Diagnosis, Encoding.UTF8, MediaTypeNames.Text.Plain), "Diagnosis");
            multipartFormDataContent.Add(new StringContent(medicalCaseDto.PatientAge.ToString(), Encoding.UTF8, MediaTypeNames.Text.Plain), "PatientAge");
            multipartFormDataContent.Add(new StringContent(medicalCaseDto.DoctorId.ToString(), Encoding.UTF8, MediaTypeNames.Text.Plain), "DoctorId");
            multipartFormDataContent.Add(new StringContent(medicalCaseDto.AssignedToMe.ToString(), Encoding.UTF8, MediaTypeNames.Text.Plain), "AssignedToMe");
            if (medicalCaseDto.CasePictures.Count > 0)
            {
                foreach (var file in medicalCaseDto.CasePictures)
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
