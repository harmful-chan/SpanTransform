using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

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
        [DataRow("--role provider --operation update --domain www.span.com --address 113.112.185.220")]
        public void TestProgram(string arg)
        {
            Program.Main(arg.Split(" "));
        }
    }
}
