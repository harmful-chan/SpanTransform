using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using SpanTransform.Models;

namespace SpanTransform.Transverter
{
    public class UdpTransverter : UdpClient, ITransverterable
    {
        public void AddLocalRecord(RecordModel recordModel)
        {
            throw new NotImplementedException();
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
    }
}
