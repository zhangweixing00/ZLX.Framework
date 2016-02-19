using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZLX.Framework.ToolKit.BytesHelper
{
    /// <summary>
    /// 提供字节封装组合的方法类
    /// star
    /// 2016-2-18
    /// </summary>
    public class BytesMaker
    {
        #region 整型

        /// <summary>
        /// Int
        /// </summary>
        /// <param name="num">数值</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public static byte[] GetIntBytes(int num, int length=4)
        {
            return GetLongBytes((long)num, length);
        }
        /// <summary>
        /// Long
        /// </summary>
        /// <param name="num"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] GetLongBytes(long num, int length)
        {
            byte[] result;
            if (length > 8 || length < 1)
            {
                result = null;
            }
            else
            {
                byte[] array = new byte[length];
                for (int i = 0; i < length; i++)
                {
                    array[i] = (byte)(num >> i * 8 & 255L);
                }
                result = array;
            }
            return result;
        }

        #endregion

        #region 字符串
        /// <summary>
        /// 默认使用Unicode编码
        /// </summary>
        public static readonly Encoding DEFAULT_ENCODING = Encoding.Unicode;

        /// <summary>
        /// 字符串编码
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="encoding"></param>
        /// <param name="lenLength"></param>
        /// <returns></returns>
        public static byte[] GetEncodingBytesWithLength(string content, Encoding encoding, int lenLength = 4)
        {
            if (content == null)
            {
                content = string.Empty;
            }
            if (encoding == null)
            {
                encoding = DEFAULT_ENCODING;
            }

            byte[] array = encoding.GetBytes(content);
            byte[] intBytes = BytesMaker.GetIntBytes(array.Length, lenLength);
            byte[] totalBytes = new byte[array.Length + lenLength];
            for (int i = 0; i < totalBytes.Length; i++)
            {
                if (i < lenLength)
                {
                    totalBytes[i] = intBytes[i];
                }
                else
                {
                    totalBytes[i] = array[i - lenLength];
                }
            }
            return totalBytes;
        }
        #endregion

        #region bool型
        /// <summary>
        /// bool型转成一个字节
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBoolBytes(bool value)
        {
            return new byte[] { (byte)(value ? 1 : 0) };
        }
        #endregion

        #region byte数组处理

        public static byte[] AppendHeaderToBytes(byte[] body, int lang = 4)
        {
            byte[] intBytes;
            byte[] array;
            if (body == null)
            {
                intBytes = BytesMaker.GetIntBytes(0, lang);
                array = new byte[lang];
            }
            else
            {
                intBytes = BytesMaker.GetIntBytes(body.Length, lang);
                array = new byte[lang + body.Length];
            }
            for (int i = 0; i < array.Length; i++)
            {
                if (i < lang)
                {
                    array[i] = intBytes[i];
                }
                else
                {
                    array[i] = body[i - lang];
                }
            }
            return array;
        }

        #endregion

        #region 合并
        /// <summary>
        /// 合并byte数组，不加头
        /// </summary>
        /// <param name="leftData"></param>
        /// <param name="rightData"></param>
        /// <returns></returns>
        public static byte[] Combine(byte[] leftData, byte[] rightData)
        {
            return Combine<byte>(leftData, rightData);
        }

        /// <summary>
        /// 合并数组，泛型方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="leftData"></param>
        /// <param name="rightData"></param>
        /// <returns></returns>
        public static T[] Combine<T>(T[] leftData, T[] rightData)
        {
            if (leftData == null && rightData == null)
            {
                return new T[0];
            }
            if (leftData == null)
            {
                return rightData;
            }
            if (rightData == null)
            {
                return leftData;
            }

            int totalLength = leftData.Length + rightData.Length;
            T[] totalBytes = new T[totalLength];
            Array.Copy(leftData, 0, totalBytes, 0, leftData.Length);
            Array.Copy(rightData, 0, totalBytes, leftData.Length, rightData.Length);
            return totalBytes;
        }
        #endregion
    }
}