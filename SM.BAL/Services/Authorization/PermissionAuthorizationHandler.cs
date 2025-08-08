using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.BAL.Services.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly UserManager<IdentityUser> _userManager;
        //private readonly PermissionService _permissionService; // Your BAL service

        public PermissionAuthorizationHandler(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            //_permissionService = permissionService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var user = await _userManager.GetUserAsync(context.User);
            if (user == null)
            {
                return; // No user, so requirement is not met
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            using (PermissionHandler handler = new PermissionHandler())
            {
                if (await handler.HasPermissionAsync(userRoles, requirement.Permission))
                {
                    // If they have the permission, the requirement is met
                    context.Succeed(requirement);
                }
            }
        }
    }
}
