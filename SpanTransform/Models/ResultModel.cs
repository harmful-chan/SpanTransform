﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SpanTransform.Models
{
    public class ResultModel
    {
        public string Raw { get; set; }
        public bool IsSuccess { get { return this.Raw.IndexOf("succeed") == 0; } }
    }
}
