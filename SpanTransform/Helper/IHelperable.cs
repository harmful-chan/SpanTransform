using SpanTransform.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpanTransform.Helper
{
    public interface IHelperable<T>
    {
        public T ToModel();
    }
}
