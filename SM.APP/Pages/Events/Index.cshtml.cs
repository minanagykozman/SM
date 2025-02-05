using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.DAL.DataModel;
using System.Text.Json;

namespace SM.APP.Pages.Events
{
    [Authorize(Roles = "Admin,Servant")]
    public class IndexModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public List<Event> Events { get; set; }
        public async Task OnGetAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                string req = "https://apitest.stmosesservices.com/Events/GetEvents/1";
                HttpResponseMessage response = await client.GetAsync(req);
                string responseData = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Enable case insensitivity
                };
                Events = JsonSerializer.Deserialize<List<Event>>(responseData, options);
                if (Events == null)
                    Events = new List<Event>();
            }
        }
    }
}
