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
        public bool IsWork => this._isWork;
        private bool _isExist;
        private bool IsExist { set { this._isExist = value; } 
            get 
            {
                this._isExist = this.GetStIds().Length == 1;
                return this._isExist;
            } 
        }
        private bool _isExist2;
        private bool IsExist2
        {
            set { this._isExist2 = value; }
            get
            {
                this._isExist2 = this.GetStIds().Length > 1;
                return this._isExist2;
            }
        }

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
        public IEnumerable<RecordModel> GetRecordsFromAddress(string address)
        {
            try
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
            catch
            {
                return null;
            }
            

        }
        public IEnumerable<RecordModel> GetRecordsFromDomain(string domain)
        {
            try
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
            catch
            {
                return null;
            }
            
        }
        public void Work()
        {
            if (!this.IsExist2)
            {
                //绑定地址
                this._socket.Bind(this._ipEndPoint);
                //监听
                this._socket.Listen(10);
                this._isWork = true;
                Config.Log(LogTypes.Listening, $"listening :" + this._ipEndPoint.Address.ToString());
                while (true)
                {
                    try
                    {
                        
                        Socket requestSocket = this._socket.Accept();
                        Config.Log(LogTypes.Listening, $"receive message from:" + (requestSocket.RemoteEndPoint as IPEndPoint).Address.ToString());
                        Thread thread = new Thread(Do);
                        thread.Start(requestSocket);
                    }
                    catch(Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            else
            {
                string idStr = "";
                foreach (int id in GetStIds())
                {
                    idStr += id + " ";
                }
                throw new Exception("transverter is exist, id:" + idStr);
            }
        }
        public void UnWork()
        {
            if (this.IsExist2)
            {
                int[] v = this.GetStIds();
                Config.Log(LogTypes.Kill, $"kill process id:" + v[0]);
                Process.GetProcessById(v[0]).Kill();
            }
            else
            {
                throw new Exception("transverter is not exist.");
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
                        string currentDate = Config.NowDateFormatted;    //当前时间
                        AddLocalRecord(request.Domain, request.Address, currentDate);    //添加记录
                        IEnumerable<RecordModel> records = GetRecordsFromDomain(request.Domain);    //读取最新记录
                        if (null != records)
                        {
                            response.Records = records;
                            response.Status = records.Any(r => r.Date.Equals(currentDate)) ? StatusType.Success : StatusType.Fail;
                        }
                    }
                }
                else if(request.Role == RoleType.User)    //user发送
                {
                    if (request.Operation == OperationType.Get)
                    {
                        IEnumerable<RecordModel> records = null;
                        if (!string.IsNullOrEmpty(request.Domain) && string.IsNullOrEmpty(request.Address))
                        {
                            records = GetRecordsFromDomain(request.Domain);    //读取最新记录
                        }
                        else if (!string.IsNullOrEmpty(request.Address) && string.IsNullOrEmpty(request.Domain))
                        {
                            records = GetRecordsFromAddress(request.Address);    //读取最新记录
                        }
                        if (null != records)
                        {
                            response.Records = records;
                            response.Status = records.Count() > 0 ? StatusType.Success : StatusType.Fail;
                        }
                    }
                }

                string sendStr = JsonSerializer.ToJson<ResponseModel>(response);
                byte[] sendByte = Encoding.ASCII.GetBytes(sendStr);
                socket.Send(sendByte);

                socket.Disconnect(true);
                socket.Close();
                socket.Dispose();
            }
        }

        private int[] GetStIds()
        {
            Process[] process = Process.GetProcessesByName("st");
            int[] ids = Array.ConvertAll<Process, int>(process, p => p.Id);
            return ids;
        }



    }
}
