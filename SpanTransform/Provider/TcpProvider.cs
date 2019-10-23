using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Kooboo.Json;
using SpanTransform.Common;
using SpanTransform.Models;

namespace SpanTransform.Provider
{
    public class TcpProvider : TcpBase, IProviderable
    {
        private IPEndPoint _ipEndPoint;
        private Socket _socket;

        public TcpProvider(IPEndPoint ipEndPoint = null)
        {
            this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            if(ipEndPoint != null)
            {
                this._ipEndPoint = ipEndPoint;
            }
            else
            {
                this._ipEndPoint = base.TransverterEndPoint;
            }
            
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
                Rule = inParam.Rule,
                Domain = inParam.Domain,
                Address = inParam.Address,
                Operation = inParam.Operation
            };

            byte[] buffer = Encoding.ASCII.GetBytes(JsonSerializer.ToJson<RequestModel>(requset));
            this._socket.SendTo(buffer, this._ipEndPoint);

            ResponseModel response = null;
            buffer = new byte[128];
            if (0 < this._socket.Receive(buffer))
            {
                
                string strResponce = Encoding.Unicode.GetString(buffer);
                response = JsonSerializer.ToObject<ResponseModel>(strResponce);
            }

            if (response.Status.Equals("succeed"))
            {
                return new OutParamModel()
                {
                    Raw = $"{response.Status} {response.Record.Domain} {response.Record.Address} {response.Record.Date}."
                };
            }
            else return null;
        }
    }
}
