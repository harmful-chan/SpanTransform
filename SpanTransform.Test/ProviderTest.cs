
using Kooboo.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpanTransform.Helper;
using SpanTransform.Models;
using SpanTransform.Provider;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SpanTransform.Test
{
    [TestClass]
    public class ProviderTest
    {
        
        private IProviderable _provider;

        private Socket _socket;
        private IPEndPoint _localEndPoint;
        private IPEndPoint _remoteEndPoint;
        public ProviderTest()
        {
            this._localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8897);
            this._remoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8898);
            this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this._socket.Bind(this._remoteEndPoint);

            this._provider = new TcpProvider(this._remoteEndPoint);
        }

        public bool RunningFlag { get; private set; }

        [TestMethod]
        [DataRow("--role provider --domian www.span.com --adddress 113.112.185.220 --operation update ")]
        public void TestUpdateTransverterRecord(string args)
        {
            Thread thread = new Thread(new ThreadStart(Listener));
            thread.Start();
            InParamModel inParamModel = new CmdHelper(args).ToModel();
            OutParamModel outParamModel = this._provider.UpdateTransverterRecord(inParamModel);

        }

        private void Listener()
        {
            
            byte[] buffer = new byte[128];
            int count = this._socket.Receive(buffer);

            string str = Encoding.Default.GetString(buffer).Substring(0, count);
            RequestModel requestModel = JsonSerializer.ToObject<RequestModel>(str);
            
            ResponseModel responseModel = new ResponseModel()
            {
                Status = "succeed",
                Record = new RecordModel()
                {
                    Domain = requestModel.Domain,
                    Address = requestModel.Address,
                    Date = "0000-00-00(00:00:00:00)"
                }
            };

            string dst = JsonSerializer.ToJson<ResponseModel>(responseModel);
            buffer = Encoding.ASCII.GetBytes(dst);
            this._socket.SendTo(buffer, this._socket.RemoteEndPoint);

            this._socket.Close();
            this._socket.Dispose();
        }
    }
}
