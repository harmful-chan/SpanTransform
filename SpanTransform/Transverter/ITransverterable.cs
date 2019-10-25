using SpanTransform.Models;
using System;
using System.Collections.Generic;

namespace SpanTransform.Transverter
{
    public enum TransverterStatus
    {
        IsListening,
        IsClose
    }
    public interface ITransverterable: IDisposable
    {
        public bool IsWork{ get; }
        public RecordModel GetRecordFromDomainLate(string domain);
        public RecordModel GetRecordFromAddressLate(string address);
        public IEnumerable<RecordModel> GetRecordsFromDomain(string domain);
        public IEnumerable<RecordModel> GetRecordsFromAddress(string address);
        public void AddLocalRecord(RecordModel recordModel);
        public void AddLocalRecord(string domain, string address, string date);

        public void Work();
        public void UnWork();
    }
}
