using Microsoft.AspNetCore.Authorization;
using SM.BAL;
using System.Security.Claims;

namespace SM.API.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {


        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var userRoleClaims = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value);

            using (AuthorizationHandler handler = new AuthorizationHandler())
            {
                bool hasPermission = await handler.HasPermissionAsync(userRoleClaims, requirement.PermissionName);

                if (hasPermission)
                {
                    context.Succeed(requirement);
                }
            }

        }
    }
}
