using System;
using System.Collections.Generic;
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
        MemberNotRegistered
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
}
