using System;
using System.Collections.Generic;
using System.Text;

namespace SpanTransform.Common
{
    //public readonly string DateTimeFormat = "yyy-MM-dd(hh:mm:ss:ff)";
    public enum StatusType
    {
        Success = 1,
        Fail
    }

    public enum OperationType
    {
        Update = 1,
        Work,
        UnWork,
        Get
    }
    public enum RoleType
    {
        User = 1,
        Provider = 2,
        Transverter =3  
    }

    public enum LogTypes
    {
        Input = 1,
        Verify,
        Output,
        Operation,
        Request,
        Response,
        Listening,
        Kill,
        Error,
        Reading,
        Default
    }

}
