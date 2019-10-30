using SpanTransform.Models;
using SpanTransform.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace SpanTransform.Common
{


    /// <summary>
    /// 业务配置信息
    /// </summary>
    public static class Config
    {

        public static CmdSerializer<InParamModel> AddMatchedGroups(this CmdSerializer<InParamModel> cmdSerializer)
        {

            Type inParamType = typeof(InParamModel);
            cmdSerializer.AddMatchedGroup("--role", inParamType.GetProperty("Role"), "transverter", RoleType.Transverter);
            cmdSerializer.AddMatchedGroup("--role", inParamType.GetProperty("Role"), "provider", RoleType.Provider);
            cmdSerializer.AddMatchedGroup("--role", inParamType.GetProperty("Role"), "user", RoleType.User);
            cmdSerializer.AddMatchedGroup("--operation", inParamType.GetProperty("Operation"), "work", OperationType.Work);
            cmdSerializer.AddMatchedGroup("--operation", inParamType.GetProperty("Operation"), "unwork", OperationType.UnWork);
            cmdSerializer.AddMatchedGroup("--operation", inParamType.GetProperty("Operation"), "update", OperationType.Update);
            cmdSerializer.AddMatchedGroup("--operation", inParamType.GetProperty("Operation"), "get", OperationType.Get);
            cmdSerializer.AddMatchedGroup("--address", inParamType.GetProperty("Address"), "*", "*");
            cmdSerializer.AddMatchedGroup("--domain", inParamType.GetProperty("Domain"), "*", "*");
            cmdSerializer.AddMatchedGroup("--wait", inParamType.GetProperty("Others"), null, "--wait");
            return cmdSerializer;
        }

        public static bool VerifyIsTrueAny(params bool[] flags)
        {
            return flags.Any(f => f);
        }


        public static void Log(LogTypes type, string[] msgs)
        {
            Array.ForEach(msgs, m => Config.Log(type, m));
            
        }

        public static void Log(LogTypes type, string msg)
        {
            if (type != LogTypes.Default) 
            {
                Console.Write("[ ");
                switch (type)
                {
                    case LogTypes.Listening: Console.ForegroundColor = ConsoleColor.Yellow; break;
                    case LogTypes.Kill: Console.ForegroundColor = ConsoleColor.Yellow; break;
                    case LogTypes.Request: Console.ForegroundColor = ConsoleColor.Yellow; break;
                    case LogTypes.Response: Console.ForegroundColor = ConsoleColor.Yellow; break;
                    case LogTypes.Error: Console.ForegroundColor = ConsoleColor.Red; break;
                    case LogTypes.Output: Console.ForegroundColor = ConsoleColor.Green; break;
                    default: Console.ForegroundColor = ConsoleColor.Magenta; break;
                }
                Console.Write(type);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" ] ");
            }
            Console.WriteLine(msg);

        }
        public static string Get()
        { 
            Console.Write(">");
            return Console.ReadLine();
        } 
        public static string DateFormat = "yyyy-MM-dd(HH:mm:ss:fff)";
        public static string NowDateFormatted { get {return DateTime.Now.ToString(DateFormat); } } 
        public static string DefaultTransverterFilePath = "transform.xml";
        
#if DEBUG
        public static string DefaultTransverterAddress = "127.0.0.1";
        public static int DefaultTransverterPort = 8898;
        
#else
        public static string DefaultTransverterAddress = "47.94.162.230";
        public static int DefaultTransverterPort = 8898;
#endif
        public static IPEndPoint DefaultTransverterEndPoint = new IPEndPoint(IPAddress.Parse(DefaultTransverterAddress), DefaultTransverterPort);
    }
}
