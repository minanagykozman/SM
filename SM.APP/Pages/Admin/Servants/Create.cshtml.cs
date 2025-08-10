using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.APP.Services;
using SM.DAL.DataModel;

namespace SM.APP.Pages.Admin.Servants
{
    [Authorize(Policy ="Servants.Manage")]
    public class CreateModel(ILogger<CreateModel> logger) : PageModelBase( logger)
    {


    }
}
