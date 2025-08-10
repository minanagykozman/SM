using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.APP.Services;

namespace SM.APP.Pages.Admin.Servants
{
    [Authorize(Policy = "Servants.Manage")]
    public class EditModel(ILogger<EditModel> logger) : PageModelBase( logger)
    {

    }
}
