using Kooboo.Json;
using SpanTransform.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SpanTransform.Test.Common
{
    public class TestServer : TestBase
    {
        private IPEndPoint _ipEndPoint;
        private Thread _thread;

        public bool IsWork { get; private set; }

        public TestServer(IPEndPoint ipEndPoint):base()
        {
            this._ipEndPoint = ipEndPoint;
            this._thread = new Thread(Do);
            this.IsWork = false;
        }
        ~TestServer()
        {
            this._thread.Abort();
        }

        public void Work()
        {
            this._thread.Start();
        }

        private void Do()
        {
            base.CommonSocket.Bind(this._ipEndPoint);
            base.CommonSocket.Listen(10);
            this.IsWork = true;

            Socket socket = base.CommonSocket.Accept();
            byte[] buffer = new byte[128];
            int count = socket.Receive(buffer);

            string str = Encoding.Default.GetString(buffer).Substring(0, count);
            RequestModel request= JsonSerializer.ToObject<RequestModel>(str);

            ResponseModel response = new ResponseModel();
            response.Records = new List<RecordModel>()
            {
                new RecordModel(){
                    Domain = request.Domain,
                    Address = request.Address,
                    Date = "0000-00-00(00:00:00:00)"
                }
            };
            string dst = JsonSerializer.ToJson<ResponseModel>(response);
            buffer = Encoding.ASCII.GetBytes(dst);
            socket.Send(buffer);
            socket.Disconnect(true);
            socket.Close();
            socket.Dispose();

            base.CommonSocket.Disconnect(true);
            base.CommonSocket.Close();

            this.IsWork = false;
        }
    }


    
}
