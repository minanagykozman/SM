using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.APP.Services;
using SM.DAL.DataModel;
using System.Text.Json;

namespace SM.APP.Pages.Events
{
    [Authorize(Roles = "Admin,Servant")]
    public class IndexModel : PageModel
    {
        int _sevantID;
        private readonly UserManager<IdentityUser> _userManager;
        public IndexModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;

        }
        [BindProperty(SupportsGet = true)]
        public List<Event> Events { get; set; }
        public async Task OnGetAsync()
        {
            if (_sevantID == 0)
            {
                var userId = _userManager.GetUserId(User);
                ServantService service = new ServantService();
                _sevantID = service.GetServantID(userId);
            }
            using (HttpClient client = new HttpClient())
            {
                string req = "https://apitest.stmosesservices.com/Events/GetEvents/" + _sevantID.ToString();
                HttpResponseMessage response = await client.GetAsync(req);
                string responseData = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Enable case insensitivity
                };
                Events = JsonSerializer.Deserialize<List<Event>>(responseData, options);
            }
            if (Events == null)
                Events = new List<Event>();
        }
    }
}
