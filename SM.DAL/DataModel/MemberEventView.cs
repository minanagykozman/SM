using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SM.DAL.DataModel
{
    public class MemberEventView
    {
        public int MemberID { get; set; }
        [MaxLength(10)]
        public string Code { get; set; } = string.Empty;
        [Display(Name = "First name")]
        public string? UNFirstName { get; set; }
        [Required]
        [Display(Name = "Last name")]
        public string? UNLastName { get; set; }
        public string? Nickname { get; set; }
        [Display(Name = "UN File Number")]
        public string? UNFileNumber { get; set; }
        [Display(Name = "UN Number")]
        public string? UNPersonalNumber { get; set; }
        [Display(Name = "Card Status")]
        public string? CardStatus { get; set; }
        public int Sequence{ get; set; }
        public string? Mobile { get; set; }
        public bool Baptised { get; set; }
        [DataType(DataType.Date)]
        public DateTime Birthdate { get; set; }
        public char Gender { get; set; }
        public bool IsMainMember { get; set; }
        [Display(Name = "Image Reference")]
        public string? ImageReference { get; set; }
        public string? Notes { get; set; }
        public bool Registered { get; set; }
        public int EventID { get; set; }
        public bool? IsException { get; set; }
        public int? ServantID { get; set; }
        public DateTime? TimeStamp { get; set; }
        public string? RegistrationNotes { get; set; }
        public bool? Attended { get; set; }
        public int? AttendanceServantID { get; set; }
        public DateTime? AttendanceTimeStamp { get; set; }
        public string? Team { get; set; }
        public string? Bus { get; set; }
        public float? Paid { get; set; } = null;
        public int Age
        {
            get
            {
                DateTime today = DateTime.Today; // Current date
                int age = today.Year - Birthdate.Year;

                // Adjust age if the birthdate hasn't occurred yet this year
                if (Birthdate > today.AddYears(-age))
                {
                    age--;
                }

                return age;
            }
        }
        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", UNFirstName, UNLastName);
            }
        }
       
    }

}
