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
using System.Globalization;

namespace SpanTransform.Transverter
{
    public partial class XmlTransverter :  ITransverterable, IDisposable
    {
        private Socket _socket;
        private IPEndPoint _ipEndPoint;
        private int _receiveBufferSize;
        private string _filePath;

        private XmlDocument _xmlDocument;
        private bool _isDispos;
        private bool _isWork;
        
        private bool IsExist => Process.GetProcesses().Any(p => p.ProcessName.Equals("st"));
        public bool IsWork => this._isWork;

        public XmlTransverter(IPEndPoint ipEndPoint = null, string filePath = null)
        {
            
            this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this._ipEndPoint = ipEndPoint ?? Config.DefaultTransverterEndPoint;
            this._receiveBufferSize = 1024;
            this._isWork = false;
            this._isDispos = false;
            this._filePath = filePath ?? Config.DefaultTransverterFilePath;
            this._xmlDocument = new XmlDocument();
            if (File.Exists(this._filePath))
            {
                this._xmlDocument.Load(this._filePath);
            }
            else
            {
                this._xmlDocument.AppendChild(this._xmlDocument.CreateXmlDeclaration("1.0", "utf-8", null));
                this._xmlDocument.AppendChild(this._xmlDocument.CreateElement("span"));
            }
        }

        ~XmlTransverter()
        {
            if (this._isDispos)
            {
                Dispose();
            }
        }
        public void Dispose()
        {
            //this._socket.Disconnect(true);
            this._socket.Close();
            this._socket.Dispose();
            this._isDispos = true;
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
        public void Work()
        {
            if (!this.IsExist)
            {
                //绑定地址
                this._socket.Bind(this._ipEndPoint);
                //监听
                this._socket.Listen(10);
                this._isWork = true;
                while (true)
                {
                    try
                    {
                        Socket requestSocket = this._socket.Accept();
                        Thread thread = new Thread(Do);
                        thread.Start(requestSocket);
                    }
                    catch
                    {

                    }
                }
            }
        }
        public void UnWork()
        {
            if (this.IsExist)
            {
                Process.GetProcessesByName("st").First().Kill();
            }
        }
        
        
        //请求处理线程
        private void Do(object obj)
        {
            Socket socket = obj as Socket;
            byte[] receiveBuffer = new byte[this._receiveBufferSize];
            int receiveCount = socket.Receive(receiveBuffer);
            if(receiveCount > 0)   //接收到数据
            {
                ResponseModel response = new ResponseModel();
                string requestStr = Encoding.Default.GetString(receiveBuffer, 0, receiveCount);
                RequestModel request = JsonSerializer.ToObject<RequestModel>(requestStr);
                
                if(request.Role == RoleType.Provider)    //provider 发送
                {
                    if(request.Operation == OperationType.Update)
                    {
                        string currentDate = DateTime.Now.ToString("yyyy-MM-dd(hh:ss:mm:ff)");    //当前时间
                        AddLocalRecord(request.Domain, request.Address, currentDate);    //添加记录
                        RecordModel record = GetRecordFromDomainLate(request.Domain);    //读取最新记录
                        if (null != record)
                        {
                            response.Record = record;
                            response.Status = record.Date.Equals(currentDate) ? StatusType.Success : StatusType.Fail;
                        }
                    }
                }
                else if(request.Role == RoleType.User)    //user发送
                {
                    if (request.Operation == OperationType.Get)
                    {
                        RecordModel record = null;
                        if (!string.IsNullOrEmpty(request.Domain) && string.IsNullOrEmpty(request.Address))
                        {
                            record = GetRecordFromDomainLate(request.Domain);    //读取最新记录
                        }
                        else if (!string.IsNullOrEmpty(request.Address) && string.IsNullOrEmpty(request.Domain))
                        {
                            record = GetRecordFromAddressLate(request.Address);    //读取最新记录
                        }
                        if (null != record)
                        {
                            response.Record = record;
                            response.Status = StatusType.Success;
                        }
                    }
                }

                string sendStr = JsonSerializer.ToJson<ResponseModel>(response);
                byte[] sendByte = Encoding.ASCII.GetBytes(sendStr);
                socket.Send(sendByte);
                //Socket clientSocket = obj as Socket;
                //if (clientSocket != null)
                //{
                //    string sendStr = JsonSerializer.ToJson<ResponseModel>(response);
                //    byte[] sendByte = Encoding.ASCII.GetBytes(sendStr);
                //    this._socket.SendTo(sendByte, this._socket.RemoteEndPoint);
                //    clientSocket.Close();
                //}
            }
        }

        //获取最新记录
        private RecordModel GetRecordLate(IEnumerable<RecordModel> records)
        {
            RecordModel newRecord = null;
            foreach (RecordModel record in records)
            {
                if (newRecord == null) newRecord = record;
                else
                {
                    if( CompareDate(newRecord.Date, record.Date) < 0)
                    {
                        newRecord = record;
                    }
                }
            }

            return newRecord;
        }

        private int CompareDate(string date1, string date2)
        {
            string[] d1 = date1.Split('-', '(', ':', ')');
            string[] d2 = date2.Split('-', '(', ':', ')');
            for (int i = 0; i < 7; i++)
            {
                int ret = Cpmpate(int.Parse(d1[0]), int.Parse(d2[0]));
                if (ret > 0) return 1;
                else if(ret < 0) return -1;
            }
            return -2;
        }

        private int Cpmpate(int i1, int i2)
        {
            return i1 == i2 ? 0 : i1 > i2 ? 1 : -1;
        }



    }
}
