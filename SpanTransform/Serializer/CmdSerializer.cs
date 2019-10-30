using SpanTransform.Common;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using SpanTransform.Models;

namespace SpanTransform.Serializer
{


    /// <summary>
    /// 命令行输入序列化
    /// </summary>
    /// <typeparam name="T">绑定的模型类型</typeparam>
    public class CmdSerializer<T> where T: IParamterable, new()
    {
        private string[] _args;
        public string[] Args { get { return this._args; } set { this._args = value; } }
        private IEnumerable<CmdMatchedGroupModel> _cmdMatchedGroups;
        public IEnumerable<CmdMatchedGroupModel> CmdMatchedGroups { get { return this._cmdMatchedGroups; } set { this._cmdMatchedGroups = value; } }
        public CmdSerializer(string[] args = null, List<CmdMatchedGroupModel> cmdMatchedGroups = null)
        {
            this._args = args ?? new string[64];
            this._cmdMatchedGroups = cmdMatchedGroups ?? new List<CmdMatchedGroupModel>();
        }
        public void AddMatchedGroup(string directive, PropertyInfo propertyInfo, string paramter, object value)
        {
            AddMatchedGroup(new CmdMatchedGroupModel()
            {
                Directive = directive,
                PropertyInfo = propertyInfo,
                Paramter = paramter,
                Value = value
            });
        }
        public void AddMatchedGroup(CmdMatchedGroupModel cmdMatchedGroup)
        {
            this._cmdMatchedGroups = this._cmdMatchedGroups.Append(cmdMatchedGroup).ToList();
        }
        public T ToModel()
        {
            T output = new T();
            try
            {


                //参数原样输入的指令
                IEnumerable<CmdMatchedGroupModel> rawCmdMatchedGroups =
                    this._cmdMatchedGroups.Where(c => c.Paramter != null ? c.Paramter.Equals("*") : false).ToList();

                //参数为null的指令
                IEnumerable<CmdMatchedGroupModel> nullCmdMatchedGroups =
                    this._cmdMatchedGroups.Where(c => c.Paramter == null).ToList();
                if (nullCmdMatchedGroups != null && nullCmdMatchedGroups.Count() > 0)
                    nullCmdMatchedGroups.First().PropertyInfo.SetValue(output, new List<string>());//初始化other为List<string>

                //固定参数的指令
                IEnumerable<CmdMatchedGroupModel> fixedCmdMatchedGroups =
                    this._cmdMatchedGroups.Except(rawCmdMatchedGroups)
                    .Except(nullCmdMatchedGroups).ToList();

                for (int i = 0; i < this._args.Length; i++)
                {
                    string directStr = this._args[i];
                    string paramStr = i < this._args.Length - 1 ? this._args[i + 1] : "";
                    IEnumerable<CmdMatchedGroupModel> cmgs = null;
                    //输入为*指令
                    cmgs = rawCmdMatchedGroups.Where(c => c.Directive.Equals(directStr)).ToList();    //匹配所有param为*的映射组
                    if (cmgs != null && cmgs.Count() > 0)
                    {
                        if (!this._cmdMatchedGroups.Any(c => c.Directive.Equals(paramStr)))    //param不是directive
                        {
                            cmgs.First().PropertyInfo.SetValue(output, paramStr);    //原样设置值
                            i++;
                        }
                    }
                    //输入为null指令
                    cmgs = nullCmdMatchedGroups.Where(c => c.Directive.Equals(directStr)).ToList();     //匹配所有param为不存在的映射组
                    if (cmgs != null && cmgs.Count() > 0)
                    {
                        List<string> ls = cmgs.First().PropertyInfo.GetValue(output) as List<string>;    //转换类型设置值
                        if (ls != null) ls.Add(cmgs.First().Value as string ?? "");
                    }
                    //输入为固定指令
                    cmgs = fixedCmdMatchedGroups.Where(c => c.Directive.Equals(directStr)).ToList();
                    if (cmgs != null && cmgs.Count() > 0)
                    {
                        IEnumerable<CmdMatchedGroupModel> fixe = cmgs.Where(c => c.Paramter.Equals(paramStr)).ToList();    //param存在
                        if (fixe != null && fixe.Count() > 0)
                        {
                            fixe.First().PropertyInfo.SetValue(output, fixe.First().Value);    //设置值
                            i++;
                        }
                    }
                }
                return output;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
