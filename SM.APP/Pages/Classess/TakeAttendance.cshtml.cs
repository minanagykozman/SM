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
    [Authorize(Roles = "Admin,Servant")]
    public class TakeAttendanceModel(UserManager<IdentityUser> userManager, ILogger<TakeAttendanceModel> logger) : PageModelBase(userManager, logger)
    {
        public List<Member> ClassOccurenceMembers { get; set; } = new List<Member>();

        public void OnGet()
        {
        }

    }
}
