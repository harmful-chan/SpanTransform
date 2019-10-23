using System;
using System.Collections.Generic;
using System.Text;

namespace SpanTransform.Common
{
    public enum StatusType
    {
        Success,
        Fail
    }

    public enum OperationType
    {
        Update,
        Start,
        Reboot,
        Stop,
        Get
    }
    public enum RoleType
    {
        Provider,
        Transverter,
        User
    }

}
