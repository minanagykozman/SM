using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.APP.Services;

namespace SM.APP.Pages.Funds
{
    public class IndexModel(UserManager<IdentityUser> userManager, ILogger<IndexModel> logger) : PageModelBase(userManager, logger)
    {
        public async Task<IActionResult> OnGet(string unFileNumber, bool? showMessage, string code)
        {
            await GetAPIToken();
            return Page();
        }
    }
}
