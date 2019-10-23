using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Kooboo.Json;
using SpanTransform.Common;
using SpanTransform.Models;
using static SpanTransform.Models.ResponseModel;

namespace SpanTransform.Provider
{
    public class TcpProvider : TcpBase, IProviderable
    {
        private IPEndPoint _ipEndPoint;
        private Socket _socket;
        private int _receiveBufferSize;

        public TcpProvider(IPEndPoint ipEndPoint = null)
        {
            this._receiveBufferSize = 1024;
            this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this._ipEndPoint = ipEndPoint ?? base.TransverterEndPoint;
        }

        ~TcpProvider()
        {
            this._socket.Close();
            this._socket.Dispose();
        }

        public OutParamModel UpdateTransverterRecord(InParamModel inParam)
        {

            RequestModel requset = new RequestModel()
            {
                Role = inParam.Role,
                Domain = inParam.Domain,
                Address = inParam.Address,
                Operation = inParam.Operation
            };

            try
            {
                //连接转化器
                this._socket.Connect(this._ipEndPoint);
                //请求体序列化
                string jsonStr = JsonSerializer.ToJson<RequestModel>(requset);
                //发送
                byte[] sendBuffer = Encoding.ASCII.GetBytes(jsonStr);
                this._socket.Send(sendBuffer);

                //接收数据
                ResponseModel response = null;
                byte[] receiveBuffer = new byte[this._receiveBufferSize];
                int receiveCount = this._socket.Receive(receiveBuffer);
                if (receiveCount > 0)
                {
                    //反序列化
                    string responceStr = Encoding.Default.GetString(receiveBuffer, 0, receiveCount);
                    response = JsonSerializer.ToObject<ResponseModel>(responceStr);
                }

                //输出
                OutParamModel outParam = new OutParamModel() { Raw = "" };
                outParam.Raw += (response.Status == StatusType.Success) ? "succeed " : "failed ";
                outParam.Raw += response.Record.Domain + " ";
                outParam.Raw += response.Record.Address + " ";
                outParam.Raw += response.Record.Date + ".";

                return outParam;
            }
            catch
            {
                return null;
            }
        }
    }
}
