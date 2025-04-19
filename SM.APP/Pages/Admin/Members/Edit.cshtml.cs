using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SM.APP.Services;
using SM.DAL;
using SM.DAL.DataModel;

namespace SM.APP.Pages.Admin.Members
{
    [Authorize(Roles = "Admin,Servant")]
    public class EditModel(UserManager<IdentityUser> userManager, ILogger<EditModel> logger) : PageModelBase(userManager, logger)
    {
        [BindProperty]
        public Member Member { get; set; } = default!;

        public List<EventRegistration> EventRegistrations { get; set; } = new();
        public List<ClassMember> ClassMemberships { get; set; } = new();
        public List<ClassAttendance> ClassAttendances { get; set; } = new();
        public List<MemberAid> MemberAids { get; set; } = new();
        public List<Fund> Funds { get; set; } = new();


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                using (HttpClient client = new HttpClient())
                {
                    string jwtToken = await GetAPIToken();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                    string url = string.Format("{0}/Member/GetMember", SMConfigurationManager.ApiBase);
                    string req = string.Format("{0}?memberID={1}", url, id);
                    HttpResponseMessage response = await client.GetAsync(req);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true // Enable case insensitivity
                        };
                        Member member = JsonSerializer.Deserialize<Member>(responseData, options);
                        if (member == null)
                        {
                            return NotFound();
                        }
                        Member = member;
                    }
                }
                return Page();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }


                var requestData = new
                {
                    memberID = Member.MemberID,
                    code = Member.Code,
                    unFirstName = Member.UNFirstName,
                    unLastName = Member.UNLastName,
                    nickname = Member.Nickname,
                    unFileNumber = Member.UNFileNumber,
                    unPersonalNumber = Member.UNPersonalNumber,
                    mobile = Member.Mobile,
                    baptised = Member.Baptised,
                    birthdate = Member.Birthdate,
                    gender = Member.Gender,
                    school = Member.School,
                    work = Member.Work,
                    isMainMember = Member.IsMainMember,
                    isActive = Member.IsActive,
                    imageReference = Member.ImageReference,
                    cardStatus = Member.CardStatus,
                    notes = Member.Notes,
                    imageURL = Member.ImageURL,
                    sequence = Member.Sequence
                };
                string request = string.Format("{0}/Member/UpdateMember", SMConfigurationManager.ApiBase);
                using (HttpClient client = new HttpClient())
                {
                    string jwtToken = await GetAPIToken();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                    var jsonContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(request, jsonContent);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Error calling API: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                    }

                }


                return RedirectToPage("./Index", new { userCode = Member.Code });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
