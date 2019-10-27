using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpanTransform.Clients;
using SpanTransform.Common;
using SpanTransform.Models;
using SpanTransform.Test.Common;
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

        [TestMethod]
        [DataRow("--role provider --domain www.span.com --address 113.112.185.220 --operation update")]
        public void TestUpdateTransverterRecord(string args)
        {
            TestServer server = new TestServer(base.RemoteTestEndpoint);
            Config config = new Config();
            CmdSerializer<InParamModel> cmdHelper = new CmdSerializer<InParamModel>(args, config.Directives, config.Paramters, config.Others);
            InParamModel inParam = cmdHelper.ToModel();
            server.Work();
            while (!server.IsWork) ;
            OutParamModel outParam = this._client.Order(inParam);
            Assert.IsTrue(outParam.Raw.IndexOf(inParam.Domain) > 0 && outParam.Raw.IndexOf(inParam.Address) > 0 && outParam.Raw.IndexOf("0000-00-00(00:00:00:00)") > 0);
        }
    }
}
