using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ZLX.Framework.ToolKit.BytesHelper;
using ZLX.Framework.ToolKit.Compress;
using ZLX.Framework.ToolKit.Encrypt;

namespace ZLX.Framework.CP.Message
{
    public static class MessageHelper
    {
        public static byte[] Serialize<T>(this MessageInfo<T> info,string encryptKey = "ZLX.STAR")
        {
            byte[] data = null;

            data = BytesMaker.Combine(data, BytesMaker.GetEncodingBytesWithLength(info.Id, null));

            data = BytesMaker.Combine(data, BytesMaker.GetIntBytes(info.MessageType));
            data = BytesMaker.Combine(data, BytesMaker.GetIntBytes((int)info.TransferType));

            data = BytesMaker.Combine(data, BytesMaker.GetBoolBytes(info.IsCompress));

            data = BytesMaker.Combine(data, BytesMaker.GetBoolBytes(info.IsEncrypt));

            byte[] bodyData = new byte[0];
            switch (info.TransferType)
            {
                case TransferType.Custom:
                    string infoString = Newtonsoft.Json.JsonConvert.SerializeObject(info.ContentArea);
                    bodyData = BytesMaker.GetEncodingBytesWithLength(infoString, null);
                    bodyData = BytesMaker.Combine(bodyData, BytesMaker.AppendHeaderToBytes(info.FileArea));
                    break;
                case TransferType.ProtocolBuf:
                    MemoryStream stream = new MemoryStream();
                    ProtoBuf.Serializer.Serialize(stream, info);
                    byte[] buf = stream.ToArray();
                    //bodyData = buf;
                    bodyData = BytesMaker.Combine(BytesMaker.AppendHeaderToBytes(buf), BytesMaker.AppendHeaderToBytes(info.FileArea));
                    break;
                default:
                    break;
            }

            if (info.IsEncrypt)
            {
                bodyData = EncryptUtils.DES3Encrypt(bodyData, encryptKey);
            }
            if (info.IsCompress)
            {
                bodyData = CompressHelper.Compress(bodyData);
            }
            data = BytesMaker.Combine(data, BytesMaker.AppendHeaderToBytes(bodyData));

            data = BytesMaker.AppendHeaderToBytes(data);

            return data;

        }

        public static MessageInfo<T> Deserialize<T>(this byte[] data, string encryptKey = "ZLX.STAR")
        {
            MessageInfo<T> info = new MessageInfo<T>();
            BytesReader reader = new BytesReader(data);
            int totalLength = reader.ReadInt();
            info.Id = reader.ReadString();
            info.MessageType = reader.ReadInt();
            info.TransferType = (TransferType)reader.ReadInt();

            info.IsCompress = reader.ReadBoolean();
            info.IsEncrypt = reader.ReadBoolean();
            byte[] bodyData = reader.ReadBytesWithHeader();

            if (info.IsCompress)
            {
                bodyData = CompressHelper.Decompress(bodyData);
            }
            if (info.IsEncrypt)
            {
                bodyData = EncryptUtils.DES3Decrypt(bodyData, encryptKey);
            }

            BytesReader bodyReader = new BytesReader(bodyData);

            switch (info.TransferType)
            {
                case TransferType.Custom:
                    string infoString = bodyReader.ReadString();
                    info.ContentArea = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(infoString);
                    info.FileArea = bodyReader.ReadBytesWithHeader();
                    break;
                case TransferType.ProtocolBuf:
                    byte[] buf = bodyReader.ReadBytesWithHeader();
                    MemoryStream stream = new MemoryStream(buf);
                    info.ContentArea = ProtoBuf.Serializer.Deserialize<T>(stream);
                    info.FileArea = bodyReader.ReadBytesWithHeader();
                    break;
                default:
                    break;
            }


            return info;

        }
    }
}
