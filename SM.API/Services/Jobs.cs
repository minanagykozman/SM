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
    }
}
