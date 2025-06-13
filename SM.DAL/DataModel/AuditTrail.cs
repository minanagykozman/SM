using System.Text.Json.Serialization;

namespace SM.DAL.DataModel
{
    public class AuditTrail
    {
        public int AuditTrailID { get; set; }
        public string ServantName { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public string EntityID { get; set; }
        public string AiditTrail { get; set; }
        public DateTime Timestamp { get; set; }
    }

}
