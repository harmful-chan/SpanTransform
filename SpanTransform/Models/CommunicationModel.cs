﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SpanTransform.Models
{
    public class CommunicationModel
    {
        public string Rule  { get; set; }
        public string Operation { get; set; }
        public string Domain { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
    }
}
