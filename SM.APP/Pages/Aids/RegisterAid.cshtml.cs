using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.APP.Services;

namespace SM.APP.Pages.Aids
{
    public class RegisterAidModel(UserManager<IdentityUser> userManager, ILogger<RegisterAidModel> logger) : PageModelBase(userManager, logger)
    {
        public async Task<IActionResult> OnGetAsync()
        {
            await GetAPIToken();
            return Page();
        }
    }
}
