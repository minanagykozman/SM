using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.APP.Services;
using SM.DAL.DataModel;
using System.Text.Json;

namespace SM.APP.Pages.Attendance
{
    [Authorize(Roles = "Admin,Servant")]
    public class IndexModel : PageModel
    {
        int _sevantID = 0;
        private readonly UserManager<IdentityUser> _userManager;
        public IndexModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;

        }
        [BindProperty(SupportsGet = true)]
        public List<Class> Classes { get; set; }
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
                string req = "https://apitest.stmosesservices.com/Meeting/GetClasses?servantID="+ _sevantID;
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
