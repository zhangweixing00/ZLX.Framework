using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZLX.Framework.ToolKit.BytesHelper
{
    /// <summary>
    /// 提供字节读取的方法类
    /// star
    /// 2016-2-18
    /// </summary>
    public class BytesReader
    {
        public const int DEFAULT_INT_LEN = 4;
        public static readonly Encoding DEFAULT_ENCODING = Encoding.Unicode;
        protected Encoding encoding;
        private byte[] body = null;
        private int offset = 0;
        public int Position
        {
            get
            {
                return this.offset;
            }
            set
            {
                this.offset = value;
            }
        }

        public BytesReader(byte[] body, Encoding encode)
        {
            this.body = body;
            this.encoding = encode;
        }
        public BytesReader(byte[] body)
        {
            this.body = body;
            this.encoding = DEFAULT_ENCODING;
        }
        public bool ReadBoolean()
        {
            return this.body[this.offset++] != 0;
        }
        public byte ReadByte()
        {
            return this.body[this.offset++];
        }

        /// <summary>
        /// 读取整型
        /// </summary>
        /// <param name="intLen"></param>
        /// <returns></returns>
        public int ReadInt(int intLen = 4)
        {
            int result;
            if (this.offset + intLen > this.body.Length)
            {
                result = 0;
            }
            else
            {
                int num = 0;
                for (int i = 0; i < intLen; i++)
                {
                    num |= (int)(255 & this.body[this.offset + i]) << i * 8;
                }
                this.offset += intLen;
                result = num;
            }
            return result;
        }

        public long ReadLong(int intLen)
        {
            long result;
            if (this.offset + intLen > this.body.Length)
            {
                result = 0L;
            }
            else
            {
                long num = 0L;
                for (int i = 0; i < intLen; i++)
                {
                    long num2 = (long)(255 & this.body[this.offset + i]);
                    long num3 = num2 << i * 8;
                    num |= num3;
                }
                this.offset += intLen;
                result = num;
            }
            return result;
        }

        /// <summary>
        /// 读取字符串
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public string ReadString(int length = 4)
        {
            string result = "";
            if (this.offset + length <= this.body.Length)
            {
                try
                {
                    int stringLength = ReadInt(length);
                    //this.offset += length;
                    if (stringLength != 0)
                    {
                        result = this.encoding.GetString(this.body, this.offset, stringLength);
                        this.offset += stringLength;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            return result;
        }

        /// <summary>
        /// 数据保护头信息
        /// </summary>
        /// <returns></returns>
        public byte[] ReadBytesWithHeader()
        {
            byte[] data = new byte[0];
            int length = ReadInt();
            //this.offset += 4;
            if (length != 0 && this.offset + length <= this.body.Length)
            {
                return ReadBytes(length);
            }
            return data;
        }

        /// <summary>
        /// 直接读取len长度的byte
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        public byte[] ReadBytes(int len)
        {
            byte[] array = new byte[len];
            Array.Copy(this.body, this.offset, array, 0, len);
            this.offset += len;
            return array;
        }

        /// <summary>
        /// 打印body数据
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result;
            if (this.body == null)
            {
                result = string.Empty;
            }
            else
            {
                StringBuilder stringBuilder = new StringBuilder();
                int num = 10;
                int num2 = this.offset;
                int num3 = this.body.Length % num;
                int num4 = this.body.Length / num;
                this.offset = 0;
                stringBuilder.AppendLine(string.Format("Bytes body length:{0}:", this.body.Length));
                for (int i = 0; i <= num4; i++)
                {
                    byte[] array = null;
                    if (i < num4)
                    {
                        array = this.ReadBytes(num);
                    }
                    if (i == num4 && num3 > 0)
                    {
                        array = this.ReadBytes(num3);
                    }
                    if (array != null)
                    {
                        StringBuilder stringBuilder2 = new StringBuilder();
                        StringBuilder stringBuilder3 = new StringBuilder();
                        StringBuilder stringBuilder4 = new StringBuilder();
                        int num5 = i * num;
                        int num6 = i * num + array.Length - 1;
                        stringBuilder.AppendFormat("Index[{0}-{1}]:", num5.ToString("000"), num6.ToString("000"));
                        for (int j = 0; j < array.Length; j++)
                        {
                            stringBuilder2.AppendFormat(" {0}", array[j].ToString("000"));
                            string text = array[j].ToString("X");
                            if (j % 2 == 0)
                            {
                                stringBuilder3.AppendFormat("\t{0}", (text.Length > 1) ? text : ("0" + text));
                            }
                            else
                            {
                                stringBuilder3.AppendFormat(" {0}", (text.Length > 1) ? text : ("0" + text));
                            }
                        }
                        stringBuilder4.AppendFormat(" {0}", this.encoding.GetString(array));
                        stringBuilder.AppendLine(string.Format("\t*Hex:{1}\t*Decimal:{0}\t*Char:{2}", stringBuilder2, stringBuilder3, stringBuilder4));
                    }
                }
                this.offset = num2;
                result = stringBuilder.ToString();
            }
            return result;
        }
    }
}
