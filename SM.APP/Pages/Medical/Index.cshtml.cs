using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.APP.Services;

namespace SM.APP.Pages.Medical
{
    [Authorize(Policy = "Medical.View")]
    public class IndexModel(ILogger<IndexModel> logger) : PageModelBase(logger)
    {
        
    }
}
