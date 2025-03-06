using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
