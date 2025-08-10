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
    [Authorize(Policy = "Events.Attendance")]
    public class EventAttendanceModel(ILogger<EventAttendanceModel> logger) : PageModelBase(logger)
    {
        public List<MemberEventView> EventMembers { get; set; } = new List<MemberEventView>();
        public async Task<IActionResult> OnGetAsync()
        {
            await GetAPIToken();
            return Page();
        }
    }
}
