using Microsoft.EntityFrameworkCore;
using SM.DAL;
using SM.DAL.DataModel;
using System.Globalization;
using System.Linq;

namespace SM.BAL
{
    public class MeetingHandler : HandlerBase
    {
        
        public List<Class> GetServantClasses(string username)
        {
            var servant =GetServantByUsername(username);
            return _dbcontext.Classes.Where(c => c.ServantClasses.Any(s => s.ServantID == servant.ServantID)).ToList();
        }
        public List<ClassOccurrence> GetClassOccurences(int classID)
        {
            return _dbcontext.ClassOccurrences.Where(c => c.ClassID == classID).ToList();
        }
        public List<Member> GetAttendedMembers(int occurrenceID)
        {
            return _dbcontext.ClassAttendances.Where(c => c.ClassOccurrenceID == occurrenceID).OrderByDescending(c => c.TimeStamp).Select(c => c.Member).ToList();
        }
        public Class CreateClass(string className, int meetingID)
        {
            try
            {
                Class cl = new DAL.DataModel.Class()
                {
                    ClassName = className,
                    IsActive = true,
                    MeetingID = meetingID
                };
                _dbcontext.Classes.Add(cl);
                _dbcontext.SaveChanges();
                return cl;
            }
            catch (Exception ex)
            {
                Logger.log.Error("", ex);
                return null;
            }
        }
        public string  CreateClassOccurences(int classID)
        {
            Class? cl = _dbcontext.Classes.Include(c=>c.Meeting).FirstOrDefault(c => c.ClassID == classID);
            if (cl == null)
            {
                throw new ArgumentException("Class not found");
            }
            // Parse the meetingDay into a DayOfWeek enum
            if (!Enum.TryParse(cl.Meeting.MeetingDay, true, out DayOfWeek targetDayOfWeek))
            {
                throw new ArgumentException("Invalid meeting day");
            }

            if (!cl.Meeting.MeetingStartDate.HasValue)
            {
                throw new ArgumentException("Invalid meeting start date");
            }
            if (!cl.Meeting.MeetingEndDate.HasValue)
            {
                throw new ArgumentException("Invalid meeting end date");
            }
            DateTime firstMeetingDate = cl.Meeting.MeetingStartDate.Value;
            while (firstMeetingDate.DayOfWeek != targetDayOfWeek)
            {
                firstMeetingDate = firstMeetingDate.AddDays(1);
            }

            // Convert startTime and endTime into TimeSpan
            if (!TimeSpan.TryParseExact(cl.Meeting.MeetingStartTime, "hh\\:mm", CultureInfo.InvariantCulture, out TimeSpan parsedStartTime) ||
                !TimeSpan.TryParseExact(cl.Meeting.MeetingEndTime, "hh\\:mm", CultureInfo.InvariantCulture, out TimeSpan parsedEndTime))
            {
                throw new ArgumentException("Invalid time format. Use HH:mm");
            }

            // Generate occurrences every 7 days until endDate
            DateTime meetingDate = firstMeetingDate;
            while (meetingDate <= cl.Meeting.MeetingEndDate.Value)
            {
                ClassOccurrence classOccurance = new ClassOccurrence()
                {
                    ClassOccurrenceStartDate = meetingDate.Date + parsedStartTime,
                    ClassOccurrenceEndDate = meetingDate.Date + parsedEndTime,
                    ClassID = classID
                };
                _dbcontext.ClassOccurrences.Add(classOccurance);
                meetingDate = meetingDate.AddDays(7); // Move to the next week
            }
            _dbcontext.SaveChanges();
            return "Occurences added!";
        }
        public List<ClassOccurrence> CreateClassOccurences(int classID, DateTime startDate, DateTime endDate)
        {
            Class? cl = _dbcontext.Classes.FirstOrDefault(c => c.ClassID == classID);
            if (cl == null)
            {
                throw new ArgumentException("Class not found");
            }
            Meeting? mt = _dbcontext.Meetings.FirstOrDefault(m => m.MeetingID == cl.ClassID);
            // Parse the meetingDay into a DayOfWeek enum
            if (!Enum.TryParse(mt.MeetingDay, true, out DayOfWeek targetDayOfWeek))
            {
                throw new ArgumentException("Invalid meeting day");
            }

            DateTime firstMeetingDate = startDate;
            while (firstMeetingDate.DayOfWeek != targetDayOfWeek)
            {
                firstMeetingDate = firstMeetingDate.AddDays(1);
            }

            // Convert startTime and endTime into TimeSpan
            if (!TimeSpan.TryParseExact(mt.MeetingStartTime, "hh\\:mm", CultureInfo.InvariantCulture, out TimeSpan parsedStartTime) ||
                !TimeSpan.TryParseExact(mt.MeetingEndTime, "hh\\:mm", CultureInfo.InvariantCulture, out TimeSpan parsedEndTime))
            {
                throw new ArgumentException("Invalid time format. Use HH:mm");
            }

            List<ClassOccurrence> classOccurances = new List<ClassOccurrence>();
            // Generate occurrences every 7 days until endDate
            DateTime meetingDate = firstMeetingDate;
            while (meetingDate <= endDate)
            {
                ClassOccurrence classOccurance = new ClassOccurrence()
                {
                    ClassOccurrenceStartDate = meetingDate.Date + parsedStartTime,
                    ClassOccurrenceEndDate = meetingDate.Date + parsedEndTime,
                    ClassID = classID
                };
                _dbcontext.ClassOccurrences.Add(classOccurance);
                classOccurances.Add(classOccurance);

                meetingDate = meetingDate.AddDays(7); // Move to the next week
            }
            _dbcontext.SaveChanges();
            return classOccurances;
        }
        public AttendanceStatus TakeClassAteendance(int classOccuranceID, string memberCode, string username, bool forceRegister)
        {
            var servant = GetServantByUsername(username);

            Member member = new Member();
            AttendanceStatus status = CheckAteendance(classOccuranceID, memberCode, out member);
            if (status == AttendanceStatus.MemberNotFound)
                return AttendanceStatus.MemberNotFound;
            if (status == AttendanceStatus.ClassNotFound)
                return AttendanceStatus.ClassNotFound;
            if (status == AttendanceStatus.AlreadyAttended)
                return AttendanceStatus.AlreadyAttended;
            if (status == AttendanceStatus.NotRegisteredInClass && !forceRegister)
                return AttendanceStatus.NotRegisteredInClass;

            if (status == AttendanceStatus.NotRegisteredInClass && forceRegister)
            {
                var cl = _dbcontext.ClassOccurrences.FirstOrDefault(c => c.ClassOccurrenceID == classOccuranceID);
                _dbcontext.ClassMembers.Add(new ClassMember() { ClassID = cl.ClassID, MemberID = member.MemberID });

                ClassAttendance classAttendance = new ClassAttendance()
                {
                    ClassOccurrenceID = classOccuranceID,
                    MemberID = member.MemberID,
                    ServantID = servant.ServantID,
                    TimeStamp = CurrentTime
                };
                _dbcontext.ClassAttendances.Add(classAttendance);
            }
            if (status == AttendanceStatus.Ready)
            {
                ClassAttendance classAttendance = new ClassAttendance()
                {
                    ClassOccurrenceID = classOccuranceID,
                    MemberID = member.MemberID,
                    ServantID = servant.ServantID,
                    TimeStamp = CurrentTime
                };
                _dbcontext.ClassAttendances.Add(classAttendance);
            }
            _dbcontext.SaveChanges(true);
            return AttendanceStatus.Ok;
        }

