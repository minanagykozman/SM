using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.DAL.DataModel;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using SM.APP.Services;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace SM.APP.Pages.Events
{
    [Authorize(Policy = "Events.View")]
    public class ListEventMembersModel(ILogger<ListEventMembersModel> logger) : PageModelBase(logger)
    {
        public int RegisteredCount { get; set; } = 0;

        public List<MemberEventView> EventMembers { get; set; } = new List<MemberEventView>();
        [BindProperty]
        public int EventID { get; set; }

        public async Task<IActionResult> OnGetAsync(int? eventID)
        {
            try
            {
                if (eventID.HasValue)
                {
                    EventID = eventID.Value;
                    string jwtToken = await GetAPIToken();
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                        string url = string.Format("{0}/Events/GetEventMembers", SMConfigurationManager.ApiBase);
                        string req = string.Format("{0}?eventID={1}&registered=true", url, eventID.ToString());
                        HttpResponseMessage response = await client.GetAsync(req);
                        if (response.IsSuccessStatusCode)
                        {
                            string responseData = await response.Content.ReadAsStringAsync();
                            var options = new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true // Enable case insensitivity
                            };
                            EventMembers = JsonSerializer.Deserialize<List<MemberEventView>>(responseData, options)!;
                            RegisteredCount = EventMembers.Count();
                        }
                        if (EventMembers == null)
                            EventMembers = new List<MemberEventView>();
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
