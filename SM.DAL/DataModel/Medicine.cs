using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.DAL.DataModel
{
    public class Medicine
    {
        public int MedicineID { get; set; }
        public string MedicineName { get; set; }
        public string Code { get;set; }
        public ICollection<MedicalAppointmentMedicine> MedicalAppointmentMedicines { get; set; }
    }
}
