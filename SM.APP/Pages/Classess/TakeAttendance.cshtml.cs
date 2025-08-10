using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SM.APP.Services;
using SM.DAL.DataModel;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SM.APP.Pages.Classess
{
    [Authorize(Policy = "Class.Attendance")]
    public class TakeAttendanceModel(ILogger<TakeAttendanceModel> logger) : PageModelBase(logger)
    {
        public List<Member> ClassOccurenceMembers { get; set; } = new List<Member>();
        public async Task<IActionResult> OnGetAsync()
        {
            await GetAPIToken();
            return Page();
        }
    }
}
