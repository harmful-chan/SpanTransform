using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using SpanTransform.Transverter;
using SpanTransform.Models;
using System.Xml;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using SpanTransform.Common;
using Kooboo.Json;
using SpanTransform.Test.Common;

namespace SpanTransform.Test
{
    [TestClass]
    public class TransverterTest : TestBase
    {
        private ITransverterable _transverter;
        private XmlDocument _xmlDocument;
        private string _filePath;

        private string _date;
        public TransverterTest():base()
        {
            this._filePath = "test.xml";

            this._transverter = new XmlTransverter(base.RemoteTestEndpoint, this._filePath);
            this._xmlDocument = new XmlDocument();
            this._date = Config.NowDateFormatted;
        }

        [TestMethod] 
        [DataRow("test.span.com", "0.0.0.0", "0000-00-00(00:00:00)")]
        public void TestAddLocalRecord(string domain, string address, string date)
        {
            //添加记录
            this._date = Config.NowDateFormatted;
            this._transverter.AddLocalRecord(domain, address, this._date);
            this._date = Config.NowDateFormatted;
            this._transverter.AddLocalRecord(domain, address, this._date);
            this._date = Config.NowDateFormatted;
            this._transverter.AddLocalRecord(domain, address, this._date);

            this._xmlDocument.Load(this._filePath);

            //第一次记录
            XmlNode xmlNode = this._xmlDocument.SelectSingleNode($"/span/mainframe/record[@date='{_date}']");
            Assert.IsNotNull(xmlNode);
        }
        
        [TestMethod]
        [DataRow("0.0.0.0")]
        public void TestGetRecordsFromAddress(string address)
        {
            IEnumerable<RecordModel> result = this._transverter.GetRecordsFromAddress(address);
            Assert.IsTrue(result.Any(r => r.Address.Equals(address)));
        }

        [TestMethod]
        [DataRow("test.span.com")]
        public void TestGetRecordsFromDomain(string domain)
        {
            IEnumerable<RecordModel> result = this._transverter.GetRecordsFromDomain(domain);
            Assert.IsTrue(result.Any(r => r.Domain.Equals(domain)));
        }

        [TestMethod]
        [DataRow("test.span.com", "0.0.0.0")]
        public void TestWork(string domain, string address)
        {
            //启动线程
            Thread thread = new Thread(this._transverter.Work);
            thread.Start();

            //等待剑灵
            while (!this._transverter.IsWork) ;

            TestClient testClient = new TestClient(base.RemoteTestEndpoint);
            ResponseModel response = testClient.Work();
            this._transverter.Dispose();
            Assert.IsTrue(response.Records.Count() == 1 && 
                response.Records.First().Domain.Equals("test.span.com") &&
                response.Records.First().Address.Equals("0.0.0.0"));
        }

        [TestMethod]
        public void TestUnWork()
        {
            try
            {
                this._transverter.UnWork();
            }
            catch(Exception ex)
            {
                ;
            }
            
        }

    }
}
