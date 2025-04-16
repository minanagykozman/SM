using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.DAL.DataModel
{
    public enum RegistrationStatus
    {
        MemeberNotFound,
        EventNotFound,
        MemberNotEligible,
        MemberAlreadyRegistered,
        ReadyToRegister,
        Ok,
        Error,
        MemberNotRegistered,
        MemberAlreadyAttended
    }
    public enum AttendanceStatus
    {
        NotRegisteredInClass,
        MemberNotFound,
        ClassNotFound,
        AlreadyAttended,
        Ready,
        Ok
    }
    public enum AidStatus
    {
        NotEligible,
        MemberNotFound,
        AidNotFound,
        AlreadyTook,
        Eligible,
        OK
    }
    public enum CardStatus
    {
        [Display(Name = "Missing Photo")]
        MissingPhoto,
        [Display(Name = "Ready to print")]
        ReadyToPrint,
        [Display(Name = "Printed")]
        Printed,
        [Display(Name = "Delivered")]
        Delivered,
        [Display(Name = "Not Applicable")]
        NotApplicable
    }
    public enum FundCategory
    {
        Rent,
        ShantatBaraka,
        Medical,
        SchoolFees,
        Others
    }
}
