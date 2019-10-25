using System;
using System.Collections.Generic;
using System.Text;

namespace SpanTransform.Common
{
    //public readonly string DateTimeFormat = "yyy-MM-dd(hh:mm:ss:ff)";
    public enum StatusType
    {
        Success,
        Fail
    }

    public enum OperationType
    {
        Update,
        Work,
        UnWoek,
        Get
    }
    public enum RoleType
    {
        Provider,
        Transverter,
        User
    }

}
