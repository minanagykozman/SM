using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SM.APP.Services
{
    public class PageModelBase : PageModel
    {
        internal readonly UserManager<IdentityUser> _userManager;
        internal readonly ILogger<PageModelBase> _logger;

        public PageModelBase(UserManager<IdentityUser> userManager, ILogger<PageModelBase> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }
        public async Task<string> GetAPIToken()
        {
            string jwtToken = Request.Cookies["AuthToken"];
            if (!AuthenticatorService.IsTokenValid(jwtToken))
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var roles = await _userManager.GetRolesAsync(user);
                jwtToken = AuthenticatorService.GenerateToken(user, roles);
            }
            return jwtToken;
        }

        public IActionResult HandleException(Exception ex)
        {
            _logger.LogError(ex, User.Identity.Name);
            return RedirectToPage("/Error");
        }
    }
}

