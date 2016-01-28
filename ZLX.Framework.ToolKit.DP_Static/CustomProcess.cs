using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;

namespace ZLX.Framework.ToolKit.DP_Static
{
    public class CustomProcess
    {
        /// <summary>
        /// 通过调用存储过程返回List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">存储过程名称</param>
        /// <param name="parameterValues">存储过程参数</param>
        /// <returns></returns>
        public static List<T> GetListByProc<T>(string name, Object[] parameterValues)
        {
            DataSet ds = ExecuteProc(name, parameterValues);
            if (ds==null||ds.Tables.Count==0)
            {
                return null;
            }
            return DTToList<T>(ds.Tables[0]);
        }

        /// <summary>
        /// 通过调用分页存储过程返回List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">分页存储过程名称</param>
        /// <param name="parameterValues">分页存储过程参数,最开始为int pageIndex,int pageSize,</param>
        /// <returns></returns>
        public static List<T> GetListPagedByProc<T>(string name, Object[] parameterValues,
            out int totalCount)
        {
            DataSet ds = ExecuteProc(name, parameterValues);
            if (ds==null||ds.Tables.Count==0)
            {
                totalCount = 0;
                return null;
            }
            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                totalCount = int.Parse(ds.Tables[1].Rows[0][0].ToString());
            }
            else
            {
                totalCount = ds.Tables[0].Rows.Count;
            }

            return DTToList<T>(ds.Tables[0]);
        }


        /// <summary>
        /// DTTOLIST
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> DTToList<T>(DataTable dt)
        {
            if (dt == null)
            {
                return new List<T>();
            }
            List<T> list = new List<T>();
            Type t = typeof(T);
            var propInfos = t.GetProperties();
            List<PropertyInfo> exsitPropInfos = new List<PropertyInfo>();
            foreach (var item in propInfos)
            {
                foreach (DataColumn colItem in dt.Columns)
                {
                    if (colItem.ColumnName.ToLower() == item.Name.ToLower())
                    {
                        exsitPropInfos.Add(item);
                    }
                }
            }
            foreach (DataRow item in dt.Rows)
            {
                T info = Activator.CreateInstance<T>();
                foreach (var propItem in exsitPropInfos)
                {
                    object value = item[propItem.Name];
                    t.GetProperty(propItem.Name).SetValue(info, value == null ? "" : value.ToString(), null);
                }
                list.Add(info);
            }
            return list;
        }

        /// <summary>
        /// 执行存储过程，返回DataSet
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public static DataSet ExecuteProc(string name, Object[] parameterValues)
        {
            return SQLHelper.ExecuteDataset(GetConnectionString(),name,parameterValues);
        }

        /// <summary>
        /// 执行存储过程,不返回参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public static int ExecuteProcNonQuery(string name, Object[] parameterValues)
        {
            return SQLHelper.ExecuteNonQuery(GetConnectionString(),name, parameterValues);
        }

        /// <summary>
        /// 设置连接字符串，不设置则获取配置文件的ConnectionString
        /// </summary>
        public static string ConnectionString { get; set; }

        private static string GetConnectionString()
        {
            if (!string.IsNullOrWhiteSpace(ConnectionString))
            {
                return ConnectionString;
            }
            return System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];

        }
    }
}
