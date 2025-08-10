using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SM.APP.Services;
using SM.DAL.DataModel;

namespace SM.APP.Pages.Cards
{
    [Authorize(Policy = "Members.ManageCards")]
    public class IndexModel(ILogger<IndexModel> logger) : PageModelBase(logger)
    {
       
    }
}
