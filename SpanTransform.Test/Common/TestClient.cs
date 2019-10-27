using Kooboo.Json;
using SpanTransform.Common;
using SpanTransform.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SpanTransform.Test.Common
{
    public class TestClient : TestBase
    {
        private IPEndPoint _remoteEndPoint;
        private int _receiveBufferSize;
        public TestClient(IPEndPoint remoteEndPoint) : base()
        {
            this._remoteEndPoint = remoteEndPoint;
            this._receiveBufferSize = 1024;
        }

        public ResponseModel Work()
        {
            //连接服务器
            RequestModel request = new RequestModel();
            request.Role = RoleType.Provider;
            request.Operation = OperationType.Update;
            request.Domain = "test.span.com";
            request.Address = "0.0.0.0";
            base.CommonSocket.Connect(this._remoteEndPoint);


            //发送数据
            byte[] sendBuffer = Encoding.ASCII.GetBytes(JsonSerializer.ToJson<RequestModel>(request));
            base.CommonSocket.Send(sendBuffer);

            //接受数据
            byte[] receiveBuffer = new byte[this._receiveBufferSize];
            int receiveCount = base.CommonSocket.Receive(receiveBuffer);
            ResponseModel response = null;
            if (receiveCount > 0)
            {
                string receiveStr = Encoding.Default.GetString(receiveBuffer, 0, receiveCount);
                response = JsonSerializer.ToObject<ResponseModel>(receiveStr);
            }
            return response;
        }
    }
}