        public string TakeClassAteendance(int classOccuranceID, List<int> memberIDs, string username)
        {
            var servant = GetServantByUsername(username);
            foreach (int memberID in memberIDs)
            {
                ClassAttendance classAttendance = new ClassAttendance()
                {
                    ClassOccurrenceID = classOccuranceID,
                    MemberID = memberID,
                    ServantID = servant.ServantID,
                    TimeStamp = CurrentTime
                };
                _dbcontext.ClassAttendances.Add(classAttendance);
            }
            _dbcontext.SaveChanges(true);
            return "Attendance done!";
        }
        public AttendanceStatus CheckAteendance(int classOccuranceID, string memberCode, out Member member)
        {
            
            member = _dbcontext.Members.Include(m => m.ClassMembers).
                FirstOrDefault(m => m.Code == memberCode || m.UNPersonalNumber == memberCode || (m.UNFileNumber == memberCode && m.IsMainMember));
            if (member == null)
                return AttendanceStatus.MemberNotFound;
            if (member.ClassMembers == null || member.ClassMembers.Count() == 0)
                return AttendanceStatus.NotRegisteredInClass;
            ClassOccurrence? cl = _dbcontext.ClassOccurrences.FirstOrDefault(c => c.ClassOccurrenceID == classOccuranceID);
            if (cl == null)
                return AttendanceStatus.ClassNotFound;
            if (!member.ClassMembers.Any(c => c.ClassID == cl.ClassID))
                return AttendanceStatus.NotRegisteredInClass;
            int memberID = member.MemberID;
            if (_dbcontext.ClassAttendances.Any(c => c.ClassOccurrenceID == classOccuranceID && c.MemberID == memberID))
                return AttendanceStatus.AlreadyAttended;

            return AttendanceStatus.Ready;

        }
    }
    
}
