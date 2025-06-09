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
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.Ocsp;
using SM.APP.Services;
using SM.DAL;
using SM.DAL.DataModel;

namespace SM.APP.Pages.Admin.Members
{
    [Authorize(Roles = "Admin,Servant")]
    public class CreateModel(UserManager<IdentityUser> userManager, ILogger<CreateModel> logger) : PageModelBase(userManager, logger)
    {
        public async Task<IActionResult> OnGet(string unFileNumber, bool? showMessage, string code)
        {
            await GetAPIToken();
            return Page();
        }

        [BindProperty]
        public Member Member { get; set; } = default!;

        [BindProperty]
        public bool ShowMessage { get; set; } = false;
        [BindProperty]
        public string? Message { get; set; } = string.Empty;

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
                string request = string.Format("{0}/Member/CreateMember", SMConfigurationManager.ApiBase);
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
                    string code = await response.Content.ReadAsStringAsync();
                    string action = Request.Form["action"]; // Get the button value

                    if (action == "AddAnother")
                    {
                        return RedirectToPage("", new { unFileNumber = Member.UNFileNumber, showMessage = true, code = code });
                    }
                    else
                    {
                        return RedirectToPage("", new { unFileNumber = "", showMessage = true, code = code });
                    }
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
