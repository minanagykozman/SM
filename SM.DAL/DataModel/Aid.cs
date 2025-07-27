using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SM.DAL.DataModel
{
    public class Aid
    {
        public int AidID { get; set; }
        [MaxLength(50)]
        public string AidName { get; set; } = string.Empty;
        public string Components { get; set; } = string.Empty;
        public decimal CostPerPerson { get; set; }
        public decimal TotalCost { get; set; }
        public int PlannedMembersCount { get; set; }
        public int ActualMembersCount { get; set; }
        public DateTime AidDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public List<int> ClassesIDs
        {
            get
            {
                if (AidClasses == null)
                    return new List<int>();
                else
                {
                    return AidClasses.Select(c => c.ClassID).ToList();
                }
            }
        }
        [JsonIgnore]
        public ICollection<MemberAid> MemberAids { get; set; } = new List<MemberAid>();
        [JsonIgnore]
        public ICollection<AidClass> AidClasses { get; set; } = new List<AidClass>();
    }

}
