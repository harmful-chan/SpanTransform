using SpanTransform.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpanTransform.Clients
{
    public interface IClientable
    {
        public OutParamModel Order(InParamModel inParam);
    }
}
