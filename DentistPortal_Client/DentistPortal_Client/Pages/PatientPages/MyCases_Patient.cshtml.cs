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
using DentistPortal_Client.Model;

namespace DentistPortal_Client.Pages.PatientPages
{
    public class MyCases_PatientModel : PageModel
    {
        private IHttpClientFactory _httpClient;
        IConfiguration config = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json")
              .AddEnvironmentVariables()
              .Build();
        [TempData]
        public string Msg { get; set; } = String.Empty;
        [TempData]
        public string Status { get; set; } = String.Empty;
        public List<PatientCase> PatientCases = new();
        public string[] PatientCasePictures { get; set; }
        public Guid PatientId;
        public MyCases_PatientModel(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task OnGet()
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(HttpContext.Session.GetString("Token"));
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            try
            {
                var request = await client.GetStringAsync($"/api/display-my-cases-pateint/{jwt.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value}");
                if (request is not null)
                {
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
                    };
                    PatientCases = JsonSerializer.Deserialize<List<PatientCase>>(request, options);
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


        public async Task<IActionResult> OnPost(PatientCaseDto patientCaseDto)
        {
            if (patientCaseDto.PatientAge <= 0 || patientCaseDto.PatientAge >= 100 || patientCaseDto.PatientAge != (int)patientCaseDto.PatientAge)
            {
                Msg = "Wrong input for Patient Age!";
                Status = "error";
                return RedirectToPage("DisplayPatientCases");
            }
            var token = HttpContext.Session.GetString("Token");
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            patientCaseDto.PatientId = Guid.Parse(jwt.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var multipartContent = new MultipartFormDataContent();
            multipartContent = await MappingContent(multipartContent, patientCaseDto);
            var request = await client.PostAsync("/api/create-patient-case", multipartContent);
            if (request.IsSuccessStatusCode)
            {
                Msg = "Successfully added your case!";
                Status = "success";
                return RedirectToPage("MyCases_Patient");
            }
            else
            {
                Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Status = "error";
                return RedirectToPage("MyCases_Patient");
            }
        }

        public async Task<IActionResult> OnPostDelete(Guid id)
        {
            var token = HttpContext.Session.GetString("Token");
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var request = await client.DeleteAsync($"api/delete-patient-case/{id}");
            if (request.IsSuccessStatusCode)
            {
                Msg = "Deleted successfully!";
                Status = "success";
                return RedirectToPage("MyCases_Patient");
            }
            else
            {
                Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Status = "error";
                return RedirectToPage("MyCases_Patient");
            }
        }

        public async Task<IActionResult> OnPostEdit(PatientCaseDto patientCaseDto, Guid id)
        {
            if (patientCaseDto.PatientAge <= 0 || patientCaseDto.PatientAge >= 100 || patientCaseDto.PatientAge != (int)patientCaseDto.PatientAge)
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
            multipartContent = await MappingContent(multipartContent, patientCaseDto);
            var request = await client.PutAsync($"api/edit-patient-case/{id}", multipartContent);
            if (request.IsSuccessStatusCode)
            {
                Msg = "Edited successfully!";
                Status = "success";
                return RedirectToPage("MyCases_Patient");
            }
            else
            {
                Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Status = "error";
                return RedirectToPage("", new { id });
            }
        }





        private async Task<MultipartFormDataContent> MappingContent(MultipartFormDataContent multipartFormDataContent, PatientCaseDto patientCaseDto)
        {
            multipartFormDataContent.Add(new StringContent(patientCaseDto.Description, Encoding.UTF8, MediaTypeNames.Text.Plain), "Description");
            multipartFormDataContent.Add(new StringContent(patientCaseDto.AssignedDoctorId.ToString(), Encoding.UTF8, MediaTypeNames.Text.Plain), "AssignedDoctorId");
            multipartFormDataContent.Add(new StringContent(patientCaseDto.PatientPhone, Encoding.UTF8, MediaTypeNames.Text.Plain), "PatientPhone");
            multipartFormDataContent.Add(new StringContent(patientCaseDto.PatientAge.ToString(), Encoding.UTF8, MediaTypeNames.Text.Plain), "PatientAge");
            multipartFormDataContent.Add(new StringContent(patientCaseDto.PatientId.ToString(), Encoding.UTF8, MediaTypeNames.Text.Plain), "PatientId");

            if (patientCaseDto.PatientCasePictures.Count > 0)
            {
                foreach (var file in patientCaseDto.PatientCasePictures)
                {
                    var fileContent = new StreamContent(file.OpenReadStream());
                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);
                    multipartFormDataContent.Add(fileContent, "PatientCasePictures", file.FileName);
                }
            }
            return multipartFormDataContent;
        }

    }
}
