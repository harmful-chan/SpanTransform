using SpanTransform.Models;

namespace SpanTransform.Provider
{
    public interface IProviderable
    {
        public OutParamModel UpdateTransverterRecord(InParamModel inParam);
    }
}
