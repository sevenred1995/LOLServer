using System;
using System.Collections.Generic;
using System.Text;


namespace NetFrame.auto {
    public class MessageEncoding {
        /// <summary>
        /// 消息体序列化
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] Encode(object value) {
            SocketModel model = value as SocketModel;
            ByteArray ba = new ByteArray();
            ba.write(model.Type);
            ba.write(model.Area);
            ba.write(model.Command);
            if (model.Message != null)
            {
                ba.write(SerializeUtil.encode(model.Message));
            }
            byte[] result = ba.getBuff(); 
            ba.Close();
            return result;
        }
        /// <summary>
        /// 消息体反序列化
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object Decode(byte[] value) {
            ByteArray ba = new ByteArray(value);
            SocketModel model = new SocketModel();
            byte type;
            int area;
            int command;
            ba.read(out type);
            ba.read(out area);
            ba.read(out command);
            model.Type = type;
            model.Area = area;
            model.Command = command;
            if (ba.Readnable)
            {
                byte[] message;
                ba.read(out message,ba.Length-ba.Postion);
                model.Message = SerializeUtil.decode(message);
            }
            ba.Close();
            return model;
        }
    }
}
