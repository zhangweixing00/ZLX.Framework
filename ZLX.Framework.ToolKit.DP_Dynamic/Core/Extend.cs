using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ZLX.Framework.ToolKit.DP_Dynamic.Core
{
    public static class Extend
    {
        public static Object ToObject(this DataSet ds)
        {
            if (ds==null||ds.Tables.Count<=0)
            {
                return null;
            }

            return CustomMapping.GetObject(ds.Tables[0]);
        }
        public static List<Object> ToList(this DataSet ds)
        {
            if (ds==null||ds.Tables.Count<=0)
            {
                return null;
            }

            return CustomMapping.GetList(ds.Tables[0]);
        }
    }
}
