using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Xml;
using SpanTransform.Models;
using System.Linq;
using System.Diagnostics;
using System.Text;
using Kooboo.Json;

namespace SpanTransform.Transverter
{
    public partial class TcpTransverter : ITransverterable
    {
        private UdpClient _udpListener;
        private IPEndPoint _ipEndPoint;
        private string _filePath = "transform.xml";

        public TcpTransverter()
        {
            this._udpListener = new UdpClient();
            this._ipEndPoint = new IPEndPoint(IPAddress.Any, 8898);
        }
        public void AddLocalRecord(RecordModel recordModel)
        {
            XmlDocument xmlDocument = new XmlDocument();

            //入参合法校验
            if (string.IsNullOrEmpty(recordModel.Domain) ||
                string.IsNullOrEmpty(recordModel.Address) ||
                null == recordModel.Date)
                throw new ArgumentNullException("recordModel");

            //加载文档
            string filePath = this._filePath;
            if (File.Exists(filePath))
            {
                xmlDocument.Load(filePath);
            }
            else
            {
                xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "utf-8", null));
                xmlDocument.AppendChild(xmlDocument.CreateElement("span"));
            }

            //搜索域名根节点
            XmlNode xmlNode = null;
            XmlNodeList xmlNodeList = xmlDocument.GetElementsByTagName("mainframe");
            if (xmlNodeList.Count == 1)    //记录存在
            {
                xmlNode = xmlNodeList.Item(0);
            }
            else if (xmlNodeList.Count == 0)
            {
                XmlElement xmlElement = xmlDocument.CreateElement("mainframe");
                xmlElement.SetAttribute("domain", recordModel.Domain);
                xmlElement.SetAttribute("date", recordModel.Date);
                XmlNode spanElement = xmlDocument.SelectSingleNode("span");
                spanElement.AppendChild(xmlElement);
                xmlNode = xmlElement as XmlNode;
            }

            bool isExist = false;
            //相同节点更新时间
            foreach (XmlNode item in xmlNode.ChildNodes)
            {
                XmlElement itemlElement = item as XmlElement;
                string oldIP = itemlElement.GetAttribute("address");
                if (recordModel.Address.Equals(oldIP))    //相同
                {
                    itemlElement.SetAttribute("date", recordModel.Date);
                    isExist = true;
                }
            }

            //不存在添加节点
            if (!isExist)
            {
                XmlElement xmlElement = xmlDocument.CreateElement("record");
                xmlElement.SetAttribute("address", recordModel.Address);
                xmlElement.SetAttribute("date", recordModel.Date);
                xmlNode.AppendChild(xmlElement);
            }

            //保存
            xmlDocument.Save(this._filePath);
        }

        public void AddLocalRecord(string domain, string address, string date)
        {
            AddLocalRecord(new RecordModel()
            {
                Domain = domain,
                Address = address,
                Date = date
            });
        }

        public RecordModel GetRecordFromAddressLate(string address)
        {
            IEnumerable<RecordModel> records = this.GetRecordsFromAddress(address);
            return GetRecordLate(records);
        }

        public RecordModel GetRecordFromDomainLate(string domain)
        {
            IEnumerable<RecordModel> records = this.GetRecordsFromDomain(domain);
            return GetRecordLate(records);
        }

        public IEnumerable<RecordModel> GetRecordsFromAddress(string address)
        {
            
            if (!File.Exists(this._filePath))
                return null;

            //加载文档
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(this._filePath);
            //读取所有record节点
            List<XmlElement> xmlElements = new List<XmlElement>();
            foreach (XmlElement e in xmlDocument.GetElementsByTagName("record"))
            {
                xmlElements.Add(e);
            }
            //匹配address
            List<RecordModel> records = new List<RecordModel>();
            foreach (XmlElement e in xmlElements.Where(r => r.GetAttribute("address").Equals(address)))
            {
                records.Add(new RecordModel()
                {
                    Domain = (e.ParentNode as XmlElement).GetAttribute("domain"),    //父节点domain
                    Address = e.GetAttribute("address"),   //本节点 address
                    Date = e.GetAttribute("date")    //本节点 date
                });
            }

            return records.Count() >= 0 ? records : null;

        }

        public IEnumerable<RecordModel> GetRecordsFromDomain(string domain)
        {
            if (!File.Exists(this._filePath))
                return null;

            //加载文档
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(this._filePath);
            //读取mainframe节点
            List<XmlElement> xmlElements = new List<XmlElement>();
            foreach (XmlElement e in xmlDocument.GetElementsByTagName("mainframe"))
            {
                xmlElements.Add(e);
            }
            //匹配domain节点
            List<XmlElement> domains = xmlElements.Where(e => e.GetAttribute("domain").Equals(domain)).ToList();
            //子阶段record全部返回
            List<RecordModel> records = new List<RecordModel>();
            foreach (XmlElement d in domains)
            {
                foreach (XmlElement c in d.ChildNodes)
                {
                    records.Add(new RecordModel()
                    {
                        Domain = (c.ParentNode as XmlElement).GetAttribute("domain"),    //父节点domain
                        Address = c.GetAttribute("address"),   //本节点 address
                        Date = c.GetAttribute("date")    //本节点 date
                    });
                }
                
            } 

            return records.Count() > 0 ? records : null;
        }

        public void Reboot()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            if (!this.IsRunning)
            {
                while (true)
                {
                    this.Listener();
                }
            }
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        //监听程序
        private void Listener()
        {
            byte[] buffer = this._udpListener.Receive(ref this._ipEndPoint);
            string msg = Encoding.Unicode.GetString(buffer);
            RequestModel request = JsonSerializer.ToObject<RequestModel>(msg);
            ResponseModel responce = new ResponseModel()
            {
                Status = "failed"
            };
            if (request.Operation.Equals("update"))
            {
                this.AddLocalRecord(domain: request.Domain, 
                    address: request.Address, 
                    date:DateTime.Now.ToString("yyyy-MM-dd(hh:mm:ss:ff)"));
                RecordModel record = this.GetRecordFromDomainLate(request.Domain);
                responce.Status = "succeed";
                responce.Record = record;
            }
            buffer = Encoding.ASCII.GetBytes(JsonSerializer.ToJson<ResponseModel>(responce));
            this._udpListener.Send(buffer, buffer.Length);
        }

        //获取最新记录
        private RecordModel GetRecordLate(IEnumerable<RecordModel> records)
        {
            RecordModel newRecord = null;
            foreach (RecordModel record in records)
            {
                if (newRecord == null)
                {
                    newRecord = record;
                    continue;
                }

                if (0 > DateTime.Compare(
                    DateTime.ParseExact(newRecord.Date, "yyyy-MM-dd(hh:mm:ss:ff)", System.Globalization.CultureInfo.CurrentCulture),
                    DateTime.ParseExact(record.Date, "yyyy-MM-dd(hh:mm:ss:ff)", System.Globalization.CultureInfo.CurrentCulture)))
                {
                    newRecord = record;
                }
            }

            return newRecord;
        }

        //检测UDP服务器是否正在启动
        public bool IsRunning
        {
            get 
            {
                Process[] pros = Process.GetProcesses();
                return pros.Any(p => 0 >= p.ProcessName.IndexOf("st"));
            }
        }
    }
}
