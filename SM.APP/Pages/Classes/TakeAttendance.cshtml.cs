using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SM.APP.Services;
using SM.DAL.DataModel;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SM.APP.Pages.Classes
{
    [Authorize(Policy = "Class.Attendance")]
    public class TakeAttendanceModel(ILogger<TakeAttendanceModel> logger) : PageModelBase(logger)
    {

    }
}
