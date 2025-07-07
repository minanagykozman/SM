using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.APP.Services;

namespace SM.APP.Pages.Admin.Members
{
    public class SearchModel(UserManager<IdentityUser> userManager, ILogger<SearchModel> logger) : PageModelBase(userManager, logger)
    {
        public async Task<IActionResult> OnGet(string unFileNumber, bool? showMessage, string code)
        {
            await GetAPIToken();
            return Page();
        }

    }
}
