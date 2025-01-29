using SM.DAL;
using SM.DAL.DataModel;

namespace SM.BAL
{
    public class EventHandler : IDisposable
    {
        private AppDbContext _dbcontext;
        public EventHandler()
        {
            _dbcontext = new AppDbContext();
        }
        public enum RegistrationStatus
        {
            MemeberNotFound,
            EventNotFound,
            MemberNotEligible,
            MemberAlreadyRegistered,
            ReadyToRegister,
            Ok,
            Error
        }
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
            Event ev = (Event)_dbcontext.Events.Where(e => e.EventID == eventID).FirstOrDefault();
            Member lmember = (Member)_dbcontext.Members.FirstOrDefault(m => (m.Code == memberCode || (m.UNFileNumber == memberCode && m.IsMainMember)));

            member = lmember;
            if (ev == null)
                return RegistrationStatus.EventNotFound;
            if (lmember == null)
                return RegistrationStatus.MemeberNotFound;
            List<int> eventClasses = _dbcontext.ClassEvents.Where(e => e.EventID == ev.EventID).Select(e => e.ClassID).ToList<int>();
            List<int> memberClasses = _dbcontext.ClassMembers.Where(m => m.MemberID == lmember.MemberID).Select(e => e.ClassID).ToList<int>();
            if (eventClasses == null || memberClasses == null)
                return RegistrationStatus.MemberNotEligible;
            if (!eventClasses.Intersect(memberClasses).Any())
                return RegistrationStatus.MemberNotEligible;

            var registered = _dbcontext.EventRegistrations.Where(e => e.EventID == eventID).Select(e => e.MemberID).ToList<int>();
            if (registered != null && registered.Contains(lmember.MemberID))
                return RegistrationStatus.MemberAlreadyRegistered;

            return RegistrationStatus.ReadyToRegister;
        }

        public List<Event> GetEvents(int servantID)
        {
            List<int> classes = _dbcontext.ServantClasses.Where(sc => sc.ServantID == servantID).Select(sc => sc.ClassID).ToList<int>();
            List<int> events = _dbcontext.ClassEvents.Where(ce => classes.Contains(ce.ClassID)).Select(ce => ce.EventID).ToList<int>();
            return _dbcontext.Events.Where(e => events.Contains(e.EventID)).ToList<Event>();
        }

        public List<Member> GetEventRegisteredMembers(int eventID)
        {
            List<Member> members = _dbcontext.EventRegistrations.Where(er => er.EventID == eventID).Select(er => er.Member).ToList<Member>();
            return members;
        }

        public RegistrationStatus Register(string memberCode, int eventID, int servantID, bool isException, string notes)
        {
            Event ev = (Event)_dbcontext.Events.Where(e => e.EventID == eventID).FirstOrDefault();
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
                ServantID = servantID,
                Notes = notes,
                TimeStamp = DateTime.Now,
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
