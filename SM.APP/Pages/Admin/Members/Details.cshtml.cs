using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.DAL.DataModel;
using SM.BAL;
using SM.APP.Services;
using Microsoft.AspNetCore.Identity;

namespace SM.APP.Pages.Admin.Members
{
    [Authorize(Policy = "Members.View")]
    public class DetailsModel(ILogger<DetailsModel> logger) : PageModelBase(logger)
    {
        
    }
}
