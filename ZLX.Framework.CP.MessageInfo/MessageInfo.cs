using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZLX.Framework.CP.Message
{
    public class MessageInfo<T>
    {
        /// <summary>
        /// 默认0为心跳，1为消息
        /// </summary>
        public int MessageType { get; set; }

        /// <summary>
        /// 传输类型
        /// </summary>
        public TransferType TransferType { get; set; }

        /// <summary>
        /// 是否压缩
        /// </summary>
        public bool IsCompress { get; set; }

        /// <summary>
        /// 是否加密,是则采用3ds加密
        /// </summary>
        public bool IsEncrypt { get; set; }

        /// <summary>
        /// 加密key
        /// </summary>
        //public string EncryptKey { get; set; }

        /// <summary>
        /// 消息实体
        /// </summary>
        public T ContentArea { get; set; }

        /// <summary>
        /// 附带信息
        /// </summary>
        public byte[] FileArea { get; set; }





    }
}
