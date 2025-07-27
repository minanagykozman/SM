using Microsoft.AspNetCore.Razor.TagHelpers;
using SM.APP.Models;

namespace SM.APP.Services
{
    [HtmlTargetElement(Attributes = "auth-policy")]
    public class AuthorizeTagHelper : TagHelper
    {
        [HtmlAttributeName("auth-policy")]
        public string Policy { get; set; }

        [HtmlAttributeName("auth-permissions")]
        public UserPermissionsDto Permissions { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrEmpty(Policy) || Permissions == null || !Permissions.Permissions.ContainsKey(Policy) || !Permissions.Permissions[Policy])
            {
                output.SuppressOutput();
            }
        }
    }

}
