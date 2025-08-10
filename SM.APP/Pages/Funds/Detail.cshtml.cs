using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.APP.Services;

namespace SM.APP.Pages.Funds
{
    [Authorize(Policy = "Funds.View")]
    public class DetailModel(ILogger<DetailModel> logger) : PageModelBase(logger)
    {

        [BindProperty(SupportsGet = true)]
        public int? Id { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!Id.HasValue)
            {
                return RedirectToPage("/Funds/Index");
            }
            return Page();
        }
    }
} 