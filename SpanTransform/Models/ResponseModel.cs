using SpanTransform.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpanTransform.Models
{


    public class ResponseModel
    {
        
        public StatusType Status { get; set; }
        public IEnumerable<RecordModel> Records { get; set; }
    }
}
