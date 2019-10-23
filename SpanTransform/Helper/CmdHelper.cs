using SpanTransform.Common;
using SpanTransform.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpanTransform.Helper
{
    public class CmdHelper : IHelperable<InParamModel>
    {
        private string _args;

        public CmdHelper(string args)
        {
            this._args = args;
        }
        public InParamModel ToModel()
        {
            try
            {
                string[] args = this._args.Split(' ');
                List<string> paraments = new List<string>(args);
                //--role provider --domian www.span.com --adddress 113.112.185.220 --operation update 
                InParamModel param = new InParamModel();
                //role
                string tmp = paraments[paraments.IndexOf("--role") + 1];
                if (tmp.Equals("transverter")) param.Role = RoleType.Transverter;
                else if (tmp.Equals("user")) param.Role = RoleType.User;
                else if(tmp.Equals("provider")) param.Role = RoleType.Provider;
                //operation
                tmp = paraments[paraments.IndexOf("--operation") + 1];
                if (tmp.Equals("get")) param.Operation = OperationType.Get;
                else if (tmp.Equals("update")) param.Operation = OperationType.Update;
                else if (tmp.Equals("start")) param.Operation = OperationType.Start;
                else if (tmp.Equals("stop")) param.Operation = OperationType.Stop;
                else if (tmp.Equals("reboot")) param.Operation = OperationType.Reboot;

                param.Domain = paraments[paraments.IndexOf("--domian") + 1];
                param.Address = paraments[paraments.IndexOf("--adddress") + 1];
                

                return param;
            }
            catch
            {
                return null;
            }
        }

    }
}
