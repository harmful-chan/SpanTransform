using SpanTransform.Common;
using SpanTransform.Models;
using System.Collections.Generic;

namespace SpanTransform.Serializer
{
    public interface ICmdSerializerable<T> where T: IParamterable
    {
        public string[] Args { get; set; }
        public IEnumerable<CmdMatchedGroupModel> CmdMatchedGroups { get; set; }
        public void AddMatchedGroup(CmdMatchedGroupModel cmdMatchedGroup);
        public T ToModel();
    }
}
