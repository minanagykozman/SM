using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.APP.Services;
using SM.DAL.DataModel;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SM.APP.Pages.Classes
{
    [Authorize(Policy = "Class.View")]
    public class ClassMembersModel(ILogger<ClassMembersModel> logger) : PageModelBase(logger)
    {

    }
}
