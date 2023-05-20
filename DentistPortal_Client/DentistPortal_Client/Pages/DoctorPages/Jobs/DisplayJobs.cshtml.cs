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


namespace DentistPortal_Client.Pages.DoctorPages.Jobs
{
    public class DisplayJobsModel : PageModel
    {
        public Guid DoctorId;
        public IConfiguration config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .AddEnvironmentVariables()
               .Build();
        private IHttpClientFactory _httpClient;
        [TempData]
        public string Msg { get; set; } = String.Empty;
        [TempData]
        public string Status { get; set; } = String.Empty;
        public Job Job = new();
        public List<Job> Jobs = new();
        public JobDto JobDto { get; set; }



        public DisplayJobsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory;
        }



        public async Task OnGet()
        {
            if (HttpContext.Session.GetString("role") == "Dentist")
            {
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(HttpContext.Session.GetString("Token"));
                DoctorId = Guid.Parse(jwt.Claims.First().Value);
                var client = _httpClient.CreateClient();
                client.BaseAddress = new Uri(config["BaseAddress"]);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));



                try
                {
                    var request = await client.GetStringAsync("/api/Display-All-Jobs");
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
                            Jobs = JsonSerializer.Deserialize<List<Job>>(request, options);
                        }
                    }
                    else
                    {
                        Msg = request.ToString();
                        Status = "error";
                    }
                }
                catch (Exception e)
                {
                    Msg = e.Message;
                    Status = "error";
                }
            }
            else
            {
                Response.Redirect($"https://localhost:7156/Login?url={"DoctorPages/Jobs/DisplayJobs"}");
            }

        }


        public async Task<IActionResult> OnPost(JobDto job)
        {
            var token = HttpContext.Session.GetString("Token");
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            job.OwnerIdDoctor = Guid.Parse(jwt.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var jsonCategory = JsonSerializer.Serialize(job);
            var content = new StringContent(jsonCategory, Encoding.UTF8, "application/json");
            var request = await client.PostAsync("/api/Add-Job", content);
            if (request.IsSuccessStatusCode)
            {
                Msg = "Job added successfully !";
                Status = "success";
                return RedirectToPage("DisplayJobs");
            }
            else
            {
                Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Status = "error";
                return RedirectToPage("DisplayJobs");
            }
        }

        public async Task<IActionResult> OnPostEdit(JobDto jobDto, Guid id)
        {

            var token = HttpContext.Session.GetString("Token");
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var jsonCategory = JsonSerializer.Serialize(jobDto);
            var content = new StringContent(jsonCategory, Encoding.UTF8, "application/json");
            var request = await client.PutAsync($"api/Edit-Job/{id}", content);

            if (request.IsSuccessStatusCode)
            {
                Msg = "Edited successfully!";
                Status = "success";
                return RedirectToPage("/DoctorPages/Jobs/DisplayJobs");
            }
            else
            {
                Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Status = "error";
                return RedirectToPage("", new { id });
            }
        }

        public async Task<IActionResult> OnPostDelete(Guid id)
        {

            var token = HttpContext.Session.GetString("Token");
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var DoctorId = jwt.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var request = await client.DeleteAsync($"/api/Delete-Job/" + id);
            if (request.IsSuccessStatusCode)
            {
                Msg = "Deleted successfully!";
                Status = "success";
                return RedirectToPage("/DoctorPages/Jobs/DisplayJobs");
            }
            else
            {
                Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Status = "error";
                return RedirectToPage("");
            }
        }
    }


}
