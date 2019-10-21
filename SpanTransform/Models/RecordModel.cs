using System;
using System.Collections.Generic;
using System.Text;

namespace SpanTransform.Models
{
    public class RecordModel
    {
        public string Domain { get; set; }
        public string Address { get; set; }
        public DateTime UpdateDateTime { get; set; }

        private string _date;

        public string Date
        {
            get { return this.UpdateDateTime?.ToString("yyyy-MM-dd(hh:mm:ss:ff)"); }
        }

    }
}
