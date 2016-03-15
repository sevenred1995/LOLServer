using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetFrame;
using NetFrame.auto;
namespace LOLServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerStart ss = new ServerStart(9000);
            ss.lengthEncode = LengthEncoding.encode;
            ss.msgEncode = MessageEncoding.Encode;
            ss.center = new HandlerCenter();
            ss.lengthDecode = LengthEncoding.decode;
            ss.msgDecode = MessageEncoding.Decode;
            ss.Start(6500);
            Console.WriteLine("服务器启动>>>>");
            while (true)
            {

            }
        }
    }
}
