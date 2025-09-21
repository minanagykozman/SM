using SM.BAL;

namespace SM.API.Services
{
    public class Jobs
    {
        public void UpdateMemberStatus()
        {
            using (MemberHandler memberHandler = new MemberHandler())
            {
                memberHandler.UpdateMemberStatus();
            }
        }
        public void UpdateEventAttendance()
        {
            using (BAL.EventHandler handler = new BAL.EventHandler())
            {
                handler.UpdateAttendance();
            }
        }
    }
}
