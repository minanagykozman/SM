using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.DAL.DataModel;
using System.Text.Json;

namespace SM.APP.Pages.Attendance
{
    [Authorize(Roles = "Admin,Servant")]
    public class IndexModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public List<Class> Classes { get; set; }
        public async Task OnGetAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                string req = "https://apitest.stmosesservices.com/Meeting/GetClasses/1";
                HttpResponseMessage response = await client.GetAsync(req);
                string responseData = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Enable case insensitivity
                };
                Classes = JsonSerializer.Deserialize<List<Class>>(responseData, options);
                if (Classes == null)
                    Classes = new List<Class>();
            }
        }
    }
}
