using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.APP.Services;
using SM.DAL.DataModel;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SM.APP.Pages.Attendance
{
    public class ClassOccurancesModel : PageModelBase
    {
        public ClassOccurancesModel(UserManager<IdentityUser> userManager, ILogger<ClassOccurancesModel> logger) : base(userManager, logger)
        {
        }
        #region Properties
        [BindProperty(SupportsGet = true)]
        public List<ClassOccurrence> TodayClasses { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<ClassOccurrence> UpcomingClasses { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<ClassOccurrence> PastClasses { get; set; }
        #endregion
        public async Task<IActionResult> OnGetAsync(int? classID)
        {
            try
            {
                if (classID.HasValue)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string jwtToken = await GetAPIToken();
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                        string url = string.Format("{0}/Meeting/GetClassOccurences", SMConfigurationManager.ApiBase);
                        string req = string.Format("{0}?classID={1}", url, classID.Value.ToString());
                        HttpResponseMessage response = await client.GetAsync(req);
                        if (response.IsSuccessStatusCode)
                        {
                            string responseData = await response.Content.ReadAsStringAsync();
                            var options = new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true // Enable case insensitivity
                            };
                            List<ClassOccurrence> Classes = JsonSerializer.Deserialize<List<ClassOccurrence>>(responseData, options);
                            if (Classes == null)
                            {
                                TodayClasses = new List<ClassOccurrence>();
                                UpcomingClasses = new List<ClassOccurrence>();
                                PastClasses = new List<ClassOccurrence>();
                            }
                            else
                            {
                                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time"); // Example for UTC+2
                                DateTime utcNow = DateTime.UtcNow;
                                DateTime utcPlus2 = TimeZoneInfo.ConvertTimeFromUtc(utcNow, tz);
                                
                                TodayClasses = Classes.Where(c => c.ClassOccurrenceStartDate.Date == utcPlus2.Date).ToList();
                                //UpcomingClasses = Classes.Where(c => c.ClassOccurrenceStartDate.Date > utcPlus2.Date).ToList();
                                PastClasses = Classes.Where(c => c.ClassOccurrenceStartDate.Date < utcPlus2.Date).ToList();
                            }
                        }
                    }
                }
                return Page();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
