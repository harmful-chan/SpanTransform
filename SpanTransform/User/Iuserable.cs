using SpanTransform.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpanTransform.User
{
    public interface IUserable
    {
        public ResultModel UpdateTransverterRecord(RecordModel recordModel);
    }
}
