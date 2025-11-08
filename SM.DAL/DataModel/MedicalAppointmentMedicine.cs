using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SM.DAL.DataModel
{
    public class MedicalAppointmentMedicine
    {
        public int MedicalAppointmentID { get; set; }
        [JsonIgnore]
        public MedicalAppointment MedicalAppointment { get; set; }

        public int MedicineID { get; set; }
        [JsonIgnore]
        public Medicine Medicine { get; set; }
    }

}
