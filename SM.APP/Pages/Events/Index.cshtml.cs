using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using NuGet.Common;
using SM.APP.Services;
using SM.DAL.DataModel;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SM.APP.Pages.Events
{
    [Authorize(Roles = "Admin,Servant")]
    public class IndexModel : PageModelBase
    {
        public IndexModel(UserManager<IdentityUser> userManager, ILogger<IndexModel> logger) : base(userManager, logger)
        {
        }

        [BindProperty(SupportsGet = true)]
        public List<Event> Events { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                string jwtToken = await GetAPIToken();
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                    string request = string.Format("{0}/Events/GetEvents", SMConfigurationManager.ApiBase);
                    HttpResponseMessage response = await client.GetAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };
                        Events = JsonSerializer.Deserialize<List<Event>>(responseData, options);
                    }
                }
                if (Events == null)
                    Events = new List<Event>();
                return Page();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
