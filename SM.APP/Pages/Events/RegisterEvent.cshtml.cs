using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.DAL.DataModel;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace SM.APP.Pages.Events
{
    public class RegisterEventModel : PageModel
    {

        [BindProperty(SupportsGet = true)]
        public string UserCode { get; set; }


        public string MemberStatus { get; set; }


        public Member Member { get; set; }


        public async Task OnGetAsync()
        {
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
                    var responseObj = JsonSerializer.Deserialize<RegistrationStatusResponse>(responseData, options);
                    if (responseObj != null)
                    {
                        switch (responseObj.status)
                        {
                            case "MemeberNotFound":
                                MemberStatus = "Member not found";
                                Member = responseObj.member;
                                break;
                            case "EventNotFound":
                                MemberStatus = "Event not found";
                                Member = responseObj.member;
                                break;
                            case "MemberAlreadyRegistered":
                                MemberStatus = "Member already registered";
                                Member = responseObj.member;
                                break;
                            case "MemberNotEligible":
                                MemberStatus = "Member not eligible";
                                Member = responseObj.member;
                                break;
                            case "ReadyToRegister":
                                MemberStatus = string.Empty;
                                Member = responseObj.member;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }


    }
    public class RegistrationStatusResponse
    {
        public Member member { get; set; }
        public string status { get; set; }
    }

}
