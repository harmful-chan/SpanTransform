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
                param.Rule = paraments[paraments.IndexOf("--role") + 1];
                param.Domain = paraments[paraments.IndexOf("--domian") + 1];
                param.Address = paraments[paraments.IndexOf("--adddress") + 1];
                param.Operation = paraments[paraments.IndexOf("--operation") + 1];

                return param;
            }
            catch
            {
                return null;
            }
        }

    }
}
