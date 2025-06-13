using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SM.DAL.DataModel
{
    public class Event
    {
        public int EventID { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public int EventRegistrationCount
        {
            get
            {
                if (EventRegistrations == null)
                    return 0;
                return EventRegistrations.Count;
            }
        }
        public List<int> ClassesIDs
        {
            get
            {
                if (ClassEvents == null)
                    return new List<int>();
                else
                {
                    return ClassEvents.Select(c => c.ClassID).ToList();
                }
            }
        }
        [JsonIgnore]
        public ICollection<ClassEvent> ClassEvents { get; set; } = new List<ClassEvent>();
        [JsonIgnore]
        public ICollection<EventRegistration> EventRegistrations { get; set; } = new List<EventRegistration>();
    }

}
