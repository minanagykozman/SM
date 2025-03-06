using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.APP.Services;
using SM.DAL.DataModel;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SM.APP.Pages.Classess
{
    public class ClassMembersModel(UserManager<IdentityUser> userManager, ILogger<ClassMembersModel> logger) : PageModelBase(userManager, logger)
    {
        public void OnGet()
        {
        }
    }
}
