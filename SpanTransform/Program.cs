using SpanTransform.Clients;
using SpanTransform.Common;
using SpanTransform.Helper;
using SpanTransform.Models;
using SpanTransform.Transverter;
using System;

namespace SpanTransform
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CmdHelper cmdHelper = new CmdHelper(args);
            InParamModel inParam = cmdHelper.ToModel();

            OutParamModel outParam = null;
            if (inParam.Role == RoleType.User || inParam.Role == RoleType.Provider)
            {
                TransverterClient client = new TransverterClient();
                outParam = client.Order(inParam);
            }
            else if (inParam.Role == RoleType.Transverter)
            {
                XmlTransverter xmlTransverter = new XmlTransverter();
                if (inParam.Operation == OperationType.Work)
                {
                    
                    xmlTransverter.Work();
                }
                else if (inParam.Operation == OperationType.UnWoek)
                {
                    xmlTransverter.UnWork()
                }
               
            }
        }
    }
}
