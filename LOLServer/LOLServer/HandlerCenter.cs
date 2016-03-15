using LOLServer.Logic;
using LOLServer.Logic.login;
using LOLServer.Logic.user;
using LOLServer.Logic.match;
using NetFrame;
using NetFrame.auto;
using System;
using System.Collections.Generic;
using System.Text;
using GameProtocol;
using LOLServer.Logic.select;
using LOLServer.Logic.fight;

namespace LOLServer {
    public class HandlerCenter:AbsHandlerCenter {
        HandlerInterface login;
        HandlerInterface user;
        HandlerInterface match;
        HandlerInterface select;
        HandlerInterface fight;
        /// <summary>
        /// 模块的初始化
        /// </summary>
        public HandlerCenter() {
            login = new LoginHandler();
            user = new userhandler();
            match = new MatchHandler();
            select = new SelectHandler();
            fight=new FightHandler();
        }
        /// <summary>
        /// 客户端连接进来进行处理
        /// </summary>
        /// <param name="token"></param>
        public override void ClientConnect(UserToken token) {
            Console.WriteLine("有客户端连接进来"+token.conn.RemoteEndPoint.ToString());
        }
        /// <summary>
        /// 客户端断开的处理
        /// </summary>
        /// <param name="token"></param>
        /// <param name="error"></param>
        public override void ClientClose(UserToken token, string error) {
            Console.WriteLine("客户端断开连接");
            fight.ClientClose(token, error);
            select.ClientClose(token, error);
            match.ClientClose(token,error);
            user.ClientClose(token, error);
            login.ClientClose(token, error);
        }
        /// <summary>
        /// 消息到达进行的处理
        /// </summary>
        /// <param name="token"></param>
        /// <param name="message"></param>
        public override void MessageReceive(UserToken token, object message) {
            //有消息接收
            SocketModel model = message as SocketModel;
            switch(model.Type)
            {
                case GameProtocol.Protocol.TYPE_LOGIN:
                    login.MessageReceive(token, model);
                    break;
                case GameProtocol.Protocol.TYPE_USER:
                    user.MessageReceive(token, model);
                    break;
                case GameProtocol.Protocol.TYPE_MATCH:
                    match.MessageReceive(token, model);
                    break;
                case GameProtocol.Protocol.TYPE_SELECT:
                    select.MessageReceive(token, model);
                    break;
                case GameProtocol.Protocol.TYPE_FIGHT:
                    fight.MessageReceive(token, model);
                    break;
                default:
                    break;
            }
        }
    }
}
