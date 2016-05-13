using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZLX.Framework.ToolKit.DP_Dynamic
{
    public class QueryInfo
    {
        public QueryInfo()
        {
            Params = new List<Object>();
        }
        public string ProcName { get; set; }
        public List<Object> Params { get; set; }
    }
}
