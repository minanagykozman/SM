using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NuGet.Common;
using SM.APP.Models;
using System.Text.Json;

namespace SM.APP.Services
{
    public class PageModelBase : PageModel
    {
        internal readonly UserManager<IdentityUser> _userManager;
        internal readonly ILogger<PageModelBase> _logger;
        public UserPermissionsDto UserPermissions { get; set; } = new UserPermissionsDto();

        public PageModelBase( UserManager<IdentityUser> userManager, ILogger<PageModelBase> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            base.OnPageHandlerExecuting(context);

            var permissionsJson = context.HttpContext.Session.GetString("UserPermissions");
            if (!string.IsNullOrEmpty(permissionsJson))
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                UserPermissions = JsonSerializer.Deserialize<UserPermissionsDto>(permissionsJson, options) ?? new UserPermissionsDto();
            }
        }
        public async Task<string> GetAPIToken()
        {
            string jwtToken = Request.Cookies["AuthToken"];
            if (!AuthenticatorService.IsTokenValid(jwtToken))
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var roles = await _userManager.GetRolesAsync(user);
                var expirationTime = DateTime.UtcNow.AddMinutes(SMConfigurationManager.TokenExpiry);
                jwtToken = AuthenticatorService.GenerateToken(user, roles, expirationTime);

                if (SMConfigurationManager.IsDevelopment)
                {
                    Response.Cookies.Append("AuthToken", jwtToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Expires = expirationTime,
                        Path = "/"
                    });
                }
                else
                {
                    Response.Cookies.Append("AuthToken", jwtToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Expires = expirationTime,
                        Domain = ".stmosesservices.com",
                        Path = "/"
                    });
                }
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

