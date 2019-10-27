using SpanTransform.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpanTransform.Models
{
    public class RequestModel
    {
        public RoleType? Role { get; set; }
        public OperationType? Operation { get; set; }
        public string? Domain { get; set; }
        public string? Address { get; set; }
    }
}
