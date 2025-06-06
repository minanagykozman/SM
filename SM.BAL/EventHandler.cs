using Microsoft.EntityFrameworkCore;
using SM.DAL;
using SM.DAL.DataModel;

namespace SM.BAL
{
    public class EventHandler : HandlerBase
    {
        /// <summary>
        /// Creates new event
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="eventStartDate"></param>
        /// <param name="eventEndDate"></param>
        /// <param name="classIDs"></param>
        public int CreateEvent(string eventName, DateTime eventStartDate, DateTime eventEndDate, List<int> classIDs)
        {
            try
            {
                Event ev = new DAL.DataModel.Event()
                {
                    EventName = eventName,
                    EventStartDate = eventStartDate,
                    EventEndDate = eventEndDate,
                    IsActive = true
                };
                _dbcontext.Events.Add(ev);

                _dbcontext.SaveChanges();

                foreach (int classID in classIDs)
                {
                    _dbcontext.ClassEvents.Add(new ClassEvent() { ClassID = classID, EventID = ev.EventID });
                }
                _dbcontext.SaveChanges();
                return ev.EventID;
            }
            catch (Exception ex)
            {
                Logger.log.Error("", ex);
                return 0;
            }
        }

        public int UpdateEvent(int eventID, string eventName, DateTime eventStartDate, DateTime eventEndDate, bool isActive, List<int> classIDs)
        {
            try
            {
                Event? ev = _dbcontext.Events.Where(e => e.EventID == eventID).FirstOrDefault();
                if (ev == null)
                    throw new Exception("Event not found");

                ev.EventName = eventName;
                ev.EventStartDate = eventStartDate;
                ev.EventEndDate = eventEndDate;
                ev.IsActive = isActive;

                var classEvents = _dbcontext.ClassEvents.Where(e => e.EventID == eventID).ToList();
                _dbcontext.ClassEvents.RemoveRange(classEvents);
                _dbcontext.SaveChanges();

                foreach (int classID in classIDs)
                {
                    _dbcontext.ClassEvents.Add(new ClassEvent() { ClassID = classID, EventID = ev.EventID });
                }
                _dbcontext.SaveChanges();
                return ev.EventID;
            }
            catch (Exception ex)
            {
                Logger.log.Error("", ex);
                return 0;
            }
        }

        public int DeleteEvent(int eventID)
        {
            try
            {
                Event? ev = _dbcontext.Events.Where(e => e.EventID == eventID).FirstOrDefault();
                if (ev == null)
                    throw new Exception("Event not found");
                ev.IsDeleted = true;

                _dbcontext.SaveChanges();
                return ev.EventID;
            }
            catch (Exception ex)
            {
                Logger.log.Error("", ex);
                return 0;
            }
        }

