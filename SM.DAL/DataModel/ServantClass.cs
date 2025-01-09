using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.DAL.DataModel
{
    public class ServantClass
    {
        public int ClassID { get; set; }
        public Class Class { get; set; } = null!;
        public int ServantID { get; set; }
        public Servant Servant { get; set; } = null!;
    }

}
