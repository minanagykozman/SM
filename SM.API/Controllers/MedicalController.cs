using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SM.BAL;
using SM.API.Services;
using SM.DAL.DataModel;
using System.Collections.Generic;

namespace SM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MedicalController(ILogger<FundController> logger) : SMControllerBase(logger)
    {

        [HttpGet("get-appointments")]
        public IActionResult GetAppointments(DateTime? date)
        {
            try
            {
                using (MedicalAppoinmentHandler handler = new MedicalAppoinmentHandler())
                {
                    var appointments = handler.GetMedicalAppoinments(date);
                    return Ok(appointments);
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [HttpGet("list-medicines")]
        public IActionResult ListMedicines()
        {
            try
            {
                using (MedicalAppoinmentHandler handler = new MedicalAppoinmentHandler())
                {
                    var medicines = handler.GettAllMedicines();
                    return Ok(medicines);
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [HttpGet("list-diagnosis")]
        public IActionResult ListDiagnosis()
        {
            try
            {
                List<string> diagnosis=new List<string>();
                diagnosis.Add("Flu");
                diagnosis.Add("Abdomen Pain");
                diagnosis.Add("Others");
                return Ok(diagnosis);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [HttpPut("update-appointment")]
        public IActionResult UpdateAppointments([FromBody] AppointmentUpdateDto update)
        {
            try
            {
                using (MedicalAppoinmentHandler handler = new MedicalAppoinmentHandler())
                {
                    var result = handler.UpdateAppoinmentStatus(update.AppointmentID, update.Attended);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [HttpPut("add-prescription")]
        public IActionResult AddPrescription([FromBody] PrescriptionDto update)
        {
            try
            {
                using (MedicalAppoinmentHandler handler = new MedicalAppoinmentHandler())
                {
                    var result = handler.AddPrescription(update.AppointmentID, update.Diagnosis, update.Medicines);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [HttpPost("create-appointment")]
        public IActionResult CreateAppointments([FromBody] List<int> memberIDs)
        {
            try
            {
                using (MedicalAppoinmentHandler handler = new MedicalAppoinmentHandler())
                {
                    var result = handler.CreateVisit(memberIDs, User.Identity.Name, null);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteAppointment(int id)
        {
            try
            {
                using (MedicalAppoinmentHandler handler = new MedicalAppoinmentHandler())
                {
                    var result = handler.DeleteAppointment(id);

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [HttpGet("get-appointment/{id}")]
        public IActionResult GetAppointment(int id)
        {
            try
            {
                using (MedicalAppoinmentHandler handler = new MedicalAppoinmentHandler())
                {
                    var result = handler.GetAppointment(id);

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        public class AppointmentUpdateDto
        {
            public int AppointmentID { get; set; }
            public bool Attended { get; set; }
        }
        public class PrescriptionDto
        {
            public int AppointmentID { get; set; }
            public string Diagnosis { get; set; }
            public List<int> Medicines { get; set; }
        }

    }
}