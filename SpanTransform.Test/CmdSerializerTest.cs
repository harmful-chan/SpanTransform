using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpanTransform.Common;
using SpanTransform.Models;
using SpanTransform.Serializer;

namespace SpanTransform.Test
{
    [TestClass]
    public class CmdSerializerTest
    {
        [TestMethod]
        [DataRow("--role provider --domain www.span.com --address 113.112.185.220 --operation update")]
        public void TestToModel(string cmd)
        {
            CmdSerializer<InParamModel> inParam = new CmdSerializer<InParamModel>(cmd.Split(' '));
            InParamModel inParamModel = inParam.ToModel();
            Assert.IsTrue(inParamModel != null);
        }
    }
}
