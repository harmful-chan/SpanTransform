using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SpanTransform.Test
{
    [TestClass]
    
    public class ProgramTest
    {
        [TestMethod]
        //[DataRow("--role user --operation get --domain www.span.com")]
        //[DataRow("--role user --operation get --address 113.112.185.220")]
        //[DataRow("--role transverter --operation work")]
        //[DataRow("--role transverter --operation unwork")]
        //[DataRow("--role provider --operation update --domain www.span.com --address 113.112.185.220")]
        //[DataRow("--role provider --operation update --domain www.span.com --address 113.112.185.220 --wait")]
        //[DataRow("--role provider --operation update --domain www.span.com --address")]
        [DataRow("--role transverter --operation unwork")]
        //[DataRow("--role transverter --operation work --address 127.0.0.1")]
        public void TestProgram(string arg)
        {
            Program.Main(arg.Split(" "));
        }
    }
}
