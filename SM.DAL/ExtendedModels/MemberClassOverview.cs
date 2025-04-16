using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.DAL.DataModel
{
    public class MemberClassOverview
    {
        public string ClassName { get; set; } = string.Empty;
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? LastPresentDate { get; set; } = null;
        public string Attendance { get; set; } = string.Empty;
        public string? Servant { get; set; } = string.Empty;
    }
}
