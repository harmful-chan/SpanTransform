using SpanTransform.Models;

namespace SpanTransform.Provider
{
    public interface IProviderable
    {
        public ResultModel UpdateTransverterRecord(RecordModel recordModel);
    }
}
