using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NuGet.Common;

namespace SM.APP.Services
{
    public class PageModelBase : PageModel
    {
        internal readonly ILogger<PageModelBase> _logger;

        public PageModelBase(ILogger<PageModelBase> logger)
        {
            _logger = logger;
            
        }
        
        public async Task<string> GetAPIToken()
        {
            string jwtToken = Request.Cookies["AuthToken"];
            
            return jwtToken;
        }

        public IActionResult HandleException(Exception ex)
        {
            _logger.LogError(ex, User.Identity.Name);
            return RedirectToPage("/Error");
        }
    }
}

