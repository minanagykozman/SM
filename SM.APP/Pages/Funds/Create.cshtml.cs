using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SM.APP.Services;
using SM.DAL.DataModel;

namespace SM.APP.Pages.Funds
{
    [Authorize(Policy ="Funds.Manage")]
    public class CreateModel(ILogger<CreateModel> logger) : PageModelBase(logger)
    {
        public async Task<IActionResult> OnGet(string unFileNumber, bool? showMessage, string code)
        {
            await GetAPIToken();
            return Page();
        }

        [BindProperty]
        public Member Member { get; set; } = default!;
    }
}
