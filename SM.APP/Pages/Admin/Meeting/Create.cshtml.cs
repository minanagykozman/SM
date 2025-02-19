using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SM.DAL;
using SM.DAL.DataModel;

namespace SM.APP.Pages.Admin.Meeting
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
            return Page();
        }

        [BindProperty]
        public DAL.DataModel.Meeting Meeting { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Meetings.Add(Meeting);
            await _context.SaveChangesAsync();

            return RedirectToPage("");
        }
    }
}
