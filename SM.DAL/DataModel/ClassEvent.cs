using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.DAL.DataModel
{
    public class ClassEvent
    {
        public int ClassID { get; set; }
        public Class Class { get; set; } = null!;
        public int EventID { get; set; }
        public Event Event { get; set; } = null!;
    }

}
