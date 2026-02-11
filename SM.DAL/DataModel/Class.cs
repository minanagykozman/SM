using System.Text.Json.Serialization;

namespace SM.DAL.DataModel
{
    public class Class
    {
        public int ClassID { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int MeetingID { get; set; }
        public bool IsActive { get; set; }
        [JsonIgnore]
        public virtual Meeting Meeting { get; set; } = null!;
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
    }

}
