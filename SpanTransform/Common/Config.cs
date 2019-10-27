using SpanTransform.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;

namespace SpanTransform.Common
{
    public enum LogTypes
    {
        Input,
        Output,
        Request,
        Response,
        Error
    }

    /// <summary>
    /// 业务配置信息
    /// </summary>
    public class Config
    {
        public Dictionary<string, PropertyInfo> Directives { get; private set; }
        public Dictionary<string, object> Paramters { get; private set; }
        public List<string> Others { get; private set; }

        public Config()
        {
            this.Directives = new Dictionary<string, PropertyInfo>();
            this.Paramters = new Dictionary<string, object>();
            this.Others = new List<string>();

            Type inParamType = typeof(InParamModel);
            this.Directives.Add("--role", inParamType.GetProperty("Role"));
            this.Directives.Add("--domain", inParamType.GetProperty("Domain"));
            this.Directives.Add("--address", inParamType.GetProperty("Address"));
            this.Directives.Add("--operation", inParamType.GetProperty("Operation"));


            this.Paramters.Add("transverter", RoleType.Transverter);
            this.Paramters.Add("provider", RoleType.Provider);
            this.Paramters.Add("user", RoleType.User);
            this.Paramters.Add("work", OperationType.Work);
            this.Paramters.Add("unwork", OperationType.UnWork);
            this.Paramters.Add("update", OperationType.Update);
            this.Paramters.Add("get", OperationType.Get);

            this.Others.Add("--domain");
            this.Others.Add("--address");
        }

        public static void Log(LogTypes type, string msg) => Console.WriteLine($"[{type}]"+msg);
        public static string DateFormat = "yyyy-MM-dd(HH:mm:ss:fff)";
        public static string NowDateFormatted { get {return DateTime.Now.ToString(DateFormat); } } 
        public static string DefaultTransverterFilePath = "transform.xml";
        public static IPEndPoint DefaultTransverterEndPoint = new IPEndPoint(IPAddress.Parse(DefaultTransverterAddress), DefaultTransverterPort);
#if DEBUG
        public static string DefaultTransverterAddress = "127.0.0.1";
        public static int DefaultTransverterPort = 8898;
        
#else
        public static string DefaultTransverterAddress = "47.94.162.230";
        public static int DefaultTransverterPort = 8898;
#endif
    }
}
