using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SpanTransform.Models
{
    public class CmdMatchedGroupModel
    {
        public string Directive { get; set; }
        public PropertyInfo PropertyInfo { get; set; }
        public string Paramter { get; set; }
        public object Value { get; set; }
    }
}
