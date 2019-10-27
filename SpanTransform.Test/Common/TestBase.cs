using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace SpanTransform.Test.Common
{
    public class TestBase
    {
        private IPEndPoint _localTestEndPoint;
        private IPEndPoint _remoteTestEndPoint;
        private Socket _commonSocket;

        protected IPEndPoint LocalTestEndpoint { get { return this._localTestEndPoint; } }
        protected IPEndPoint RemoteTestEndpoint { get { return this._remoteTestEndPoint; } }
        protected Socket CommonSocket { get { return this._commonSocket; } }

        public TestBase()
        {
            this._localTestEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8897);
            this._remoteTestEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8898);
            this._commonSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        }

        ~TestBase()
        {
            this._commonSocket.Disconnect(true);
            this._commonSocket.Close();
            this._commonSocket.Dispose();
        }
    }
}
