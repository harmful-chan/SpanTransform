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

namespace SpanTransform.Test
{
    [TestClass]
    public class TransverterTest : TestBase
    {
        private ITransverterable _transverter;
        private XmlDocument _xmlDocument;
        private string _filePath;
        private int _receiveBufferSize;
        private string _oneDate;
        private string _twoDate;
        private string _threeDate;
        public TransverterTest():base()
        {
            this._filePath = "test.xml";
            this._receiveBufferSize = 1024;
            this._transverter = new XmlTransverter(base.RemoteTestEndpoint, this._filePath);
            this._xmlDocument = new XmlDocument();
            this._oneDate = Config.NowDateFormatted;
            Thread.Sleep(10);
            this._twoDate = Config.NowDateFormatted;
            Thread.Sleep(10);
            this._threeDate = Config.NowDateFormatted;
        }

        [TestMethod] 
        [DataRow("test.span.com", "0.0.0.0", "0000-00-00(00:00:00)")]
        public void TestAddLocalRecord(string domain, string address, string date)
        {
            //添加记录
            this._transverter.AddLocalRecord(domain, address, this._oneDate);
            this._transverter.AddLocalRecord(domain, address, this._twoDate);
            this._transverter.AddLocalRecord(domain, address, this._threeDate);

            this._xmlDocument.Load(this._filePath);

            //第一次记录
            XmlNode xmlNode = this._xmlDocument.SelectSingleNode($"/span/mainframe/record[@date='{_threeDate}']");
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
        [DataRow("0.0.0.0")]
        public void TestGetRecordFromAddressLate(string address)
        {
            RecordModel record = this._transverter.GetRecordFromAddressLate(address);
            Assert.IsTrue(this._threeDate.Equals(record.Date));
        }


        [TestMethod]
        [DataRow("test.span.com")]
        public void TestGetRecordFromDomainLate(string domain)
        {
            RecordModel record = this._transverter.GetRecordFromDomainLate(domain);
            Assert.IsTrue(_threeDate.Equals(record.Date));
        }

        [TestMethod]
        [DataRow("test.span.com", "0.0.0.0")]
        public void TestStart(string domain, string address)
        {
            //启动线程
            Thread thread = new Thread(this._transverter.Work);
            thread.Start();
            
            //等待剑灵
            while (!this._transverter.IsWork)
            {
                Thread.Sleep(1000);
            }

            //连接服务器
            RequestModel request = new RequestModel();
            request.Role = RoleType.Provider;
            request.Operation = OperationType.Update;
            request.Domain = domain;
            request.Address = address;
            base.CommonSocket.Connect(base.RemoteTestEndpoint);


            //发送数据
            byte[] sendBuffer = Encoding.ASCII.GetBytes(JsonSerializer.ToJson<RequestModel>(request));
            base.CommonSocket.Send(sendBuffer);
            
            //接受数据
            byte[] receiveBuffer = new byte[this._receiveBufferSize];
            int receiveCount = base.CommonSocket.Receive(receiveBuffer);
            ResponseModel response = null;
            if (receiveCount > 0)
            {
                string receiveStr = Encoding.Default.GetString(receiveBuffer, 0, receiveCount);
                response = JsonSerializer.ToObject<ResponseModel>(receiveStr);
            }

            this._transverter.Dispose();
            Assert.IsTrue(response.Status == StatusType.Success); ;
        }

    }
}
