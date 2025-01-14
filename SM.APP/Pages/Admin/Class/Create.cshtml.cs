using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SM.DAL;
using SM.DAL.DataModel;

namespace SM.APP.Pages.Admin.Class
{
    public class CreateModel : PageModel
    {
        private readonly SM.DAL.AppDbContext _context;

        public CreateModel(SM.DAL.AppDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["MeetingID"] = new SelectList(_context.Meetings, "MeetingID", "MeetingName");
            return Page();
        }

        [BindProperty]
        public DAL.DataModel.Class Class { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Classes.Add(Class);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
