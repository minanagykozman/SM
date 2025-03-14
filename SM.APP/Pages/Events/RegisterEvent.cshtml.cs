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
    public class RegisterEventModel(UserManager<IdentityUser> userManager, ILogger<RegisterEventModel> logger) : PageModelBase(userManager, logger)
    {
        public void OnGet()
        {

        }



    }


}
