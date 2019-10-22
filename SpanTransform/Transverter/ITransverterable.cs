using SpanTransform.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpanTransform.Transverter
{
    public interface ITransverterable
    {
        public string GetRecordFromDomainLater(string domain);
        public string GetRecordFromAddressLater(string address);
        public IEnumerable<RecordModel> GetRecordsFromDomain(string domain);
        public IEnumerable<RecordModel> GetRecordsFromAddress(string address);
        public void AddLocalRecord(RecordModel recordModel);
        public void Start();
        public void Stop();
        public void Reboot();
    }
}
