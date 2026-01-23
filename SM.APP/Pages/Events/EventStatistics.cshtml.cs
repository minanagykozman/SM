using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.APP.Services;

namespace SM.APP.Pages.Events
{
    [Authorize(Policy = "Events.Attendance")]
    public class EventStatisticsModel(ILogger<EventAttendanceModel> logger) : PageModelBase(logger)
    {
       
    }
}
