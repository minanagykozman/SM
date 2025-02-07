using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.DAL.DataModel;
using System.Text;
using System.Text.Json;

namespace SM.APP.Pages.Attendance
{
    [Authorize(Roles = "Admin,Servant")]
    public class TakeAttendanceModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string UserCode { get; set; } = string.Empty;
        [BindProperty]
        public string? Notes { get; set; } = null;

        public string MemberStatus { get; set; } = string.Empty;

        public bool ShowModal { get; set; } = false;
        public static AttendanceStatusResponse? MemberData { get; set; }
        public List<Member> ClassOccurenceMembers { get; set; } = new List<Member>();
        public int ClassOccurenceID;
        public async Task OnGetAsync(int? classOccurenceID)
        {
            if (classOccurenceID.HasValue)
            {
                ClassOccurenceID = classOccurenceID.Value;
                if (!string.IsNullOrEmpty(UserCode))
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string req = "https://apitest.stmosesservices.com/Meeting/CheckAteendance?classOccuranceID=" + classOccurenceID + "&memberCode=" + UserCode;
                        HttpResponseMessage response = await client.GetAsync(req);
                        string responseData = await response.Content.ReadAsStringAsync();
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true // Enable case insensitivity
                        };
                        MemberData = JsonSerializer.Deserialize<AttendanceStatusResponse>(responseData, options);
                    }
                    if (MemberData != null)
                    {
                        if (MemberData.Status == AttendanceStatus.Ready)
                        {
                            StringContent jsonContent;
                            string request = TakeAttendanceString(MemberData.Member.Code, ClassOccurenceID, 1, false, out jsonContent);
                            using (HttpClient client = new HttpClient())
                            {
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
                        else
                        { LoadData(); }
                    }
                    UserCode = string.Empty;
                }
                using (HttpClient client = new HttpClient())
                {
                    string req = "https://apitest.stmosesservices.com/Meeting/GetAttendedMembers?occurenceID=" + ClassOccurenceID.ToString(); ;
                    HttpResponseMessage response = await client.GetAsync(req);
                    string responseData = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true // Enable case insensitivity
                    };
                    ClassOccurenceMembers = JsonSerializer.Deserialize<List<Member>>(responseData, options);
                    if (ClassOccurenceMembers == null)
                        ClassOccurenceMembers = new List<Member>();
                }
            }
        }

        private void LoadData()
        {

            switch (MemberData.Status)
            {
                case AttendanceStatus.MemberNotFound:
                    MemberStatus = "Member not found";
                    ShowModal = true;
                    break;
                case AttendanceStatus.ClassNotFound:
                    MemberStatus = "Class not found";
                    ShowModal = true;
                    break;
                case AttendanceStatus.AlreadyAttended:
                    MemberStatus = "Member already had attendance";
                    ShowModal = true;
                    break;
                case AttendanceStatus.NotRegisteredInClass:
                    MemberStatus = "Member not registered in class";
                    ShowModal = true;
                    break;
                case AttendanceStatus.Ready:
                    MemberStatus = "Can Attend";
                    ShowModal = false;
                    break;
                default:
                    break;
            }


        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (MemberData != null)
            {
                StringContent jsonContent;
                string request = TakeAttendanceString(MemberData.Member.Code, ClassOccurenceID, 1, true, out jsonContent);
                using (HttpClient client = new HttpClient())
                {

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

        private string TakeAttendanceString(string memberCode, int classOccuranceID, int servantID, bool forceRegister, out StringContent jsonContent)
        {
            var requestData = new
            {
                classOccuranceID,
                memberCode,
                servantID,
                forceRegister
            };

            string apiUrl = "https://apitest.stmosesservices.com/Meeting/TakeAttendance";
            string request = string.Format("{0}?classOccuranceID={0}&memberCode={1}&servantID={2}&forceRegister={3}", apiUrl, ClassOccurenceID, 1, true);
            jsonContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            return request;
        }
    }
    public class AttendanceStatusResponse
    {
        public Member Member { get; set; }
        public AttendanceStatus Status { get; set; }
    }
    public enum AttendanceStatus
    {
        NotRegisteredInClass,
        MemberNotFound,
        ClassNotFound,
        AlreadyAttended,
        Ready,
        Ok
    }
}
