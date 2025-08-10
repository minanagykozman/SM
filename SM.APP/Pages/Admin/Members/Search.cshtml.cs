using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.APP.Services;

namespace SM.APP.Pages.Admin.Members
{
    [Authorize(Policy ="Members.View")]
    public class SearchModel(ILogger<SearchModel> logger) : PageModelBase(logger)
    {
        
    }
}
