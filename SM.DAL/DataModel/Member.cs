using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SM.DAL.DataModel
{
    public class Member
    {
        public int MemberID { get; set; }
        [MaxLength(10)]
        public string Code { get; set; }
        [Required]
        [Display(Name = "First name")]
        [MaxLength(50)]
        public string? UNFirstName { get; set; }
        [Required]
        [Display(Name = "Last name")]
        [MaxLength(100)]
        public string? UNLastName { get; set; }
        public string? Nickname { get; set; }
        [Display(Name = "UN File Number")]
        public string UNFileNumber { get; set; }
        [Display(Name = "UN Number")]
        [MaxLength(50)]
        [Required]
        public string UNPersonalNumber { get; set; }
        public string? Mobile { get; set; }
        public bool Baptised { get; set; }
        [DataType(DataType.Date)]
        public DateTime Birthdate { get; set; }
        public char Gender { get; set; }
        public string? School { get; set; }
        public string? Work { get; set; }
        public bool IsMainMember { get; set; }
        public bool IsActive { get; set; }
        [Display(Name = "Image Reference")]
        public string? ImageReference { get; set; }
        [Display(Name = "Card Status")]
        [MaxLength(20)]
        public string? CardStatus { get; set; }
        public string? Notes { get; set; }
        public string? ImageURL { get; set; } = null;
        public int Sequence { get; set; } = 0;
        [Display(Name = "Last Modified")]
        public DateTime? ModifiedAt { get; set; } = null;
        public int? ModifiedBy { get; set; } = null;
        public DateTime? CreatedAt { get; set; } = null;
        public int? CreatedBy { get; set; } = null;
        public string? ModifiedLog { get; set; } = null;
        [JsonIgnore]
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
        [JsonIgnore]
        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", UNFirstName, UNLastName);
            }
        }
        [JsonIgnore]
        public CardStatus? CardStatusDisplay
        {
            get
            {
                try
                {
                    return (CardStatus)Enum.Parse(typeof(CardStatus), CardStatus);
                }
                catch
                { return null; }
            }
        }
        [JsonIgnore]
        public virtual ICollection<ClassMember> ClassMembers { get; set; } = new List<ClassMember>();
        [JsonIgnore]
        public virtual ICollection<EventRegistration> EventRegistrations { get; set; } = new List<EventRegistration>();
        [JsonIgnore]
        public virtual ICollection<MemberAid> MemberAids { get; set; } = new List<MemberAid>();
        [JsonIgnore]
        public virtual ICollection<Fund> Funds { get; set; } = new List<Fund>();
    }

}
