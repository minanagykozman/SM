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
    [Authorize(Roles = "Admin,Servant")]
    public class RegisterEventModel : PageModelBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        public RegisterEventModel(UserManager<IdentityUser> userManager, ILogger<RegisterEventModel> logger) : base(userManager, logger)
        {
        }

        [BindProperty(SupportsGet = true)]
        public string UserCode { get; set; } = string.Empty;
        [BindProperty]
        public string? Notes { get; set; } = null;
        [BindProperty]
        public int RegisteredCount { get; set; } = 0;

        public string MemberStatus { get; set; } = string.Empty;

        public bool ShowModal { get; set; } = false;

        public RegistrationStatusResponse? MemberData { get; set; }
        public List<Member> EventMembers { get; set; } = new List<Member>();
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
                        string url = string.Format("{0}/Events/GetEventRegisteredMembers", SMConfigurationManager.ApiBase);
                        string req = string.Format("{0}?eventID={1}", url, eventID.ToString());
                        HttpResponseMessage response = await client.GetAsync(req);
                        if (response.IsSuccessStatusCode)
                        {
                            string responseData = await response.Content.ReadAsStringAsync();
                            var options = new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true // Enable case insensitivity
                            };
                            EventMembers = JsonSerializer.Deserialize<List<Member>>(responseData, options);
                            RegisteredCount = EventMembers.Count();
                        }
                        if (EventMembers == null)
                            EventMembers = new List<Member>();
                    }
                    if (!string.IsNullOrEmpty(UserCode))
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                            string url = string.Format("{0}/Events/CheckRegistrationStatus", SMConfigurationManager.ApiBase);
                            string req = string.Format("{0}?memberCode={1}&eventID={2}", url, UserCode, eventID.ToString());
                            HttpResponseMessage response = await client.GetAsync(req);
                            if (response.IsSuccessStatusCode)
                            {
                                string responseData = await response.Content.ReadAsStringAsync();
                                var options = new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true // Enable case insensitivity
                                };
                                MemberData = JsonSerializer.Deserialize<RegistrationStatusResponse>(responseData, options);
                                HttpContext.Session.SetString("MemberData", JsonSerializer.Serialize(MemberData));
                                LoadData();
                            }
                        }
                        UserCode = string.Empty;
                    }
                }
                return Page();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private void LoadData()
        {

            if (MemberData != null)
            {
                switch (MemberData.Status)
                {
                    case RegistrationStatus.MemeberNotFound:
                        MemberStatus = "Member not found";
                        ShowModal = true;
                        break;
                    case RegistrationStatus.EventNotFound:
                        MemberStatus = "Event not found";
                        ShowModal = true;
                        break;
                    case RegistrationStatus.MemberAlreadyRegistered:
                        MemberStatus = "Member already registered";
                        ShowModal = true;
                        break;
                    case RegistrationStatus.MemberNotEligible:
                        MemberStatus = "Member not eligible";
                        ShowModal = true;
                        break;
                    case RegistrationStatus.ReadyToRegister:
                        MemberStatus = "Can Register";
                        ShowModal = true;
                        break;
                    default:
                        break;
                }

            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var sessionData = HttpContext.Session.GetString("MemberData");
                if (!string.IsNullOrEmpty(sessionData))
                {
                    MemberData = JsonSerializer.Deserialize<RegistrationStatusResponse>(sessionData);
                }
                if (MemberData != null)
                {
                    string memberCode = MemberData.Member.Code;
                    int eventID = EventID;
                    bool isException = MemberData.Status == RegistrationStatus.MemberNotEligible ? true : false;
                    string notes = string.IsNullOrEmpty(Notes) ? string.Empty : Notes;
                    var requestData = new
                    {
                        memberCode,
                        eventID,
                        isException,
                        notes
                    };
                    string url = string.Format("{0}/Events/Register", SMConfigurationManager.ApiBase);
                    string request = string.Format("{0}?memberCode={1}&eventID={2}&&isException={3}&notes={4}", url, memberCode, EventID, isException.ToString().ToLower(), notes);
                    string jwtToken = await GetAPIToken();
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                        var jsonContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

                        HttpResponseMessage response = await client.PostAsync(request, jsonContent);

                        if (!response.IsSuccessStatusCode)
                        {
                            throw new Exception($"Error calling API: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                        }
                        UserCode = string.Empty;
                    }
                }
                return RedirectToPage("", new { eventID = EventID });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
    public class RegistrationStatusResponse
    {
        public Member Member { get; set; }
        public RegistrationStatus Status { get; set; }
    }


}
