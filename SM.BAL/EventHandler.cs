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
        public void CreateEvent(string eventName, DateTime eventStartDate, DateTime eventEndDate, List<int> classIDs)
        {
            try
            {
                Event ev = new DAL.DataModel.Event()
                {
                    EventName = eventName,
                    EventStartDate = eventStartDate,
                    EventEndDate = eventEndDate
                };
                _dbcontext.Events.Add(ev);
                _dbcontext.SaveChanges();

                foreach (int classID in classIDs)
                {
                    _dbcontext.ClassEvents.Add(new ClassEvent() { ClassID = classID, EventID = ev.EventID });
                }
                _dbcontext.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.log.Error("", ex);
            }
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
        public RegistrationStatus TakeEventAttendance(int eventID, string memberCode, int servantID)
        {
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
            eventRegistration.Attended = true;
            eventRegistration.AttendanceServantID = servantID;
            eventRegistration.AttendanceTimeStamp = CurrentTime;
            _dbcontext.SaveChanges();

            return RegistrationStatus.Ok;
        }
        public List<Event> GetEvents(int servantID)
        {
            List<int> classes = _dbcontext.ServantClasses.Where(sc => sc.ServantID == servantID).Select(sc => sc.ClassID).ToList<int>();
            List<int> events = _dbcontext.ClassEvents.Where(ce => classes.Contains(ce.ClassID)).Select(ce => ce.EventID).ToList<int>();
            return _dbcontext.Events.Where(e => events.Contains(e.EventID)).ToList<Event>();
        }
        public List<Event> GetEvents(string username)
        {
            var servant = GetServantByUsername(username);
            if (servant == null)
                throw new Exception("Servant not found");
            List<int> classes = _dbcontext.ServantClasses.Where(sc => sc.ServantID == servant.ServantID).Select(sc => sc.ClassID).ToList<int>();
            List<int> events = _dbcontext.ClassEvents.Where(ce => classes.Contains(ce.ClassID)).Select(ce => ce.EventID).ToList<int>();
            return _dbcontext.Events.Where(e => events.Contains(e.EventID)).ToList<Event>();
        }
        public List<Member> GetEventRegisteredMembers(int eventID)
        {
            List<Member> members = _dbcontext.EventRegistrations.Where(er => er.EventID == eventID).OrderByDescending(ev => ev.TimeStamp).Select(er => er.Member).ToList<Member>();
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

        public void Dispose()
        {
            if (_dbcontext != null)
                _dbcontext.Dispose();
        }
    }
}
