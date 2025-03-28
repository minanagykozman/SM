using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.APP.Services;

namespace SM.APP.Pages.Admin.Authenticator
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        public IndexModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        [BindProperty]
        public string Token { get; set; } = string.Empty;
        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);
            var expirationTime = DateTime.UtcNow.AddMinutes(300);
            Token = AuthenticatorService.GenerateToken(user, roles,expirationTime);
        }
    }
}
