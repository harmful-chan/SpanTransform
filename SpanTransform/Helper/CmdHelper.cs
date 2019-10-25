using SpanTransform.Common;
using SpanTransform.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SpanTransform.Helper
{
    public class CmdHelper : IHelperable<InParamModel>
    {
        private string[] _args;

        private Dictionary<string, RoleType> _rolePairs;
        private Dictionary<string, OperationType> _operationPairs;
        private Dictionary<string, string> _otherPairs;

        public CmdHelper(string[] args, Dictionary<string, RoleType> rolePairs = null, Dictionary<string, OperationType> operationPairs = null, Dictionary<string, string> otherPairs = null)
        {
            this._args = args;
            this._rolePairs = rolePairs ?? new Dictionary<string, RoleType>();
            this._operationPairs = operationPairs ?? new Dictionary<string, OperationType>();
            this._otherPairs = otherPairs ?? new Dictionary<string, string>();

            this._rolePairs.Add("transverter", RoleType.Transverter);
            this._rolePairs.Add("provider", RoleType.Provider);
            this._rolePairs.Add("user", RoleType.User);

            this._operationPairs.Add("work", OperationType.Work);
            this._operationPairs.Add("unwork", OperationType.UnWoek);
            this._operationPairs.Add("update", OperationType.Update);
            this._operationPairs.Add("get", OperationType.Get);

            this._otherPairs.Add("domain", "");
            this._otherPairs.Add("address", "");
        }


        public InParamModel ToModel()
        {
            try
            {
                List<string> paraments = new List<string>(this._args);
                //--role provider --domian www.span.com --adddress 113.112.185.220 --operation update 
                InParamModel param = new InParamModel();

                foreach (PropertyInfo propertyInfo in typeof(InParamModel).GetProperties())
                {

                } 
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
