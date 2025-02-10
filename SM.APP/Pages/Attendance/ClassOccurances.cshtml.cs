using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.APP.Services;
using SM.DAL.DataModel;
using System.Text.Json;

namespace SM.APP.Pages.Attendance
{
    public class ClassOccurancesModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public List<ClassOccurrence> TodayClasses { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<ClassOccurrence> UpcomingClasses { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<ClassOccurrence> PastClasses { get; set; }
        public async Task OnGetAsync(int? classID)
        {
            if (classID.HasValue)
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = string.Format("{0}/Meeting/GetClassOccurences", SMConfigurationManager.ApiBase);
                    string req = string.Format("{0}?classID={1}", url, classID.Value.ToString());
                    HttpResponseMessage response = await client.GetAsync(req);
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
                        TodayClasses = Classes.Where(c => c.ClassOccurrenceStartDate.Date == DateTime.Now.Date).ToList();
                        UpcomingClasses = Classes.Where(c => c.ClassOccurrenceStartDate.Date > DateTime.Now.Date).ToList();
                        PastClasses = Classes.Where(c => c.ClassOccurrenceStartDate.Date < DateTime.Now.Date).ToList();
                    }

                }
            }
        }
    }
}
