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
        Update = 21,
        Work = 21,
        UnWork = 22,
        Get = 11
    }
    public enum RoleType
    {
        User = 1,
        Provider = 2,
        Transverter =3  
    }

}
