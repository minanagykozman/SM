using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SM.APP.Services;
using SM.DAL.DataModel;

namespace SM.APP.Pages.Admin.Members
{
    [Authorize(Policy = "Members.View")]
    public class CreateModel(ILogger<CreateModel> logger) : PageModelBase(logger)
    {
        
    }
}
