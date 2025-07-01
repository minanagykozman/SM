using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.APP.Services;

namespace SM.APP.Pages.Funds
{
    [Authorize]
    public class DetailModel : PageModelBase
    {
        public DetailModel(UserManager<IdentityUser> userManager, ILogger<DetailModel> logger) : base(userManager, logger)
        {
        }

        [BindProperty(SupportsGet = true)]
        public int? Id { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!Id.HasValue)
            {
                return RedirectToPage("/Funds/Index");
            }
            await GetAPIToken();
            return Page();
        }
    }
} 