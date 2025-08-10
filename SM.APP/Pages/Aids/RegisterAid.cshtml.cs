using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.APP.Services;

namespace SM.APP.Pages.Aids
{
    [Authorize(Policy = "Aids.Register")]
    public class RegisterAidModel(ILogger<RegisterAidModel> logger) : PageModelBase(logger)
    {
       
    }
}
