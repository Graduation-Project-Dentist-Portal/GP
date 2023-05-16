using DentistPortal_API.DTO;
using DentistPortal_API.Model;
using DentistPortal_Client.DTO;
using DentistPortal_Client.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace DentistPortal_Client.Pages.DoctorPages.MedicalCases
{
    public class MyMedicalCasesModel : PageModel
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
        public List<MedicalCase> MedicalCases = new();
        public List<PatientCase> PatientCases = new();
        public string[] Pictures { get; set; }


        public MyMedicalCasesModel(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }

        //public async Task OnGet()
        //{
        //    var jwt = new JwtSecurityTokenHandler().ReadJwtToken(HttpContext.Session.GetString("Token"));
        //    var id = Guid.Parse(jwt.Claims.First().Value);
        //    var client = _httpClient.CreateClient();
        //    client.BaseAddress = new Uri(config["BaseAddress"]);
        //    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
        //    try
        //    {
        //        var request1 = await client.GetStringAsync($"/api/my-medical-cases/{id}");
        //        if (request1 is not null)
        //        {
        //            var options1 = new JsonSerializerOptions
        //            {
        //                WriteIndented = true,
        //                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        //                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
        //            };
        //            MedicalCases = JsonSerializer.Deserialize<List<MedicalCase>>(request1, options1);
        //        }
        //        else
        //        {
        //            Msg = request1.ToString();
        //            Status = "error";
        //        }

        //        var request2 = await client.GetStringAsync($"/api/my-patients-cases/{id}");
        //        if(request2 is not null)
        //        {
        //            var options2 = new JsonSerializerOptions
        //            {
        //                WriteIndented = true,
        //                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        //                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
        //            };
        //            PatientCases = JsonSerializer.Deserialize<List<PatientCase>>(request2, options2);
        //        }
        //        else
        //        {
        //            Msg = request2.ToString();
        //            Status= "error";
        //        }
        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        if (ex.StatusCode == HttpStatusCode.Unauthorized)
        //        {
        //            LoginModel loginModel = new LoginModel(_httpClient);
        //            await loginModel.GetNewToken(HttpContext);
        //            await OnGet();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Msg = e.Message;
        //        Status = "error";
        //    }
        //}






        public async Task OnGet()
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(HttpContext.Session.GetString("Token"));
            var id = Guid.Parse(jwt.Claims.First().Value);
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            try
            {
                var request1 = await client.GetStringAsync($"/api/my-cases-doctor/{id}");
                if (request1 is not null)
                {
                    var options1 = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
                    };
                    var combinedList = JsonSerializer.Deserialize<CombinedCasesDto>(request1, options1);
                    PatientCases = combinedList.PatientCases;
                    MedicalCases = combinedList.MedicalCases;
                }
                else
                {
                    Msg = request1.ToString();
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




        public async Task<IActionResult> OnPostCancel(Guid id)
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(HttpContext.Session.GetString("Token"));
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            var request = await client.PutAsJsonAsync($"/api/leave-case-all", id);
            if (request.IsSuccessStatusCode)
            {
                Msg = "Canceled successfully";
                Status = "success";
                return RedirectToPage("");
            }
            else
            {
                Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Status = "error";
                return RedirectToPage("");
            }







            }


  //public async Task<IActionResult> OnPostCancelByPatient(Guid id)
            //{
            //    var jwt = new JwtSecurityTokenHandler().ReadJwtToken(HttpContext.Session.GetString("Token"));
            //    var client = _httpClient.CreateClient();
            //    client.BaseAddress = new Uri(config["BaseAddress"]);
            //    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            //    var request = await client.PutAsJsonAsync($"/api/leave-patient-case", id);
            //    if (request.IsSuccessStatusCode)
            //    {
            //        Msg = "Canceled successfully";
            //        Status = "success";
            //        return RedirectToPage("");
            //    }
            //    else
            //    {
            //        Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            //        Status = "error";
            //        return RedirectToPage("");
            //    }







            //}


            //private async Task<IActionResult> CancelCase(string endpoint, Guid id)
            //{
            //    var jwt = new JwtSecurityTokenHandler().ReadJwtToken(HttpContext.Session.GetString("Token"));
            //    var client = _httpClient.CreateClient();
            //    client.BaseAddress = new Uri(config["BaseAddress"]);
            //    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            //    var request = await client.PutAsJsonAsync(endpoint, id);
            //    if (request.IsSuccessStatusCode)
            //    {
            //        Msg = "Canceled successfully";
            //        Status = "success";
            //        return RedirectToPage("");
            //    }
            //    else
            //    {
            //        Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            //        Status = "error";
            //        return RedirectToPage("");
            //    }
            //}

            //public async Task<IActionResult> OnPostCancel(Guid id)
            //{
            //    return await CancelCase("/api/leave-medical-case", id);
            //}

            //public async Task<IActionResult> OnPostCancelByPatient(Guid id)
            //{
            //    return await CancelCase("/api/leave-patient-case", id);
            //}



        }
}
