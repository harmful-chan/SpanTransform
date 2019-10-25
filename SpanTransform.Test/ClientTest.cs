
using Kooboo.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpanTransform.Clients;
using SpanTransform.Helper;
using SpanTransform.Models;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TransverterClient = SpanTransform.Clients.TransverterClient;

namespace SpanTransform.Test
{
    [TestClass]
    public class ClientTest : TestBase
    {
        
        private IClientable _client;

        public ClientTest():base()
        {
            this._client = new TransverterClient(base.RemoteTestEndpoint);
        }

        private bool _isOk = false;

        [TestMethod]
        [DataRow("--role provider --domian www.span.com --adddress 113.112.185.220 --operation update ")]
        public void TestUpdateTransverterRecord(string args)
        {
            Thread thread = new Thread(Start);
            thread.Start();
            InParamModel inParamModel = new CmdHelper(args).ToModel();
            while (!this._isOk) ;
            OutParamModel outParamModel = this._client.Order(inParamModel);

        }

        private void Start()
        {
            base.CommonSocket.Bind(base.RemoteTestEndpoint);
            base.CommonSocket.Listen(10);
            this._isOk = true;

            Socket socket = base.CommonSocket.Accept();
            byte[] buffer = new byte[128];
            int count = socket.Receive(buffer);

            string str = Encoding.Default.GetString(buffer).Substring(0, count);
            RequestModel requestModel = JsonSerializer.ToObject<RequestModel>(str);

            ResponseModel responseModel = new ResponseModel()
            {
                //Status = "succeed",
                Record = new RecordModel()
                {
                    Domain = requestModel.Domain,
                    Address = requestModel.Address,
                    Date = "0000-00-00(00:00:00:00)"
                }
            };

            string dst = JsonSerializer.ToJson<ResponseModel>(responseModel);
            buffer = Encoding.ASCII.GetBytes(dst);
            socket.Send(buffer);
            socket.Disconnect(true);
            socket.Close();
            socket.Dispose();

            base.CommonSocket.Disconnect(true);
            base.CommonSocket.Close();
        }
    }
}
