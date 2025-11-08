using Microsoft.EntityFrameworkCore;
using SM.DAL.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.BAL
{
    public class MedicalAppoinmentHandler : HandlerBase
    {
        public int CreateVisit(int memberID, string username, string notes)
        {
            Servant servant = GetServantByUsername(username);
            Member member = _dbcontext.Members.Where(m => m.MemberID == memberID && !m.IsDeleted).FirstOrDefault();
            if (member == null)
            {
                throw new Exception("Member not found");
            }
            MedicalAppointment medicalVisit = new MedicalAppointment();
            medicalVisit.MemberID = memberID;
            medicalVisit.AppoinmentDate = CurrentTime;
            medicalVisit.Notes = notes;
            medicalVisit.ServantID = servant.ServantID;
            medicalVisit.IsAttended = false;

            _dbcontext.MedicalAppoinments.Add(medicalVisit);
            _dbcontext.SaveChanges();
            return medicalVisit.MedicalAppointmentID;
        }
        public bool CreateVisit(List<int> memberIDs, string username, string notes)
        {
            Servant servant = GetServantByUsername(username);
            foreach (int memberID in memberIDs)
            {
                Member member = _dbcontext.Members.Where(m => m.MemberID == memberID && !m.IsDeleted).FirstOrDefault();
                if (member == null)
                {
                    throw new Exception("Member not found");
                }
                MedicalAppointment medicalVisit = new MedicalAppointment();
                medicalVisit.MemberID = memberID;
                medicalVisit.AppoinmentDate = CurrentTime;
                medicalVisit.Notes = notes;
                medicalVisit.ServantID = servant.ServantID;
                medicalVisit.IsAttended = false;

                _dbcontext.MedicalAppoinments.Add(medicalVisit);
            }
            _dbcontext.SaveChanges();
            return true;
        }
        public bool DeleteAppointment(int appointmentID)
        {
            var appoint = _dbcontext.MedicalAppoinments.Where(q => q.MedicalAppointmentID == appointmentID).FirstOrDefault();
            if (appoint == null)
            {
                throw new Exception("Appointment not found");
            }
            _dbcontext.MedicalAppoinments.Remove(appoint);
            _dbcontext.SaveChanges();
            return true;
        }
        public List<MedicalAppointment> GetMedicalAppoinments(DateTime? date)
        {
            if (date == null)
            {
                return _dbcontext.MedicalAppoinments.Include(a => a.Member).OrderBy(a => a.AppoinmentDate).ToList();
            }
            return _dbcontext.MedicalAppoinments.Where(a => a.AppoinmentDate.Date == date.Value.Date).Include(a => a.Member).OrderBy(a => a.AppoinmentDate).ToList();
        }
        public bool UpdateAppoinmentStatus(int appointmentID, bool attended)
        {
            var appoint = _dbcontext.MedicalAppoinments.Where(q => q.MedicalAppointmentID == appointmentID).FirstOrDefault();
            if (appoint == null)
            {
                throw new Exception("Appointment not found");
            }
            appoint.IsAttended = attended;
            _dbcontext.SaveChanges();
            return true;
        }
        public List<Medicine> GettAllMedicines()
        {
            return _dbcontext.Medicines.ToList();
        }
        public MedicalAppointment GetAppointment(int appoinemtentID)
        {
            var appoinemtent=_dbcontext.MedicalAppoinments.Where(m=>m.MedicalAppointmentID==appoinemtentID).Include(m=>m.MedicalAppointmentMedicines).FirstOrDefault();
            return appoinemtent;
        }
        public bool AddPrescription(int appointmentID, string diagnosis, List<int> medicines)
        {
            var appoint = _dbcontext.MedicalAppoinments.Where(q => q.MedicalAppointmentID == appointmentID).FirstOrDefault();
            if (appoint == null)
            {
                throw new Exception("Appointment not found");
            }
            appoint.Diagnosis = diagnosis;
            var oldMedicines = _dbcontext.MedicalAppointmentMedicines.Where(m => m.MedicalAppointmentID == appointmentID);
            _dbcontext.MedicalAppointmentMedicines.RemoveRange(oldMedicines);
            _dbcontext.SaveChanges();
            medicines.ForEach(m =>
            {
                _dbcontext.MedicalAppointmentMedicines.Add(new MedicalAppointmentMedicine() { MedicineID = m, MedicalAppointmentID = appointmentID });
            });
            _dbcontext.SaveChanges();
            return true;
        }
    }
}
