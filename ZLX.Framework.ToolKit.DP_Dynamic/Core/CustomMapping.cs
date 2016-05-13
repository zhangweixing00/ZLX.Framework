using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ZLX.Framework.ToolKit.DP_Dynamic.Core
{
    public class CustomMapping
    {
        public static Object GetObject(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            List<string> props = new List<string>();
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (DataColumn item in dt.Columns)
            {
                props.Add(item.ColumnName);
                dict.Add(item.ColumnName, dt.Rows[0][item.ColumnName].ToString());
            }

            Type type = new CustomType().CreateTypeAdvance(props);

            Object result = new CustomType().GetInstance(type, dict);

            return result;
        }
        public static List<Object> GetList(DataTable dt)
        {
            List<object> objList = new List<object>();
            if (dt == null || dt.Rows.Count == 0)
            {
                return objList;
            }

            List<string> props = new List<string>();

            foreach (DataColumn item in dt.Columns)
            {
                props.Add(item.ColumnName);
            }

            CustomType customType = new CustomType();
            Type type = customType.CreateTypeAdvance(props);

            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                foreach (DataColumn item in dt.Columns)
                {
                    dict.Add(item.ColumnName, dr[item.ColumnName].ToString());
                }
                Object result = customType.GetInstance(type, dict);
                objList.Add(result);
            }
            return objList;
        }
    }
}
