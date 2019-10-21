using SpanTransform.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpanTransform.Provider
{
    public interface IProviderable
    {
        public ResultModel UpdateTransverterRecord(RecordModel recordModel);
    }
}
