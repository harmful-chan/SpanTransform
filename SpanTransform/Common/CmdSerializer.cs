using SpanTransform.Common;
using SpanTransform.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace SpanTransform.Common
{
    public class CmdMatchedGroup
    {
        public string Directive { get; set; }
        public PropertyInfo PropertyInfo { get; set; }
        public string Paramter { get; set; }
        public object Value { get; set; }
    }

    /// <summary>
    /// 命令行输入序列化
    /// </summary>
    /// <typeparam name="T">绑定的模型类型</typeparam>
    public class CmdSerializer<T> where T : class, new()
    {
        private string[] _args;
        public string[] Args { get { return this._args; } set { this._args = value; } }
        private Dictionary<string, PropertyInfo> _directives;
        private Dictionary<string, object> _paramters;
        private List<string> _stringParamters;

        //private List<KeyValuePair<string, KeyValuePair<PropertyInfo, KeyValuePair<string, object>>>> _cmdMatchedGroup;
        private List<CmdMatchedGroup> _cmdMatchedGroups;

        
        public CmdSerializer(string arg, 
            Dictionary<string, PropertyInfo> directives = null, 
            Dictionary<string, object> paramters = null, 
            List<string> stringParamters = null)
        {
            this._args = arg.Split(' ');
            this._directives = directives ?? new Dictionary<string, PropertyInfo>();
            this._paramters = paramters ?? new Dictionary<string, object>();
            this._stringParamters = stringParamters ?? new List<string>();
        }

        public CmdSerializer(string[] args = null, 
            Dictionary<string, PropertyInfo> directives = null, 
            Dictionary<string, object> paramters = null, 
            List<string> stringParamters = null)
        {
            this._args = args;
            this._directives = directives ?? new Dictionary<string, PropertyInfo>();
            this._paramters = paramters ?? new Dictionary<string, object>();
            this._stringParamters = stringParamters ?? new List<string>();
        }

        public CmdSerializer(string[] args = null, List<CmdMatchedGroup> cmdMatchedGroups = null)
        {
            this._args = args;
            this._cmdMatchedGroups = cmdMatchedGroups ?? new List<CmdMatchedGroup>();
        }

        public void Add(string directive, PropertyInfo propertyInfo, string paramter, object value)
        {
            this._cmdMatchedGroups.Add(new CmdMatchedGroup() { 
                Directive = directive,
                PropertyInfo = propertyInfo, 
                Paramter = paramter, 
                Value = value });
        }

        public T ToModel()
        {
            T output = new T();
            try
            {
                //--role provider --doamin www.span.com --address 113.112.185.220 --operation update --wait
                //for (int i = 0; i < this._args.Length; i++)
                //{
                //    //获取出入指令字符串
                //    string k = this._directives.Keys.Single(k => k.Equals(this._args[i].Replace(" ", "")));
                //    if (!string.IsNullOrEmpty(k))
                //    {
                //        string param = this._args[++i]; 
                //        if (!this._directives.Keys.Any(k => k.Equals(param)))    //命令不存在
                //        {
                //            if (this._paramters.Keys.Any(k => k.Equals(param)))    //参数中存在
                //            {
                //                this._directives[k].SetValue(output, this._paramters[param]);   //往output填值
                //            }
                //            else if(this._stringParamters.Any(o => o.Equals(k)))   //other中存在
                //            {
                //                this._directives[k].SetValue(output, param);
                //            }
                //        }
                //    }
                //}

                //参数原样输入的指令
                IEnumerable<CmdMatchedGroup> rawCmdMatchedGroups = this._cmdMatchedGroups.Where(c => c.Paramter.Equals("*"));
                //参数为null的指令
                IEnumerable<CmdMatchedGroup> nullCmdMatchedGroups = this._cmdMatchedGroups.Where(c => c.Paramter == null);
                //固定参数的指令
                CmdMatchedGroup[] tmp = new CmdMatchedGroup[this._args.Length];
                this._cmdMatchedGroups.CopyTo(tmp);
                List<CmdMatchedGroup> fixedCmdMatchedGroups = new List<CmdMatchedGroup>(tmp);
                foreach (var item in rawCmdMatchedGroups) fixedCmdMatchedGroups.Remove(item);
                foreach (var item in nullCmdMatchedGroups)fixedCmdMatchedGroups.Remove(item);

                for (int i = 0; i < this._args.Length; i++)
                {
                    string str = this._args[i];
                    //原样输入命令
                    CmdMatchedGroup raw = rawCmdMatchedGroups.Single(c => c.Directive.Equals(str));
                    if (raw != null)   
                    {
                        raw.PropertyInfo.SetValue(output, this._args[++i]);
                    }
                    //空参数命令
                    CmdMatchedGroup nul = nullCmdMatchedGroups.Single(c => c.Directive.Equals(str));
                    if (nul != null)    
                    {
                        object o = nul.PropertyInfo.GetValue(output);
                        if(o == null)
                        {
                            nul.PropertyInfo.SetValue(output, new List<string>());
                        }
                        if (o is List<string> ls)
                        {
                            ls.Add(nul.Value as string ?? "");
                        }
                    }
                    //固定参数命令
                    IEnumerable<CmdMatchedGroup> fixeds = fixedCmdMatchedGroups.Where(c => c.Directive.Equals(str));
                    if(fixeds != null)    
                    {
                        fixe.PropertyInfo.SetValue(output, fixe.Value)
                    }
                }



                return output;
            }
            catch(Exception ex)
            {
                Config.Log(LogTypes.Error, "args to model.");
                return null;
            }
        }

    }
}
