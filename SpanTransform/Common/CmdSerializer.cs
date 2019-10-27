using SpanTransform.Common;
using SpanTransform.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace SpanTransform.Common
{
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
        private List<string> _others;

        public CmdSerializer(string arg, Dictionary<string, PropertyInfo> directives = null, Dictionary<string, object> paramters = null, List<string> other = null)
        {
            this._args = arg.Split(' ');
            this._directives = directives ?? new Dictionary<string, PropertyInfo>();
            this._paramters = paramters ?? new Dictionary<string, object>();
            this._others = other ?? new List<string>();
        }

        public CmdSerializer(string[] args = null, Dictionary<string, PropertyInfo> directives = null, Dictionary<string, object> paramters = null, List<string> other = null)
        {
            this._args = args;
            this._directives = directives ?? new Dictionary<string, PropertyInfo>();
            this._paramters = paramters ?? new Dictionary<string, object>();
            this._others = other ?? new List<string>();
        }

        public T ToModel()
        {
            T output = new T();
            try
            {
                //--role provider --doamin www.span.com --address 113.112.185.220 --operation update 
                for (int i = 0; i < this._args.Length; i++)
                {
                    //获取出入指令字符串
                    string k = this._directives.Keys.Single(k => k.Equals(this._args[i].Replace(" ", "")));
                    if (!string.IsNullOrEmpty(k))
                    {
                        string param = this._args[++i]; 
                        if (!this._directives.Keys.Any(k => k.Equals(param)))    //命令不存在
                        {
                            if (this._paramters.Keys.Any(k => k.Equals(param)))    //参数中存在
                            {
                                this._directives[k].SetValue(output, this._paramters[param]);   //往output填值
                            }
                            else if(this._others.Any(o => o.Equals(k)))   //other中存在
                            {
                                this._directives[k].SetValue(output, param);
                            }
                        }
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
