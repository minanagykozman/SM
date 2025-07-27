using System.Text.Json.Serialization;

namespace SM.DAL.DataModel
{
    public class AidClass
    {
        public int AidID { get; set; }
        public int ClassID { get; set; }
        [JsonIgnore]
        public virtual Class Class{ get; set; } = default!;
        [JsonIgnore]
        public virtual Aid Aid { get; set; } = default!;
    }

}
