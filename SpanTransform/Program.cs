using SpanTransform.Clients;
using SpanTransform.Common;
using SpanTransform.Models;
using SpanTransform.Transverter;

namespace SpanTransform
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //配置参数初始化
            Config config = new Config();
            
            
            
            //入参格式化
            InParamModel inParam = new CmdSerializer<InParamModel>(args, config.Directives, config.Paramters, config.Others).ToModel();
            Config.Log(LogTypes.Input, $"role:{inParam.Role} operation:{inParam.Operation} [domain:{inParam.Domain}] [address:{inParam.Address}]");
            if (inParam == null)
            {
                Config.Log(LogTypes.Input, "arg error.");
                return;
            } 



            //模型验证
            bool flag1 = (inParam.Role == RoleType.User ? inParam.Operation == OperationType.Get : false);
            bool flag2 = (inParam.Role == RoleType.Provider ? inParam.Operation == OperationType.Update && !string.IsNullOrEmpty(inParam.Domain) && !string.IsNullOrEmpty(inParam.Address): false);
            bool flag3 = (inParam.Role == RoleType.Transverter ? inParam.Operation == OperationType.Work || inParam.Operation == OperationType.UnWork : false);
            if (!(flag1 || flag2 || flag3))
            {
                Config.Log(LogTypes.Input, "model verify error.");
                return;
            }


            OutParamModel outParam = null;
            //user privider
            if (inParam.Role == RoleType.User || inParam.Role == RoleType.Provider)    
            {
                TransverterClient client = new TransverterClient(Config.DefaultTransverterEndPoint);
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