        public Event? GetEvent(int eventID)
        {
            var ev = _dbcontext.Events.Include(e => e.ClassEvents).Where(e => e.EventID == eventID).FirstOrDefault();
            return ev;
        }
        public RegistrationStatus CheckRegistationStatus(string memberCode, int eventID, out Member member)
        {
            memberCode = memberCode.Trim();
            Event? ev = _dbcontext.Events.Where(e => e.EventID == eventID).FirstOrDefault();
            Member? lmember = _dbcontext.Members.Include(m => m.ClassMembers).
                FirstOrDefault(m => m.Code.Contains(memberCode)
                || m.UNPersonalNumber == memberCode
                || (m.UNFileNumber == memberCode && m.IsMainMember));
            if (lmember == null)
            {
                member = null;
                return RegistrationStatus.MemeberNotFound;
            }
            else
            {
                member = lmember;
            }
            if (ev == null)
                return RegistrationStatus.EventNotFound;

            var registered = _dbcontext.EventRegistrations.Where(e => e.EventID == eventID).Select(e => e.MemberID).ToList<int>();
            if (registered != null && registered.Contains(lmember.MemberID))
                return RegistrationStatus.MemberAlreadyRegistered;
            List<int> eventClasses = _dbcontext.ClassEvents.Where(e => e.EventID == ev.EventID).Select(e => e.ClassID).ToList<int>();
            List<int> memberClasses = _dbcontext.ClassMembers.Where(m => m.MemberID == lmember.MemberID).Select(e => e.ClassID).ToList<int>();
            if (eventClasses == null || memberClasses == null)
                return RegistrationStatus.MemberNotEligible;
            if (!eventClasses.Intersect(memberClasses).Any())
                return RegistrationStatus.MemberNotEligible;
            return RegistrationStatus.ReadyToRegister;
        }
        public RegistrationStatus CheckEventAttendance(int eventID, string memberCode, out MemberEventView member)
        {
            memberCode = memberCode.Trim();
            var lmember = _dbcontext.MemberEventView.
                FirstOrDefault(m => m.Code.Contains(memberCode)
                || m.UNPersonalNumber == memberCode
                || (m.UNFileNumber == memberCode && m.IsMainMember));
            member = lmember;
            if (member == null)
            {
                return RegistrationStatus.MemeberNotFound;
            }
            var ev = _dbcontext.Events.FirstOrDefault(e => e.EventID == eventID);
            if (ev == null)
            {
                return RegistrationStatus.EventNotFound;
            }
            var eventRegistration = _dbcontext.EventRegistrations.FirstOrDefault(e => e.EventID == eventID && e.MemberID == lmember.MemberID);
            if (eventRegistration == null)
            {
                return RegistrationStatus.MemberNotRegistered;
            }
            if (eventRegistration.Attended.HasValue && eventRegistration.Attended == true)
            {
                return RegistrationStatus.MemberAlreadyAttended;
            }
            return RegistrationStatus.Ok;
        }
        public RegistrationStatus TakeEventAttendance(int eventID, string memberCode, string userName)
        {
            var servant = GetServantByUsername(userName);
            var member = _dbcontext.Members.Include(m => m.ClassMembers).
                FirstOrDefault(m => m.Code == memberCode || m.UNPersonalNumber == memberCode || (m.UNFileNumber == memberCode && m.IsMainMember));
            if (member == null)
            {
                return RegistrationStatus.MemeberNotFound;
            }
            var ev = _dbcontext.Events.FirstOrDefault(e => e.EventID == eventID);
            if (ev == null)
            {
                return RegistrationStatus.EventNotFound;
            }
            var eventRegistration = _dbcontext.EventRegistrations.FirstOrDefault(e => e.EventID == eventID && e.MemberID == member.MemberID);
            if (eventRegistration == null)
            {
                return RegistrationStatus.MemberNotRegistered;
            }
            if (eventRegistration.Attended.HasValue && eventRegistration.Attended == true)
            {
                return RegistrationStatus.MemberAlreadyAttended;
            }
            eventRegistration.Attended = true;
            eventRegistration.AttendanceServantID = servant.ServantID;
            eventRegistration.AttendanceTimeStamp = CurrentTime;
            _dbcontext.SaveChanges();

            return RegistrationStatus.Ok;
        }
        public List<Event> GetEvents(int servantID)
        {
            List<int> classes = _dbcontext.ServantClasses.Where(sc => sc.ServantID == servantID).Select(sc => sc.ClassID).ToList<int>();
            List<int> events = _dbcontext.ClassEvents.Where(ce => classes.Contains(ce.ClassID)).Select(ce => ce.EventID).ToList<int>();
            return _dbcontext.Events.Where(e => events.Contains(e.EventID) && e.IsDeleted == false && e.IsActive).ToList<Event>();
        }
        public List<Event> GetEvents(string username)
        {
            var servant = GetServantByUsername(username);
            if (servant == null)
                throw new Exception("Servant not found");
            List<int> classes = _dbcontext.ServantClasses.Where(sc => sc.ServantID == servant.ServantID).Select(sc => sc.ClassID).ToList<int>();
            List<int> events = _dbcontext.ClassEvents.Where(ce => classes.Contains(ce.ClassID)).Select(ce => ce.EventID).ToList<int>();
            return _dbcontext.Events.Where(e => events.Contains(e.EventID) && e.IsDeleted == false && e.IsActive).ToList<Event>();
        }
        public List<Member> GetEventRegisteredMembers(int eventID)
        {
            List<Member> members = _dbcontext.EventRegistrations.Where(er => er.EventID == eventID).OrderByDescending(ev => ev.TimeStamp).Select(er => er.Member).ToList<Member>();
            return members;
        }
        public List<Member> GetEventAttendedMembers(int eventID)
        {
            List<Member> members = _dbcontext.EventRegistrations.Where(er => er.EventID == eventID && er.Attended == true).OrderByDescending(ev => ev.AttendanceTimeStamp).Select(er => er.Member).ToList<Member>();
            return members;
        }
        public List<MemberEventView> GetEventMembers(int eventID, bool? registered, bool? attended)
        {
            List<MemberEventView> members = _dbcontext.MemberEventView.Where(
                er => er.EventID == eventID
                && (!registered.HasValue || er.Registered == registered.Value)
                && (!attended.HasValue || er.Attended == attended.Value))
                .OrderByDescending(ev => ev.TimeStamp).ToList();
            return members;
        }
        public RegistrationStatus Register(string memberCode, int eventID, string username, bool isException, string? notes)
        {
            var servant = GetServantByUsername(username);
            if (servant == null)
                throw new Exception("Servant not found");
            Event ev = (Event)_dbcontext.Events.Include(e => e.EventRegistrations).Where(e => e.EventID == eventID).FirstOrDefault();
            Member member = (Member)_dbcontext.Members.FirstOrDefault(m => (m.Code == memberCode || (m.UNFileNumber == memberCode && m.IsMainMember)));
            if (ev == null)
                return RegistrationStatus.EventNotFound;
            if (member == null)
                return RegistrationStatus.MemeberNotFound;
            if (ev.EventRegistrations.Any(r => r.MemberID == member.MemberID))
                return RegistrationStatus.MemberAlreadyRegistered;
            _dbcontext.EventRegistrations.Add(new EventRegistration()
            {
                EventID = eventID,
                MemberID = member.MemberID,
                ServantID = servant.ServantID,
                Notes = notes,
                TimeStamp = CurrentTime,
                IsException = isException
            });
            _dbcontext.SaveChanges();
            return RegistrationStatus.Ok;

        }
        public void DistributeMembers(int eventID, List<string> teams, List<string> busses)
        {
            List<EventRegistration> registrations = _dbcontext.EventRegistrations.Where(e => e.EventID == eventID).OrderBy(e => e.Member.Birthdate).ToList();
            int teamCount = registrations.Count / teams.Count;
            int busCount = registrations.Count / busses.Count;
            int tempTeam = teamCount;
            int tempBus = busCount;
            int teamsCounter = 0;
            int busCounter = 0;
            foreach (EventRegistration registration in registrations)
            {
                if (tempTeam == 0 && (teamsCounter + 1) < teams.Count)
                {
                    teamsCounter++;
                    tempTeam = teamCount;
                }
                if (tempBus == 0 && (busCounter + 1) < busses.Count)
                {
                    busCounter++;
                    tempBus = busCount;
                }
                registration.Team = teams[teamsCounter];
                registration.Bus = busses[busCounter];
                tempBus--;
                tempTeam--;
            }
            _dbcontext.SaveChanges();
        }
    }
}
