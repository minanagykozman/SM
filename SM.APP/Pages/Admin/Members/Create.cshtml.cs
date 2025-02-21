using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SM.DAL;
using SM.DAL.DataModel;

namespace SM.APP.Pages.Admin.Members
{
    [Authorize(Roles = "Admin,Servant")]
    public class CreateModel : PageModel
    {
        private readonly SM.DAL.AppDbContext _context;

        public CreateModel(SM.DAL.AppDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet(string unFileNumber, bool? showMessage, string code)
        {
            if (!string.IsNullOrEmpty(unFileNumber))
            {
                if (Member == null)
                    Member = new Member();
                Member.UNFileNumber = unFileNumber;
            }
            if (showMessage.HasValue && showMessage.Value && !string.IsNullOrEmpty(code))
            {
                ShowMessage = showMessage.Value;
                Message = string.Format("Member created successfully with code {0}", code);
            }
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
            if (!ModelState.IsValid)
            {
                return Page();
            }
            Member.IsActive = true;
            Member.IsMainMember = Member.Age >= 20;

            int seq = _context.Members.Select(m => m.Sequence).Max() + 1;
            Member.Code = string.Format("{0}{1}-{2}", Member.Birthdate.ToString("yy"), Member.Gender, seq.ToString("0000"));
            Member.Sequence = seq;
            Member.CardStatus = string.IsNullOrEmpty(Member.ImageReference) ? "MissingPhoto" : "ReadyToPrint";
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time"); // Example for UTC+2
            DateTime utcNow = DateTime.UtcNow;
            DateTime utcPlus2 = TimeZoneInfo.ConvertTimeFromUtc(utcNow, tz);
            Member.LastModifiedDate = utcPlus2;
            _context.Members.Add(Member);
            await _context.SaveChangesAsync();

            string action = Request.Form["action"]; // Get the button value

            if (action == "AddAnother")
            {

                return RedirectToPage("", new { unFileNumber = Member.UNFileNumber, showMessage = true, code = Member.Code });
            }
            else
            {
                return RedirectToPage("", new { unFileNumber = "", showMessage = true, code = Member.Code });
            }
        }
    }
}
