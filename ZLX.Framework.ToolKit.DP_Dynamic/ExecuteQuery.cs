using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZLX.Framework.ToolKit.DP_Dynamic.QueryBase;
using ZLX.Framework.ToolKit.DP_Dynamic.Core;

namespace ZLX.Framework.ToolKit.DP_Dynamic
{
    public class ExecuteQuery
    {
        public static List<Object> GetList(QueryInfo info)
        {
            return CustomProcess.ExecuteProc(info.ProcName, info.Params.ToArray()).ToList();
        }
        public static object GetInfo(QueryInfo info)
        {
            return CustomProcess.ExecuteProc(info.ProcName, info.Params.ToArray()).ToObject();
        }
    }
}
