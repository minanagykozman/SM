using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SM.DAL.DataModel
{
    public class ClassOccurrence
    {
        public int ClassOccurrenceID { get; set; }
        public int ClassID { get; set; }
        public DateTime ClassOccurrenceStartDate { get; set; }
        public DateTime ClassOccurrenceEndDate { get; set; }
        [JsonIgnore]
        public string ClassOccurrenceName
        {
            get
            {
                return string.Format("{0} {1}", ClassOccurrenceStartDate.ToString("ddd"), ClassOccurrenceStartDate.ToString("dd-MM-yy"));
            }
        }
        [JsonIgnore]
        public Class Class { get; set; } = null!;
    }
}
