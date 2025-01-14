using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SM.DAL;
using SM.DAL.DataModel;

namespace SM.APP.Pages.General
{
    public class FamilyModel : PageModel
    {
        private readonly SM.DAL.AppDbContext _context;

        public FamilyModel(SM.DAL.AppDbContext context)
        {
            _context = context;
        }

        public IList<Member> Member { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (!string.IsNullOrEmpty(SearchString))
            {
                var member = _context.Members.Where(m => m.UNFileNumber == SearchString || m.Code == SearchString).FirstOrDefault();
                if (member != null)
                    Member = await _context.Members.Where(m => m.UNFileNumber == member.UNFileNumber).OrderBy(m => m.Birthdate).ToListAsync();
                else
                    Member = new List<DAL.DataModel.Member>();
            }
            else
                Member = new List<DAL.DataModel.Member>();
        }

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }
    }
}
