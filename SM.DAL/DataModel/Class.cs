using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SM.DAL.DataModel
{
    public class Class
    {
        public int ClassID { get; set; }
        [MaxLength(50)]
        public string ClassName { get; set; } = string.Empty;
        public DateTime? AgeStartDate { get; set; }
        public DateTime? AgeEndDate { get; set; }
        public char Gender { get; set; } = 'A';
        public DateTime? ClassStartDate { get; set; }
        public DateTime? ClassEndDate { get; set; }
        [MaxLength(10)]
        public string ClassDay { get; set; } = string.Empty;
        [MaxLength(5)]
        public string ClassStartTime { get; set; } = string.Empty;
        [MaxLength(5)]
        public string ClassEndTime { get; set; } = string.Empty;
        [MaxLength(10)]
        public string ClassFrequency { get; set; } = string.Empty;
        public int ChurchID { get; set; } = 1;
        public string? Notes { get; set; }
        public int Year { get; set; } = 2025;
        //public int MeetingID { get; set; }
        public bool IsActive { get; set; }
        //[JsonIgnore]
        //public virtual Meeting Meeting { get; set; } = null!;
        [JsonIgnore]
        public virtual ICollection<ClassMember> ClassMembers { get; set; } = new List<ClassMember>();
        [JsonIgnore]
        public virtual ICollection<ClassOccurrence> ClassOccurrences { get; set; } = new List<ClassOccurrence>();
        [JsonIgnore]
        public virtual ICollection<ServantClass> ServantClasses { get; set; } = new List<ServantClass>();
        [JsonIgnore]
        public virtual ICollection<ClassEvent> ClassEvents { get; set; } = new List<ClassEvent>();
        [JsonIgnore]
        public virtual ICollection<Visitation> Visitations { get; set; } = new List<Visitation>();
        public Church Church { get; set; }
    }

}
