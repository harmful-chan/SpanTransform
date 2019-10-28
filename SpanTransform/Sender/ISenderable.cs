using SpanTransform.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpanTransform.Sender
{
    public interface ISenderable
    {
        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="inParam">传入参数</param>
        /// <returns>传出参数</returns>
        public OutParamModel Order(InParamModel inParam);
    }
}
