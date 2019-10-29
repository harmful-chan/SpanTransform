using SpanTransform.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpanTransform.Models
{
    public class InParamModel : IParamterable
    {
        public RoleType Role { get; set; }
        public OperationType Operation { get; set; }
        public string Domain { get; set; }
        public string Address { get; set; }
        private IEnumerable<string> _others;
        public IEnumerable<string> Others
        {
            set
            {
                this._others = value;
            }
            get
            {
                return this._others;
            }
        }
    }
}
