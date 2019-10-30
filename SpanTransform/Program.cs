using SpanTransform.Sender;
using SpanTransform.Common;
using SpanTransform.Models;
using SpanTransform.Transverter;
using System.Linq;
using SpanTransform.Serializer;
using System;
using System.Net;
using System.Collections.Generic;

namespace SpanTransform
{
    public class Program
    {
        public static InParamModel InParam { get; private set; }
        public static OutParamModel OutParam { get; private set; }

        public static void Main(string[] args)
        {
            try
            {
                //入参格式化
                Input(args);

                //模型验证
                Verify();

                //操作
                if (InParam.Role == RoleType.User) UserOperation();
                else if (InParam.Role == RoleType.Provider) ProviderOperation();
                else if (InParam.Role == RoleType.Transverter) TransveterOperation();

                //结果输出
                Output();
            }catch(Exception e)
            {
                Config.Log(LogTypes.Error, e.Message);
            }

            
        }

        private static void Input(string[] args)
        {
            CmdSerializer<InParamModel> cmdSerializer = new CmdSerializer<InParamModel>(args);
            InParam = cmdSerializer.AddMatchedGroups().ToModel();
            
            Config.Log(LogTypes.Input, 
                $"[role:{InParam.Role}] "+
                $"[operation:{InParam.Operation}] "+
                $"[domain:{InParam.Domain}] "+
                $"[address:{InParam.Address}]");
            if (InParam == null)  throw new Exception("arg error.");
        }

        private static void Verify()
        {
            //user get
            bool isUser = (InParam.Role == RoleType.User ? InParam.Operation == OperationType.Get : false);
            //provider update 
            bool isProvider = (InParam.Role == RoleType.Provider && InParam.Operation == OperationType.Update);
            //domain=*
            bool haveDomain = !string.IsNullOrEmpty(InParam.Domain);
            //address=*
            bool haveAddress = !string.IsNullOrEmpty(InParam.Address);
            //domain=* address=*
            bool haveDomainAddress = haveDomain && haveAddress;
            //domain=* address=null
            bool haveDomainOnly = haveDomain && !haveAddress;
            //domain=null address=*
            bool haveAddressOnly = !haveDomain && haveAddress;

            //transverter work/unwork
            bool isTransverter = (InParam.Role == RoleType.Transverter ? InParam.Operation == OperationType.Work || InParam.Operation == OperationType.UnWork : false);
            bool isUnWork = InParam.Operation == OperationType.UnWork;
            bool isWork = InParam.Operation == OperationType.Work;
            //--wait
            bool isWait = InParam.Others.Any(o => o.Equals("--wait"));
            //验证
            bool flag = Config.VerifyIsTrueAny((isUser && haveDomainOnly),
                (isUser && haveAddressOnly),
                (isProvider && haveDomainAddress),
                (isProvider && haveDomainOnly && isWait),
                (isProvider && haveAddressOnly && isWait),
                (isTransverter && !haveDomain && !haveAddress && isUnWork),
                (isTransverter && haveAddressOnly && isWork),
                (isTransverter && !haveAddressOnly && isWait && isWork));
            if (!flag) throw new Exception("verify business");

            //获取字符串
            if (isProvider && haveAddressOnly && isWait) InParam.Domain = Config.Get();
            if (isProvider && haveDomainOnly && isWait) InParam.Address = Config.Get();
            if (isTransverter && !haveAddressOnly && isWait) InParam.Address = Config.Get();

            Config.Log(LogTypes.Verify,
                $"[role:{InParam.Role}] " +
                $"[operation:{InParam.Operation}] " +
                $"[domain:{InParam.Domain}] " +
                $"[address:{InParam.Address}]");

        }

        private static void UserOperation()
        {
            TransSender client = new TransSender(Config.DefaultTransverterEndPoint);
            Config.Log(LogTypes.Operation, "user working.");
            OutParam = client.Order(InParam);
            Config.Log(LogTypes.Operation, "user work finish.");
            if (OutParam == null)  throw new Exception("user client output paramter null.");
        }

        private static void ProviderOperation()
        {
            TransSender client = new TransSender(Config.DefaultTransverterEndPoint);
            Config.Log(LogTypes.Operation, "provider working.");
            OutParam = client.Order(InParam);
            Config.Log(LogTypes.Operation, "provider work finish.");
            if (OutParam == null) throw new Exception("provider client output paramter null.");
        }
        private static void TransveterOperation()
        {
            XmlTransverter xmlTransverter = new XmlTransverter();
            Config.Log(LogTypes.Operation, "transverter working.");
            if (InParam.Operation == OperationType.Work)
            {
                if (!string.IsNullOrEmpty(InParam.Address))
                    xmlTransverter = new XmlTransverter(new IPEndPoint(IPAddress.Parse(InParam.Address), Config.DefaultTransverterPort));
                else throw new Exception("address is null");
                xmlTransverter.Work();
            }
            else if (InParam.Operation == OperationType.UnWork)
            {
                xmlTransverter.UnWork();
                OutParam = new OutParamModel() { Raw = "succeed" };
            }
            Config.Log(LogTypes.Operation, "transverter finish.");
        }

        private static void Output()
        {
            if (OutParam == null) throw new Exception("output is null");
            Config.Log(LogTypes.Output, $"{OutParam.IsSuccess}");

            if (OutParam.Records != null)
            {
                foreach (RecordModel record in OutParam.Records)
                {
                    Config.Log(LogTypes.Default, $"[{record.Domain} {record.Address} {record.Date}]");
                }
            }
            
        }
    }
}
