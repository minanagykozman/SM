using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.DAL.DataModel
{
    public class MemberAttendanceSummaryView
    {
        public int MemberID { get; set; }
        public int ClassID { get; set; }
        public string Code { get; set; }
        public string UNFileNumber { get; set; }
        public string UNFirstName { get; set; }
        public string UNLastName { get; set; }
        public string ClassName { get; set; }
        public DateTime MeetingStartDate { get; set; }
        public int AttendedTimes { get; set; }
        public DateTime? LastAttendanceDate { get; set; }
        public int TotalOccurrencesToDate { get; set; }
        // Optional: Helper property to calculate attendance percentage
        public double AttendancePercentage
        {
            get
            {
                if (TotalOccurrencesToDate > 0)
                {
                    return (double)AttendedTimes / TotalOccurrencesToDate * 100;
                }
                else
                {
                    return 0;
                }
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
