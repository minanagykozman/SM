namespace SM.DAL.DataModel
{
    public class ChurchMember
    {
        public int ChurchID { get; set; }
        public int MemberID { get; set; }
        public Member Member { get; set; }
        public Church Church { get; set; }
    }
}
