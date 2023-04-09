using DentistPortal_Client.DTO;
using DentistPortal_Client.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace DentistPortal_Client.Pages.DoctorPages.Clinics
{
    public class ViewClinicModel : PageModel
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
        public Clinic Clinic = new();
        public List<Feedback> Feedbacks = new();
        public string[] Pictures { get; set; }

        public ViewClinicModel(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task OnGet(Guid id)
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(HttpContext.Session.GetString("Token"));
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            var request = await client.GetStringAsync($"/api/get-clinic/{id}");
            if (request is not null)
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
                };
                Clinic = JsonSerializer.Deserialize<Clinic>(request, options);
            }
            else
            {
                Msg = request.ToString();
                Status = "error";
            }
            var req = await client.GetStringAsync($"/api/get-feedbacks/{id}");
            if (req is not null)
            {
                if (req.Length > 0)
                {
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
                    };
                    Feedbacks = JsonSerializer.Deserialize<List<Feedback>>(req, options);
                }
            }
            else
            {
                Msg = request.ToString();
                Status = "error";
            }
        }

        public async Task<IActionResult> OnPostAddComment(FeedbackDto feedbackDto)
        {
            try
            {
                Comment comment = new Comment
                {
                    comment = feedbackDto.Comment
                };
                var clientForAi = _httpClient.CreateClient();
                clientForAi.BaseAddress = new Uri("http://127.0.0.1:5000");
                var jsonComment = JsonSerializer.Serialize(comment);
                var contentForAi = new StringContent(jsonComment, Encoding.UTF8, "application/json");
                var requestForAi = await clientForAi.PostAsync("/api/get-score", contentForAi);
                if (requestForAi.IsSuccessStatusCode)
                {
                    feedbackDto.AiScore = requestForAi.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }
                else
                {
                    Msg = requestForAi.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    Status = "error";
                    return RedirectToPage("", new { id = feedbackDto.ClinicId });
                }
            }
            catch (Exception e)
            {
                Msg = e.Message;
                Status = "error";
                return RedirectToPage("", new { id = feedbackDto.ClinicId });
            }
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(HttpContext.Session.GetString("Token"));
            feedbackDto.UserId = Guid.Parse(jwt.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            var jsonCategory = JsonSerializer.Serialize(feedbackDto);
            var content = new StringContent(jsonCategory, Encoding.UTF8, "application/json");
            try
            {
                var request = await client.PostAsync($"/api/add-feedback", content);
                if (request.IsSuccessStatusCode)
                {
                    Msg = "Added susscessfully!";
                    Status = "success";
                    return RedirectToPage("", new { id = feedbackDto.ClinicId });
                }
                else
                {
                    Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    Status = "error";
                    return RedirectToPage("", new { id = feedbackDto.ClinicId });
                }
            }
            catch (Exception e)
            {
                Msg = e.Message;
                Status = "error";
                return RedirectToPage("", new { id = feedbackDto.ClinicId });
            }
        }

        public async Task<IActionResult> OnPostLike(Guid id, Guid clinicId)
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(HttpContext.Session.GetString("Token"));
            LikeDto likeDto = new LikeDto();
            likeDto.FeedbackId = id;
            likeDto.PatientId = Guid.Parse(jwt.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            var jsonCategory = JsonSerializer.Serialize(likeDto);
            var content = new StringContent(jsonCategory, Encoding.UTF8, "application/json");
            try
            {
                var request = await client.PostAsync($"/api/like", content);
                if (request.IsSuccessStatusCode)
                {
                    Msg = "Up voted susscessfully!";
                    Status = "success";
                    return RedirectToPage("", new { id = clinicId });
                }
                else
                {
                    Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    Status = "error";
                    return RedirectToPage("", new { id = clinicId });
                }
            }
            catch (Exception e)
            {
                Msg = e.Message;
                Status = "error";
                return RedirectToPage("", new { id = clinicId });
            }
        }

        public async Task<IActionResult> OnPostUnLike(Guid id, Guid clinicId)
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(HttpContext.Session.GetString("Token"));
            LikeDto likeDto = new LikeDto();
            likeDto.FeedbackId = id;
            likeDto.PatientId = Guid.Parse(jwt.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            var jsonCategory = JsonSerializer.Serialize(likeDto);
            var content = new StringContent(jsonCategory, Encoding.UTF8, "application/json");
            try
            {
                var request = await client.PostAsync($"/api/un-like", content);
                if (request.IsSuccessStatusCode)
                {
                    Msg = "Down voted susscessfully!";
                    Status = "success";
                    return RedirectToPage("", new { id = clinicId });
                }
                else
                {
                    Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    Status = "error";
                    return RedirectToPage("", new { id = clinicId });
                }
            }
            catch (Exception e)
            {
                Msg = e.Message;
                Status = "error";
                return RedirectToPage("", new { id = clinicId });
            }
        }

        public async Task<IActionResult> OnPostDelete(Guid id, Guid clinicId)
        {
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            var request = await client.DeleteAsync($"api/delete-feedback/{id}");
            if (request.IsSuccessStatusCode)
            {
                Msg = "Successfully Deleted !";
                Status = "success";
                return RedirectToPage("", new { id = clinicId });
            }
            else
            {
                Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Status = "error";
                return RedirectToPage("", new { id = clinicId });
            }
        }

        public async Task<IActionResult> OnPostEdit(FeedbackDto feedbackDto, Guid id)
        {
            try
                {
                Comment comment = new Comment
                {
                    comment = feedbackDto.Comment
                };
                var clientForAi = _httpClient.CreateClient();
                clientForAi.BaseAddress = new Uri("http://127.0.0.1:5000");
                var jsonComment = JsonSerializer.Serialize(comment);
                var contentForAi = new StringContent(jsonComment, Encoding.UTF8, "application/json");
                var requestForAi = await clientForAi.PostAsync("/api/get-score", contentForAi);
                if (requestForAi.IsSuccessStatusCode)
                {
                    feedbackDto.AiScore = requestForAi.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }
                else
                {
                    Msg = requestForAi.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    Status = "error";
                    return RedirectToPage("", new { id = feedbackDto.ClinicId });
                }
            }
            catch (Exception e)
            {
                Msg = e.Message;
                Status = "error";
                return RedirectToPage("", new { id = feedbackDto.ClinicId });
            }
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            var jsonCategory = JsonSerializer.Serialize(feedbackDto);
            var content = new StringContent(jsonCategory, Encoding.UTF8, "application/json");
            try
            {
                var request = await client.PutAsync($"api/edit-feedback/{id}", content);
                if (request.IsSuccessStatusCode)
                {
                    Msg = "Edited successfully!";
                    Status = "success";
                    return RedirectToPage("", new { id = feedbackDto.ClinicId });
                }
                else
                {
                    Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    Status = "error";
                    return RedirectToPage("", new { id = feedbackDto.ClinicId });
                }
            }
            catch (Exception e)
            {
                Msg = e.Message;
                Status = "error";
                return RedirectToPage("", new { id = feedbackDto.ClinicId });
            }
        }
    }
}
