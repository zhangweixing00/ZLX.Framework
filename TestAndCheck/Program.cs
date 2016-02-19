using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestAndCheck
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] aa = ZLX.Framework.CP.Message.MessageHelper.Serialize<M_S>(
                 new ZLX.Framework.CP.Message.MessageInfo<M_S>()
                 {
                     ContentArea = new M_S{ Id = 123, Name = "youhaoa" },
                     FileArea = new byte[] { 2, 1 },
                     MessageType = 0,
                     TransferType = ZLX.Framework.CP.Message.TransferType.Custom
                     ,IsCompress=true
                     ,
                     IsEncrypt=true
                 });
            Console.WriteLine(aa.Length);

            ZLX.Framework.CP.Message.MessageInfo<M_S> cc = ZLX.Framework.CP.Message.MessageHelper.Deserialize<M_S>(aa);
            Console.WriteLine(cc.Id);

            Console.WriteLine(cc.MessageType);
            Console.WriteLine(cc.TransferType);
            Console.WriteLine(cc.ContentArea.Name);
            Console.WriteLine(cc.FileArea[0]);
        }
        class M_S
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
