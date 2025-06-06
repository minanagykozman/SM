using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.APP.Services;

namespace SM.APP.Pages.Admin.Events
{
    public class EditModel(UserManager<IdentityUser> userManager, ILogger<EditModel> logger) : PageModelBase(userManager, logger)
    {
        public async Task<IActionResult> OnGetAsync()
        {
            await GetAPIToken();
            return Page();
        }
    }
}
