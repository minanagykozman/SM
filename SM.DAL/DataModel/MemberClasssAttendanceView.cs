﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SM.DAL.DataModel
{
    public class MemberClasssAttendanceView
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
        public string? Mobile { get; set; }
        public bool Baptised { get; set; }
        [DataType(DataType.Date)]
        public DateTime Birthdate { get; set; }
        public char Gender { get; set; }
        public bool IsMainMember { get; set; }
        [Display(Name = "Image Reference")]
        public string? ImageReference { get; set; }
        public string? Notes { get; set; }
        public bool Present { get; set; }
        public int ClassID { get; set; }
        public int ClassOccurrenceID { get; set; }
        public int? ServantID { get; set; }
        public DateTime? TimeStamp { get; set; }
        public DateTime ClassOccurrenceStartDate { get; set; }
        public DateTime ClassOccurrenceEndDate { get; set; }
        public string ClassName { get; set; }
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
