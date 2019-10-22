using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;
using SpanTransform.Models;

namespace SpanTransform.Transverter
{
    public partial class UdpTransverter : ITransverterable
    {
        private UdpClient _udpClient;
        private IPEndPoint _ipEndPoint;
        private XmlDocument _xmlDocument;
        public UdpTransverter()
        {
            this._udpClient = new UdpClient();
            this._ipEndPoint = new IPEndPoint(IPAddress.Any, 8898);
            this._xmlDocument = new XmlDocument();
        }
        public void AddLocalRecord(RecordModel recordModel)
        {
            //入参合法校验
            if (string.IsNullOrEmpty(recordModel.Domain) ||
                string.IsNullOrEmpty(recordModel.Address) ||
                null == recordModel.UpdateDateTime)
                throw new ArgumentNullException("recordModel");

            //加载文档
            string filePath = "transform.xml";
            if (File.Exists(filePath))
            {
                this._xmlDocument.Load(filePath);
            }
            else
            {
                this._xmlDocument.AppendChild(this._xmlDocument.CreateXmlDeclaration("1.0", "utf-8", null));
                this._xmlDocument.AppendChild(this._xmlDocument.CreateElement("root"));
            }

            //搜索域名根节点
            XmlNode xmlNode = null;
            XmlNodeList xmlNodeList = this._xmlDocument.GetElementsByTagName(recordModel.Domain);
            if(xmlNodeList.Count == 1)    //记录存在
            {
                xmlNode = xmlNodeList.Item(0);
            }
            else if(xmlNodeList.Count == 0)
            {
                XmlElement xmlElement = this._xmlDocument.CreateElement("mainframe");
                xmlElement.SetAttribute("domain", recordModel.Domain);
                xmlElement.SetAttribute("date", recordModel.Date);
                XmlElement rootElement = this._xmlDocument.DocumentElement;
                rootElement.AppendChild(this._xmlDocument.AppendChild(xmlElement));
            }

            bool isExist = false;
            //相同节点更新时间
            foreach (XmlNode item in xmlNode.ChildNodes)
            {
                XmlElement itemlElement = item as XmlElement;
                string oldIP = itemlElement.GetAttribute("ip");
                if (recordModel.Address.Equals(oldIP))    //相同
                {
                    itemlElement.SetAttribute("date", recordModel.Date);
                    isExist = true;
                }
            }

            //不存在添加节点
            if (!isExist)
            {
                XmlElement xmlElement = this._xmlDocument.CreateElement("record");
                xmlElement.SetAttribute("ip", recordModel.Address);
                xmlElement.SetAttribute("date", recordModel.Date);
                xmlNode.AppendChild(xmlElement);
            }

            //保存
            this._xmlDocument.Save("filePath");
        }

        public string GetRecordFromAddressLater(string address)
        {
            throw new NotImplementedException();
        }

        public string GetRecordFromDomainLater(string domain)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RecordModel> GetRecordsFromAddress(string address)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RecordModel> GetRecordsFromDomain(string domain)
        {
            throw new NotImplementedException();
        }

        public void Reboot()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
