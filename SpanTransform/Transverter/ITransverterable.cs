using SpanTransform.Models;
using System;
using System.Collections.Generic;

namespace SpanTransform.Transverter
{
    public enum TransverterStatus
    {
        IsListening,
        IsClose
    }
    public interface ITransverterable: IDisposable
    {
        /// <summary>
        /// 监听线程运行标志位
        /// </summary>
        public bool IsWork{ get; }
        /// <summary>
        /// 从记录中获取相同域名的记录
        /// </summary>
        /// <param name="domain">域名</param>
        /// <returns></returns>
        public IEnumerable<RecordModel> GetRecordsFromDomain(string domain);
        /// <summary>
        /// 获取记录中ip地址相同的记录
        /// </summary>
        /// <param name="address">IP地址</param>
        /// <returns></returns>
        public IEnumerable<RecordModel> GetRecordsFromAddress(string address);
        /// <summary>
        /// 添加记录，重复则更新时间
        /// </summary>
        /// <param name="recordModel">记录模型</param>
        public void AddLocalRecord(RecordModel recordModel);
        /// <summary>
        /// 添加记录，重复则更新时间
        /// </summary>
        /// <param name="domain">域名</param>
        /// <param name="address">IP地址</param>
        /// <param name="date">更新时间</param>
        public void AddLocalRecord(string domain, string address, string date);
        /// <summary>
        /// 启动监听线程
        /// </summary>
        public void Work();
        /// <summary>
        /// 停止监听线程
        /// </summary>
        public void UnWork();
    }
}
