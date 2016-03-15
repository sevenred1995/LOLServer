using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace NetFrame
{
    /// <summary>
    /// 连接对象
    /// </summary>
    public class UserToken
    {
        /// <summary>
        /// 用户连接
        /// </summary>
        public Socket conn;
        /// <summary>
        /// 用户异步接收网络数据对象
        /// </summary>
        public SocketAsyncEventArgs receiveSAEA;
        /// <summary>
        /// 用户异步发送网络数据对象
        /// </summary>
        public SocketAsyncEventArgs sendSAEA;
        List<byte> cache = new List<byte>();
        public LengthEncode lengthEncode;
        public LengthDecode lengthDecode;
        public MsgEncode msgEncode;
        public MsgDecode msgDecode;
        public delegate void SendProssess(SocketAsyncEventArgs e);
        public SendProssess sendProssess;
        public delegate void CloseProcess(UserToken token,string error);
        public CloseProcess closeProcess;
        public AbsHandlerCenter center;

        private bool isReading=false;
        public bool isWriting = false;

        Queue<byte[]> writeQueue = new Queue<byte[]>();
        public UserToken() {
            sendSAEA = new SocketAsyncEventArgs();
            receiveSAEA = new SocketAsyncEventArgs();
            receiveSAEA.UserToken = this;
            sendSAEA.UserToken = this;
            receiveSAEA.SetBuffer(new byte[1024], 0, 1024);
        }
        public void receive(byte[] buff) {
            cache.AddRange(buff);//将消息写入缓存
            if(!isReading)
            {
                isReading = true;
                OnData();
            }
        }
        /// <summary>
        /// 缓存中有数据处理
        /// </summary>
        void OnData() {
            //解码消息存储对象
            byte[] buff = null;
            if(lengthDecode!=null)
            {
                buff = lengthDecode(ref cache);
                //消息未接收全 退出数据处理 等待下次消息到达
                if (buff == null)
                {
                    isReading = false;
                    return;
                }
            }
            else
            {
                //缓存区中没有数据 直接跳出数据处理 等待下次消息到达
                if (cache.Count == 0)
                {
                    isReading = false;
                    return;
                }
            }
            if (msgDecode == null) { throw new Exception("message decode process is null"); }
            object message = msgDecode(buff);
            //TODO通知应用层有消息到达
            center.MessageReceive(this, message);
            //伪递归 防止在消息处理过程中有其它消息到达没有经过处理
            OnData();
        }

        public void write(byte[] value) {

            if (conn == null)
            {
                closeProcess(this,"调用已经断开");
                return;//连接断开
            }
            writeQueue.Enqueue(value);//向队列添加消息
            if (!isWriting)
            {
                isWriting = true;
                OnWrite();
            }
        }
        public void OnWrite() {
            //判断发送消息队列是否有消息
            if(writeQueue.Count==0)
            {
                isWriting = false;
                return;
            }
            //取出第一条待发消息
            byte[] buff = writeQueue.Dequeue();
            //设置消息发送异步对象的发送数据缓冲区的数据
            sendSAEA.SetBuffer(buff, 0, buff.Length);
            //开启异步发送
            bool result = conn.SendAsync(sendSAEA);
            //是否挂起
            if (!result)
            {
                sendProssess(sendSAEA);
            }
        }
        public void writed() {
            //伪递归
            OnWrite();
        }
        public void Close() {
            try
            {
                writeQueue.Clear();
                cache.Clear();
                isReading = false;
                isWriting = false;
                conn.Shutdown(SocketShutdown.Both);
                conn.Close();
                conn = null;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
