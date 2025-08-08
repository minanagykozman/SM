using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SM.APP.Services
{
    [HtmlTargetElement(Attributes = "auth-policy")]
    public class AuthorizeTagHelper : TagHelper
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthorizeTagHelper(IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor)
        {
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
        }

        // This public property will receive the value from the auth-policy attribute.
        // For example, if you have <div auth-policy="Funds.Manage">,
        // this property will be set to "Funds.Manage".
        [HtmlAttributeName("auth-policy")]
        public string Policy { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            // Get the current user from the HttpContext.
            var user = _httpContextAccessor.HttpContext.User;

            // If a policy name has been provided, check it.
            if (!string.IsNullOrEmpty(Policy))
            {
                // Use the authorization service to check if the user meets the policy.
                var isAuthorized = await _authorizationService.AuthorizeAsync(user, Policy);

                // If the authorization check fails...
                if (!isAuthorized.Succeeded)
                {
                    // ...completely suppress the output. The element and its children
                    // will not be rendered in the final HTML.
                    output.SuppressOutput();
                }
            }
        }
    }
}
