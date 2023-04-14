using DentistPortal_API.Model;
using DentistPortal_Client.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;

namespace DentistPortal_Client.Pages
{
    public class ViewProfileModel : PageModel
    {
        public FullUserDto userDto { get; set; } = new FullUserDto();

        IConfiguration config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .AddEnvironmentVariables()
               .Build();
        public async Task OnGet(string Id)
        {
            var httpClient = HttpContext.RequestServices.GetService<IHttpClientFactory>();
            var client = httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            var jsoncategory = JsonConvert.SerializeObject(Id);
            var content = new StringContent(jsoncategory, Encoding.UTF8, "application/json");
            var request = await client.PostAsync("/api/Profile", content);

            if (request.IsSuccessStatusCode)
            {
                var stringdata = request.Content.ReadAsStringAsync().Result;
                userDto = JsonConvert.DeserializeObject<FullUserDto>(stringdata);
            }
        }
    }
}
