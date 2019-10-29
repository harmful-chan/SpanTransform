using System;
using System.Collections.Generic;
using System.Text;

namespace SpanTransform.Common
{
    public interface IParamterable
    {
        public IEnumerable<string> Others { get; set; }
    }
}
