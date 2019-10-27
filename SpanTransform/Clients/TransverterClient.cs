using Kooboo.Json;
using SpanTransform.Clients;
using SpanTransform.Common;
using SpanTransform.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SpanTransform.Clients
{
    public class TransverterClient: IClientable
    {
        private Socket _socket;
        private IPEndPoint _remoteEndPoint;
        private int _receiveBufferSize;

        public TransverterClient(IPEndPoint remoteEndPoint = null)  
        {
            this._receiveBufferSize = 1024;
            this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this._remoteEndPoint = remoteEndPoint ?? Config.DefaultTransverterEndPoint;
        }

        ~TransverterClient()
        {
            this._socket.Close();
            this._socket.Dispose();
        }

        public ResponseModel CommunicationWithServer(RequestModel request)
        {
            try
            {
                //连接转化器
                this._socket.Connect(this._remoteEndPoint);
                //请求体序列化
                string jsonStr = JsonSerializer.ToJson<RequestModel>(request);
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

                return response;
            }
            catch
            {
                return null;
            }
        }


        public OutParamModel Order(InParamModel inParam)
        {
            RequestModel request = new RequestModel();
            request.Role = inParam.Role;
            request.Operation = inParam.Operation;
            request.Domain = inParam.Domain;
            request.Address = inParam.Address;

            Config.Log(LogTypes.Request, "tcp client request");
            ResponseModel response = this.CommunicationWithServer(request);
            Config.Log(LogTypes.Response, "tcp client response");
            OutParamModel outParam = new OutParamModel() { Raw = "" };
            if (response != null)
            {
                outParam.Raw += (response.Status == StatusType.Success) ? "succeed " : "failed ";
                foreach (RecordModel record in response.Records)
                {
                    outParam.Raw += "[";
                    outParam.Raw += record.Domain + " ";
                    outParam.Raw += record.Address + " ";
                    outParam.Raw += record.Date;
                    outParam.Raw += "]";
                }
                

            }

            return outParam;
        }
    }
}
