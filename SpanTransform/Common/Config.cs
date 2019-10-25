using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SpanTransform.Common
{
    public static class Config
    {
        public static string DateFormat = "yyyy-MM-dd(HH:mm:ss:fff)";
        public static string NowDateFormatted { get {return DateTime.Now.ToString(DateFormat); } } 
        public static string DefaultTransverterFilePath = "transform.xml";
        public static string DefaultTransverterAddress = "47.94.162.230";
        public static int DefaultTransverterPort = 8898;
        public static IPEndPoint DefaultTransverterEndPoint = new IPEndPoint(IPAddress.Parse(DefaultTransverterAddress), DefaultTransverterPort);
    }
}
