using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SM.DAL;
using SM.DAL.DataModel;

namespace SM.APP.Pages.Admin.Class
{
    public class EditModel : PageModel
    {
        private readonly SM.DAL.AppDbContext _context;

        public EditModel(SM.DAL.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public DAL.DataModel.Class Class { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cl =  await _context.Classes.FirstOrDefaultAsync(m => m.ClassID == id);
            if (cl == null)
            {
                return NotFound();
            }
            Class = cl;
           ViewData["MeetingID"] = new SelectList(_context.Meetings, "MeetingID", "MeetingDay");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Class).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClassExists(Class.ClassID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ClassExists(int id)
        {
            return _context.Classes.Any(e => e.ClassID == id);
        }
    }
}
