using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SM.DAL.DataModel;
using System.Buffers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace SM.APP.Pages.Events
{
    public class RegisterEventModel : PageModel
    {

        [BindProperty(SupportsGet = true)]
        public string UserCode { get; set; }


        public string MemberStatus { get; set; }

        public bool ShowModal { get; set; } = false;
        public static RegistrationStatusResponse? MemberData { get; set; }
        public List<Member> EventMembers { get; set; } = new List<Member>();

        public async Task OnGetAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                string req = "http://ec2-98-81-132-234.compute-1.amazonaws.com/Events/GetEventRegisteredMembers?eventID=1";
                HttpResponseMessage response = await client.GetAsync(req);
                string responseData = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Enable case insensitivity
                };
                EventMembers = JsonSerializer.Deserialize<List<Member>>(responseData, options);
                if (EventMembers == null)
                    EventMembers = new List<Member>();
            }
            if (!string.IsNullOrEmpty(UserCode))
            {
                using (HttpClient client = new HttpClient())
                {
                    string req = "http://ec2-98-81-132-234.compute-1.amazonaws.com/Events/CheckRegistrationStatus?memberCode=" + UserCode + "&eventID=1";
                    HttpResponseMessage response = await client.GetAsync(req);
                    string responseData = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true // Enable case insensitivity
                    };
                    MemberData = JsonSerializer.Deserialize<RegistrationStatusResponse>(responseData, options);
                    LoadData();
                }
                UserCode = string.Empty;
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
            if (MemberData != null)
            {
                string memberCode = MemberData.Member.Code;
                int eventID = 1;
                int servantID = 1;
                bool isException = MemberData.Status == RegistrationStatus.MemberNotEligible ? true : false;
                string notes = MemberData.Member.Notes;
                var requestData = new
                {
                    memberCode,
                    eventID,
                    servantID,
                    isException,
                    notes
                };

                string apiUrl = "http://ec2-98-81-132-234.compute-1.amazonaws.com/Events/Register";
                string request = string.Format("{0}?memberCode={1}&eventID=1&servantID=1&isException=false&notes=NA", apiUrl, MemberData.Member.Code);
                using (HttpClient client = new HttpClient())
                {
                    var jsonContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(request, jsonContent);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        throw new Exception($"Error calling API: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                    }
                    UserCode = string.Empty;

                }
            }
            return RedirectToPage();
        }
    }
    public class RegistrationStatusResponse
    {
        public Member Member { get; set; }
        public RegistrationStatus Status { get; set; }
    }
    public enum RegistrationStatus
    {
        MemeberNotFound,
        EventNotFound,
        MemberNotEligible,
        MemberAlreadyRegistered,
        ReadyToRegister,
        Ok,
        Error
    }

}
