using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.DAL.DataModel
{
    public class MedicalAppointment
    {
        public int MedicalAppointmentID { get; set; }
        public DateTime AppoinmentDate { get; set; }
        public int MemberID { get; set; }
        public int ServantID { get; set; }
        public int? PharmacistID { get; set; }
        public string? Notes {  get; set; }
        public string? Diagnosis { get; set; }
        public string? Doctor { get; set; }
        public bool IsAttended { get; set; }
        public ICollection<MedicalAppointmentMedicine> MedicalAppointmentMedicines { get; set; }
        public virtual Member Member { get; set; }
        public virtual Servant Servant { get; set; }
        public virtual Servant? Pharmacist { get; set; }
    }
}
