using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.APP.Services;
using SM.DAL.DataModel;

namespace SM.APP.Pages.Admin.Aids
{
    [Authorize(Roles = "Admin")]
    public class CreateModel(UserManager<IdentityUser> userManager, ILogger<CreateModel> logger) : PageModelBase(userManager, logger)
    {
        public async Task<IActionResult> OnGetAsync()
        {
            await GetAPIToken();
            return Page();
        }
    }
}
