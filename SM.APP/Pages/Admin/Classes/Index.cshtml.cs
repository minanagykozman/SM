using Microsoft.AspNetCore.Authorization;
using SM.APP.Services;

namespace SM.APP.Pages.Admin.Classes
{
    [Authorize(Policy = "Class.Manage")]
    public class IndexModel(ILogger<IndexModel> logger) : PageModelBase(logger)
    {

    }
}
