using DentistPortal_API.Model;
using DentistPortal_Client.DTO;
using DentistPortal_Client.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace DentistPortal_Client.Pages.DoctorPages.MedicalCases
{
    public class FinishCaseModel : PageModel
    {
        private IHttpClientFactory _httpClient;
        public IConfiguration config = new ConfigurationBuilder()
                       .AddJsonFile("appsettings.json")
                       .AddEnvironmentVariables()
                       .Build();
        [TempData]
        public string Msg { get; set; } = String.Empty;
        [TempData]
        public string Status { get; set; } = String.Empty;
        public FinishedCaseDto FinishedCase { get; set; }

        public FinishCaseModel(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }

        public void OnGet(Guid id, FinishedCaseDto? finishedCaseDto)
        {
            if (HttpContext.Session.GetString("role") == "Dentist")
            {
                if (!string.IsNullOrEmpty(finishedCaseDto.DoctorWork))
                {
                    FinishedCase = finishedCaseDto;
                    FinishedCase.CaseId = finishedCaseDto.CaseId;
                }
                else
                {
                    FinishedCase = new();
                    FinishedCase.CaseId = id;
                }
            }
            else
            {
                Response.Redirect($"https://localhost:7156/Login?url={"DoctorPages/MedicalCases/FinishCase"}");
                Response.WriteAsync("redercting...");
            }
        }

        public async Task<IActionResult> OnPost(FinishedCaseDto finishedCaseDto)
        {
            var token = HttpContext.Session.GetString("Token");
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            finishedCaseDto.DoctorId = Guid.Parse(jwt.Claims.First().Value);
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var multipartContent = new MultipartFormDataContent();
            multipartContent = await MappingContent(multipartContent, finishedCaseDto);
            var request = await client.PutAsync("/api/finish-patient-case", multipartContent);
            if (request.IsSuccessStatusCode)
            {
                Msg = "Successfully finished!";
                Status = "success";
                return RedirectToPage("/DoctorPages/MedicalCases/MyMedicalCases");
            }
            else
            {
                Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Status = "error";
                return RedirectToPage("", finishedCaseDto);
            }
        }

        private async Task<MultipartFormDataContent> MappingContent(MultipartFormDataContent multipartFormDataContent, FinishedCaseDto finishedCaseDto)
        {
            multipartFormDataContent.Add(new StringContent(finishedCaseDto.DoctorWork, Encoding.UTF8, MediaTypeNames.Text.Plain), "DoctorWork");
            multipartFormDataContent.Add(new StringContent(finishedCaseDto.DoctorId.ToString(), Encoding.UTF8, MediaTypeNames.Text.Plain), "DoctorId");
            multipartFormDataContent.Add(new StringContent(finishedCaseDto.CaseId.ToString(), Encoding.UTF8, MediaTypeNames.Text.Plain), "CaseId");
            var fileContent = new StreamContent(finishedCaseDto.BeforePicture.OpenReadStream());
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(finishedCaseDto.BeforePicture.ContentType);
            multipartFormDataContent.Add(fileContent, "BeforePicture", finishedCaseDto.BeforePicture.FileName);
            fileContent = new StreamContent(finishedCaseDto.AfterPicture.OpenReadStream());
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(finishedCaseDto.AfterPicture.ContentType);
            multipartFormDataContent.Add(fileContent, "AfterPicture", finishedCaseDto.AfterPicture.FileName);
            return multipartFormDataContent;
        }
    }
}
