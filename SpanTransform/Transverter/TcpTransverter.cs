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
using SpanTransform.Common;
using System.Threading;

namespace SpanTransform.Transverter
{
    public partial class TcpTransverter : TcpBase, ITransverterable
    {
        private Socket _socket;
        private IPEndPoint _ipEndPoint;
        private int _receiveBufferSize;
        private string _filePath;

        private XmlDocument _xmlDocument;
        public TcpTransverter(IPEndPoint ipEndPoint = null)
        {
            
            this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this._ipEndPoint = ipEndPoint ?? base.TransverterEndPoint;
            this._receiveBufferSize = 1024;

            this._filePath = "transform.xml";
            if (File.Exists(this._filePath))
            {
                this._xmlDocument = new XmlDocument();
                this._xmlDocument.Load(this._filePath);
            }
            else
            {
                this._xmlDocument.AppendChild(this._xmlDocument.CreateXmlDeclaration("1.0", "utf-8", null));
                this._xmlDocument.AppendChild(this._xmlDocument.CreateElement("span"));
            }
        }
        public void AddLocalRecord(RecordModel recordModel)
        {
            //入参合法校验
            if (string.IsNullOrEmpty(recordModel.Domain) ||
                string.IsNullOrEmpty(recordModel.Address) ||
                string.IsNullOrEmpty(recordModel.Date))
                throw new ArgumentNullException("recordModel");


            XmlNode spanNode = this._xmlDocument.SelectSingleNode("span");
            XmlNode domainNode = spanNode.SelectSingleNode($"/span/mainframe[@domain='{recordModel.Domain}']");
            if(null == domainNode)    //不在在
            {
                XmlElement mainframeElement = this._xmlDocument.CreateElement("mainframe");
                mainframeElement.SetAttribute("domain", recordModel.Domain);
                mainframeElement.SetAttribute("date", recordModel.Date);
                domainNode = spanNode.AppendChild(mainframeElement);
            }

            //查找record
            XmlNode recordNode = domainNode.SelectSingleNode($"/span/mainframe/record[@address='{recordModel.Address}']");
            if(null == recordNode)   //不在在
            {
                XmlElement recordElement = this._xmlDocument.CreateElement("record");
                recordElement.SetAttribute("address", recordModel.Address);
                recordElement.SetAttribute("date", recordModel.Date);
                recordNode = domainNode.AppendChild(recordElement);
            }
            else    //存在
            {
                ((XmlElement)recordNode).SetAttribute("date", recordModel.Date);
            }

            //保存
            this._xmlDocument.Save(this._filePath);
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
            XmlNodeList recordNodes = this._xmlDocument.SelectNodes($"//record[@address='{address}']");
            List<RecordModel> records = new List<RecordModel>();
            foreach (XmlElement node in recordNodes)
            {
                records.Add(new RecordModel()
                {
                    Domain = (node.ParentNode as XmlElement).GetAttribute("domain"),    //父节点domain
                    Address = node.GetAttribute("address"),   //本节点 address
                    Date = node.GetAttribute("date")    //本节点 date
                });
            }

            return records.Count >= 0 ? records : null;

        }

        public IEnumerable<RecordModel> GetRecordsFromDomain(string domain)
        {
            XmlNodeList recordNodes = this._xmlDocument.SelectNodes($"//mainframe[@domain='{domain}']").Item(0).ChildNodes;
            List<RecordModel> records = new List<RecordModel>();
            foreach (XmlElement node in recordNodes)
            {
                records.Add(new RecordModel()
                {
                    Domain = (node.ParentNode as XmlElement).GetAttribute("domain"),    //父节点domain
                    Address = node.GetAttribute("address"),   //本节点 address
                    Date = node.GetAttribute("date")    //本节点 date
                });
            }

            return records.Count > 0 ? records : null;
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
                    //this.Listener();
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
            //绑定地址
            this._socket.Bind(this._ipEndPoint);
            //监听
            this._socket.Listen(10);
            //
            Socket requestSocket =  this._socket.Accept();
            Thread thread = new Thread(Do);
            thread.Start();
        }

        //请求处理线程
        private void Do(object obj)
        {
            byte[] receiveBuffer = new byte[this._receiveBufferSize];
            int receiveCount = this._socket.Receive(receiveBuffer);
            if(receiveCount > 0)
            {
                string requestStr = Encoding.Default.GetString(receiveBuffer);
                RequestModel request = JsonSerializer.ToObject<RequestModel>(requestStr);
                if(request.Role == RoleType.Transverter)
                {
                    if(request.Operation == OperationType.Start)
                    {

                    }
                    else if (request.Operation == OperationType.Stop)
                    {

                    }
                    else if (request.Operation == OperationType.Reboot)
                    {

                    }
                }
            }
            
        }

        //获取最新记录
        private RecordModel GetRecordLate(IEnumerable<RecordModel> records)
        {
            RecordModel newRecord = null;
            foreach (RecordModel record in records)
            {
                if (newRecord == null) newRecord = record;

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
