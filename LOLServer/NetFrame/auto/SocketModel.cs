using System;
using System.Collections.Generic;

using System.Text;


namespace NetFrame.auto {
    public class SocketModel {
        public byte Type { get; set; }//消息类型---消息所属模块（OperationCode)
        public int Area { get; set; }//所属区域码---子模块(SubCode)
        public int Command { get; set; }//内部协议--区分逻辑处理功能(指令)
        public object Message { get; set; }//消息体
        public SocketModel() {
        }
        public SocketModel(byte type, int area, int command, object message) {
            this.Type = type;
            this.Area = area;
            this.Command = command;
            this.Message = message;
        }
        public T GetMessage<T>() {
            return (T)Message;
        }
    }
}
