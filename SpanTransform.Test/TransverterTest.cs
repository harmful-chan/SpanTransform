using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using SpanTransform.Transverter;
using SpanTransform.Models;
using System.Xml;
using System.Linq;

namespace SpanTransform.Test
{
    [TestClass]
    public class TransverterTest
    {
        private ITransverterable _transverter;
        public TransverterTest()
        {
            this._transverter = new TcpTransverter();
        }


        [TestMethod]
        [DataRow("test.span.com", "112.113.152.25", "2019-10-22(00:00:00)")]
        public void TestAddLocalRecord(string domain, string address, string date)
        {
            date = DateTime.Now.ToString("yyyy-MM-dd(hh:mm:ss:ff)");
            this._transverter.AddLocalRecord(domain, address, date);
            date = DateTime.Now.ToString("yyyy-MM-dd(hh:mm:ss:ff)");
            this._transverter.AddLocalRecord(domain, address, date);
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load("transform.xml");
            XmlNode xmlNode = xmlDocument.SelectSingleNode($"/span/mainframe/record[@date='{date}']");
            Assert.IsNotNull(xmlNode);
        }
        
        [TestMethod]
        [DataRow("112.113.152.25")]
        public void TestGetRecordsFromAddress(string address)
        {
            IEnumerable<RecordModel> result = this._transverter.GetRecordsFromAddress(address);
            result = result.Where(r => r.Address.Equals(address));
            Assert.IsTrue(result.Count() > 0);
        }

        [TestMethod]
        [DataRow("test.span.com")]
        public void TestGetRecordsFromDomain(string domain)
        {
            IEnumerable<RecordModel> result = this._transverter.GetRecordsFromDomain(domain);
            result = result.Where(r => r.Domain.Equals(domain));
            Assert.IsTrue(result.Count() > 0);
        }

        [TestMethod]
        [DataRow("112.113.152.25")]
        public void GetRecordFromAddressLate(string address)
        {
            RecordModel testRecord = new RecordModel()
            {
                Address = address,
                Domain = "test.span.com",
                Date = DateTime.Now.ToString("yyy-MM-dd(hh:mm:ss:ff)")
            };
            this._transverter.AddLocalRecord(testRecord);
            RecordModel record = this._transverter.GetRecordFromAddressLate(address);
            Assert.IsTrue(testRecord.Date.Equals(record.Date));
        }


        [TestMethod]
        [DataRow("test.span.com")]
        public void GetRecordFromDomainLate(string domain)
        {
            RecordModel testRecord = new RecordModel()
            {
                Address = "112.113.152.25",
                Domain = domain,
                Date = DateTime.Now.ToString("yyy-MM-dd(hh:mm:ss:ff)")
            };
            this._transverter.AddLocalRecord(testRecord);
            RecordModel record = this._transverter.GetRecordFromDomainLate(domain);
            Assert.IsTrue(testRecord.Date.Equals(record.Date));
        }

    }
}
