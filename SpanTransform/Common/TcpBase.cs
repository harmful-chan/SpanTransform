using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SpanTransform.Common
{
    public abstract class TcpBase
    {
        protected string TransverterAddress = "47.94.162.230";
        protected int TransverterPort = 8898;
        protected IPEndPoint TransverterEndPoint = new IPEndPoint(IPAddress.Parse("47.94.162.230"), 8898);
    }
}
