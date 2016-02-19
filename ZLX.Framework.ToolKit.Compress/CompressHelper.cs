using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace ZLX.Framework.ToolKit.Compress
{
    /// <summary>
    ///GZIP格式，使用微软自带的压缩方法
    ///暂时不用ICSharpCode.SharpZipLib
    ///STAR
    /// </summary>
    public class CompressHelper
    {
        /// <summary>
        /// 压缩字节数组
        /// </summary>
        /// <param name="str"></param>
        public static byte[] Compress(byte[] inputBytes)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                using (GZipStream zipStream = new GZipStream(outStream, CompressionMode.Compress, true))
                {
                    zipStream.Write(inputBytes, 0, inputBytes.Length);
                    zipStream.Close(); //很重要，必须关闭，否则无法正确解压
                    return outStream.ToArray();
                }
            }
        }

        /// <summary>
        /// 解压缩字节数组
        /// </summary>
        /// <param name="str"></param>
        public static byte[] Decompress(byte[] inputBytes)
        {

            using (MemoryStream inputStream = new MemoryStream(inputBytes))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    using (GZipStream zipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                    {
                        zipStream.CopyTo(outStream);
                        zipStream.Close();
                        return outStream.ToArray();
                    }
                }

            }
        }


        /// <summary>
        /// 压缩字符串
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Compress(string input)
        {
            byte[] inputBytes = Encoding.Default.GetBytes(input);
            byte[] result = Compress(inputBytes);
            return Convert.ToBase64String(result);
        }
        /// <summary>
        /// 解压缩字符串
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Decompress(string input)
        {
            byte[] inputBytes = Convert.FromBase64String(input);
            byte[] depressBytes = Decompress(inputBytes);
            return Encoding.Default.GetString(depressBytes);
        }
    }
}
