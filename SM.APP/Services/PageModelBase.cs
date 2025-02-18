using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SM.APP.Services
{
    public class PageModelBase : PageModel
    {
        internal readonly UserManager<IdentityUser> _userManager;
        public PageModelBase(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
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
    }
}

