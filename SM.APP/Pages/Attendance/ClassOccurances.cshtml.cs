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
        public ClassOccurancesModel(UserManager<IdentityUser> userManager) : base(userManager)
        {


        }
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
                            TodayClasses = Classes.Where(c => c.ClassOccurrenceStartDate.Date == DateTime.Now.Date).ToList();
                            UpcomingClasses = Classes.Where(c => c.ClassOccurrenceStartDate.Date > DateTime.Now.Date).ToList();
                            PastClasses = Classes.Where(c => c.ClassOccurrenceStartDate.Date < DateTime.Now.Date).ToList();
                        }
                    }
                }
            }
        }
    }
}
