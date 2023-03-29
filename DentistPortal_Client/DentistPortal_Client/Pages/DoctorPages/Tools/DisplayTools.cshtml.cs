using DentistPortal_Client.DTO;
using DentistPortal_Client.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace DentistPortal_Client.Pages.DoctorPages.Tools
{
    public class DisplayToolsModel : PageModel
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
        public List<Tool> Tools = new();
        public string[] Pictures { get; set; }
        public Tool tool1;
        public DisplayToolsModel(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task OnGet()
        {
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            try
            {
                var request = await client.GetStringAsync("/api/display-tools");
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
                        Tools = JsonSerializer.Deserialize<List<Tool>>(request, options);
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

        public async Task<IActionResult> OnPost(ToolDto toolDto, List<IFormFile> files)
        {
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await file.CopyToAsync(ms);
                        var fileBytes = ms.ToArray();
                        string s = Convert.ToBase64String(fileBytes);
                        // act on the Base64 data
                        toolDto.PicturePaths.Add(s);
                    }
                }
            }
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(HttpContext.Session.GetString("Token"));
            toolDto.SellerIdDoctor = Guid.Parse(jwt.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            var jsonCategory = JsonSerializer.Serialize(toolDto);
            var content = new StringContent(jsonCategory, Encoding.UTF8, "application/json");
            var request = await client.PostAsync("/api/create-tool", content);
            if (request.IsSuccessStatusCode)
            {
                Msg = "Successfully added";
                Status = "success";
                return RedirectToPage("DisplayTools");
            }
            else
            {
                Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Status = "error";
                return RedirectToPage("DisplayTools");
            }
        }

        public async Task<IActionResult> OnPostDelete(Guid id)
        {
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            var request = await client.DeleteAsync($"api/delete-Tool/{id}");
            if (request.IsSuccessStatusCode)
            {
                Msg = "Successfully Deleted !";
                Status = "success";
                return RedirectToPage("DisplayTools");
            }
            else
            {
                Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Status = "error";
                return RedirectToPage("DisplayTools");
            }
        }

        public async Task<IActionResult> OnPostEdit(ToolDto toolDto, List<IFormFile>? files, Guid id)
        {
            if (files != null)
            {
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await file.CopyToAsync(ms);
                            var fileBytes = ms.ToArray();
                            string s = Convert.ToBase64String(fileBytes);
                            // act on the Base64 data
                            toolDto.PicturePaths.Add(s);

                        }
                    }
                }
            }
            var client = _httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            var jsonCategory = JsonSerializer.Serialize(toolDto);
            var content = new StringContent(jsonCategory, Encoding.UTF8, "application/json");
            var request = await client.PutAsync($"api/edit-tool/{id}", content);
            if (request.IsSuccessStatusCode)
            {
                Msg = "Edited successfully!";
                Status = "success";
                return RedirectToPage("DisplayTools");
            }
            else
            {
                Msg = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Status = "error";
                return RedirectToPage("", new { id });
            }
        }
    }
}
