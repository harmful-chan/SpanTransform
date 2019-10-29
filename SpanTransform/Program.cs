using SpanTransform.Sender;
using SpanTransform.Common;
using SpanTransform.Models;
using SpanTransform.Transverter;
using System.Linq;
using SpanTransform.Serializer;

namespace SpanTransform
{
    public class Program
    {
        public static void Main(string[] args)
        {

            //入参格式化
            CmdSerializer<InParamModel> cmdSerializer = new CmdSerializer<InParamModel>(args);
            InParamModel inParam = Config.AddMatchedGroups(cmdSerializer).ToModel();
            Config.Log(LogTypes.Input, $" [role:{inParam.Role}] [operation:{inParam.Operation}] [domain:{inParam.Domain}] [address:{inParam.Address}]");
            if (inParam == null)
            {
                Config.Log(LogTypes.Input, "arg error.");
                return;
            } 



            //模型验证
            //user get
            bool isUser = (inParam.Role == RoleType.User ? inParam.Operation == OperationType.Get : false);
            //provider update 
            bool isProvider = (inParam.Role == RoleType.Provider && inParam.Operation == OperationType.Update);
            //domain=* address=*
            bool haveDomainAddress = (!string.IsNullOrEmpty(inParam.Domain) && !string.IsNullOrEmpty(inParam.Address));
            //domain=* address=null
            bool haveDomainOnly = (!string.IsNullOrEmpty(inParam.Domain) && string.IsNullOrEmpty(inParam.Address));
            //domain=null address=*
            bool haveAddressOnly = (string.IsNullOrEmpty(inParam.Domain) && !string.IsNullOrEmpty(inParam.Address));
            //transverter work/unwork
            bool isTransverter = (inParam.Role == RoleType.Transverter ? inParam.Operation == OperationType.Work || inParam.Operation == OperationType.UnWork : false);
            //user/provider haveDomainAddress/haveAddressOnly
            bool isWait = inParam.Others.Any(o => o.Equals("--wait"));
            if (!( 
                (isUser && haveDomainOnly) || 
                (isUser && haveAddressOnly)  || 
                (isProvider && haveDomainAddress) ||
                (isProvider && haveDomainOnly) ||
                (isProvider && haveAddressOnly) ||
                (isTransverter && !haveDomainAddress)))
            {
                Config.Log(LogTypes.Input, "model verify error.");
                return;
            }

            string inputStr = (isProvider && haveAddressOnly) || (isProvider && haveDomainOnly) ? Config.Get() : "";
            inParam.Domain = (isProvider && haveAddressOnly) ? inputStr : inParam.Domain;
            inParam.Address = (isProvider && haveDomainOnly) ? inputStr : inParam.Address;
            Config.Log(LogTypes.Input, $" [role:{inParam.Role}] [operation:{inParam.Operation}] [domain:{inParam.Domain}] [address:{inParam.Address}]");

            OutParamModel outParam = null;
            //user privider
            if (inParam.Role == RoleType.User || inParam.Role == RoleType.Provider)    
            {
                TransSender client = new TransSender(Config.DefaultTransverterEndPoint);
                Config.Log(LogTypes.Input, "client working.");
                outParam = client.Order(inParam);
                Config.Log(LogTypes.Output, "client work finish.");
                if(outParam == null)
                {
                    Config.Log(LogTypes.Error,"client OutPut Paramter null.");
                }
            }
            //transverter
            else if (inParam.Role == RoleType.Transverter)    
            {
                XmlTransverter xmlTransverter = new XmlTransverter(Config.DefaultTransverterEndPoint);
                Config.Log(LogTypes.Input, "transverter working.");
                if (inParam.Operation == OperationType.Work)
                {
                    xmlTransverter.Work();   
                }
                else if (inParam.Operation == OperationType.UnWork)
                {
                    xmlTransverter.UnWork();
                }
                Config.Log(LogTypes.Output, "transverter finish.");
            }

            //结果输出
            if (outParam != null)
            {
                Config.Log(LogTypes.Output, outParam.Raw);
            }
            
        }
    }
}
