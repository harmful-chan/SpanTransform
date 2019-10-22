using SpanTransform.Models;
using System.Collections.Generic;

namespace SpanTransform.Transverter
{
    public interface ITransverterable
    {
        public RecordModel GetRecordFromDomainLate(string domain);
        public RecordModel GetRecordFromAddressLate(string address);
        public IEnumerable<RecordModel> GetRecordsFromDomain(string domain);
        public IEnumerable<RecordModel> GetRecordsFromAddress(string address);
        public void AddLocalRecord(RecordModel recordModel);
        public void AddLocalRecord(string domain, string address, string date);
        public void Start();
        public void Stop();
        public void Reboot();
    }
}
