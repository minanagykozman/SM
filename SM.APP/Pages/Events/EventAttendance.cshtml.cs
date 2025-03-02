using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.DAL.DataModel;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using SM.APP.Services;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace SM.APP.Pages.Events
{
    [Authorize(Roles = "Admin,Servant")]
    public class EventAttendanceModel(UserManager<IdentityUser> userManager, ILogger<EventAttendanceModel> logger) : PageModelBase(userManager, logger)
    {
        [BindProperty(SupportsGet = true)]
        public string UserCode { get; set; } = string.Empty;
        [BindProperty]
        public string? Notes { get; set; } = null;
        [BindProperty]
        public int RegisteredCount { get; set; } = 0;

        public string MemberStatus { get; set; } = string.Empty;

        public bool ShowModal { get; set; } = false;

        public RegistrationStatusResponse? MemberData { get; set; }
        public List<Member> EventMembers { get; set; } = new List<Member>();
        [BindProperty]
        public int EventID { get; set; }

        public async Task<IActionResult> OnGetAsync(int? eventID)
        {
            try
            {
                return Page();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

    }
}
