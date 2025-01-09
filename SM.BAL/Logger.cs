using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.BAL
{
    internal static class Logger
    {
       public static readonly ILog log = LogManager.GetLogger("");
    }
}
