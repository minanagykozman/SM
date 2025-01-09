using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.DAL.DataModel
{
    public class Member
    {
        public int MemberID { get; set; }
        public string? UNFirstName { get; set; }

        public string Code { get; set; }
        public string? UNLastName { get; set; }
        public string? Nickname { get; set; }
        public string? UNFileNumber { get; set; }
        public string? UNPersonalNumber { get; set; }
        public string? Mobile { get; set; }
        public bool Baptised { get; set; }
        public DateTime Birthdate { get; set; }
        public char Gender { get; set; }
        public string? School { get; set; }
        public string? Work { get; set; }
        public bool IsMainMember { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }

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

        public ICollection<ClassMember> ClassMembers { get; set; } = new List<ClassMember>();
        public ICollection<EventRegistration> EventRegistrations { get; set; } = new List<EventRegistration>();
        public ICollection<EventAttendance> EventAttendances { get; set; } = new List<EventAttendance>();
        public ICollection<MemberAid> MemberAids { get; set; } = new List<MemberAid>();
        public ICollection<Fund> Funds { get; set; } = new List<Fund>();
    }

}
