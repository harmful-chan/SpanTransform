using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpanTransform.Common;
using SpanTransform.Models;
using SpanTransform.Sender;
using SpanTransform.Serializer;
using SpanTransform.Test.Common;
namespace SpanTransform.Test
{
    [TestClass]
    public class SenderTest : TestBase
    {
        
        private ISenderable _client;

        public SenderTest():base()
        {
            this._client = new TransSender(base.RemoteTestEndpoint);
        }

        [TestMethod]
        [DataRow("--role provider --domain www.span.com --address 113.112.185.220 --operation update")]
        public void TestOrder(string args)
        {
            TestServer server = new TestServer(base.RemoteTestEndpoint);
            CmdSerializer<InParamModel> cmdHelper = new CmdSerializer<InParamModel>(args.Split(' '));
            InParamModel inParam = cmdHelper.ToModel();
            server.Work();
            while (!server.IsWork) ;
            OutParamModel outParam = this._client.Order(inParam);
            Assert.IsTrue(outParam.Raw.IndexOf(inParam.Domain) > 0 && outParam.Raw.IndexOf(inParam.Address) > 0 && outParam.Raw.IndexOf("0000-00-00(00:00:00:00)") > 0);
        }
    }
}
