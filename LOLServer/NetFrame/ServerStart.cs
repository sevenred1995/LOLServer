using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NetFrame
{
    /// <summary>
    /// 启动服务器--->监听IP（可选）--->监听端口
    /// </summary>
    public class ServerStart
    {
        private Socket Server;//服务器监听对象
        private int maxClient;//最大客户端连接数
        private UserTokenPool pool;//对象连接池
        private Semaphore acceptClients;//信号量,监听链接线程池
        public LengthEncode lengthEncode;
        public LengthDecode lengthDecode;
        public MsgEncode msgEncode;
        public MsgDecode msgDecode;
        /// <summary>
        /// 消息处理中心，由外部传入
        /// </summary>
        public AbsHandlerCenter center;

        /// <summary>
        /// 初始化通信监听
        /// </summary>
        /// <param name="port"></param>
        public ServerStart(int max) {
            try
            {
                Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                maxClient = max;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
        public void Start(int port) {
            pool = new UserTokenPool(maxClient);
            acceptClients = new Semaphore(maxClient, maxClient);
            for (int i = 0; i < maxClient; i++)
            {
                UserToken token = new UserToken();
                //TODO初始化连接信息
                token.receiveSAEA.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                token.sendSAEA.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                token.lengthDecode = lengthDecode;
                token.lengthEncode = lengthEncode;
                token.msgDecode = msgDecode;
                token.msgEncode = msgEncode;
                token.sendProssess = ProcessSend;
                token.closeProcess = ClientClose;
                token.center = center;
                pool.push(token);
            }
            try
            {
                Server.Bind(new IPEndPoint(IPAddress.Any, port));//监听当前服务器网卡所有可用IP地址和端口
                Server.Listen(10);//置于监听
                StartAccept(null);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        /// <summary>
        /// 开始客户端监听连接
        /// </summary>
        public void StartAccept(SocketAsyncEventArgs e) {
            //如果当前传入为空，说明需要调用新的客户端连接监听事件，否则移除当前
            if (e == null)
            {
                e = new SocketAsyncEventArgs();
                e.Completed += new EventHandler<SocketAsyncEventArgs>(Accept_Completed);
            }
            else
            {
                e.AcceptSocket = null;
            }
            acceptClients.WaitOne();//信号量减1；达到阻塞效果
            bool result = Server.AcceptAsync(e);
            //判断异步事件是否挂起。没挂起说明立刻执行完成 直接处理事件 否则处理完成后触发--Accept_Completed--事件
            //SocketAsyncEventArgs同一时间只能进行一个操作，通过Completed来确认当前操作是否完成，如果同步完成是不会触该事件需要自己手动调用处理。
            if(!result)
            {
                ProcessAccept(e);
            }

        }
        /// <summary>
        /// 接受连接响应事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Accept_Completed(object sender, SocketAsyncEventArgs e) {
            ProcessAccept(e);
        } 
        void ProcessAccept(SocketAsyncEventArgs e) {
            UserToken token = pool.pop();//分配一个链接对象
            token.conn = e.AcceptSocket;
            //TODO通知应用层有客户端连接
            center.ClientConnect(token);
            //开启消息到达监听
            StartReceive(token);
            //释放当前异步对象
            StartAccept(e);//把当前异步事件释放，等待下次连接  
        }
        public void StartReceive(UserToken token) {
            try
            {
                bool result = token.conn.ReceiveAsync(token.receiveSAEA);
                if (!result)
                {
                    lock (token)
                    {
                        ProcessReceive(token.receiveSAEA);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        void IO_Completed(object sender, SocketAsyncEventArgs e) {
           if(e.LastOperation==SocketAsyncOperation.Receive)
           {
               ProcessReceive(e);
           }
           else
           {
               ProcessSend(e);
           }
        }
        /// <summary>
        /// 接收消息处理机制
        /// </summary>
        /// <param name="e"></param>
        public void ProcessReceive(SocketAsyncEventArgs e) {
            UserToken token = e.UserToken as UserToken;
            if (token.receiveSAEA == null)
                return;
            //判断网络接受消息是否成功
            if (token.receiveSAEA.BytesTransferred > 0 && token.receiveSAEA.SocketError == SocketError.Success)
            {
                byte[] message=new byte[token.receiveSAEA.BytesTransferred];
                Buffer.BlockCopy(token.receiveSAEA.Buffer, 0, message, 0, token.receiveSAEA.BytesTransferred);
                //处理接受到的消息
                token.receive(message);
                StartReceive(token);
            }else
            {
                if(token.receiveSAEA.SocketError!=SocketError.Success)
                {
                    ClientClose(token,token.receiveSAEA.SocketError.ToString());
                }
                else
                {
                    ClientClose(token, "客户端主动断开连接");
                }
            }
        }
        /// <summary>
        /// 发送消息处理机制
        /// </summary>
        /// <param name="e"></param>
        public void ProcessSend(SocketAsyncEventArgs e) {
            UserToken token = e.UserToken as UserToken;//获取到连接对象
            if(e.SocketError!=SocketError.Success)
            {
                ClientClose(token, e.SocketError.ToString());
            }
            else
            {
                //消息发送成功，回调成功
                token.writed();
            }
        }
        /// <summary>
        /// 客户端断开连接
        /// </summary>
        /// <param name="token">断开连接的用户对象</param>
        /// <param name="error">断开连接的错误编码</param>
        public void ClientClose(UserToken token,string error) {
            if (token.conn != null)
            {
                lock (token)
                {
                    //通知引用层面 客户端断开连接
                    center.ClientClose(token, error);
                    token.Close();
                    //加回信号量供其它使用
                    pool.push(token);
                    acceptClients.Release();
                }
            }
        }
    }
}
