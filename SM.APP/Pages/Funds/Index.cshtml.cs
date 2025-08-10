using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.APP.Services;

namespace SM.APP.Pages.Funds
{
    [Authorize(Policy = "Funds.View")]
    public class IndexModel(ILogger<IndexModel> logger) : PageModelBase(logger)
    {
        
    }
}
