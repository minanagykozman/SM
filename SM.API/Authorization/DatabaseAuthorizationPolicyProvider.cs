using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace SM.API.Authorization
{
    public class DatabaseAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public DatabaseAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options) { }

        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            var policy = await base.GetPolicyAsync(policyName);

            if (policy == null)
            {
                policy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new PermissionRequirement(policyName))
                    .Build();
            }

            return policy;
        }
    }
}
