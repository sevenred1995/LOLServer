using GameProtocol;
using GameProtocol.dto;
using LOLServer.biz;
using LOLServer.biz.impl;
using LOLServer.dao;
using LOLServer.tool;
using NetFrame;
using NetFrame.auto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOLServer.Logic.user {
    public class userhandler:AbsOnceHandler,HandlerInterface {
        IUserBiz userBiz = BizFactory.userBiz;
        public override byte Type {
            get {
                return Protocol.TYPE_USER;
            }
        }
        public void ClientConnect(NetFrame.UserToken token) {
            ExcutorPool.Instance.excute(delegate()
            {
            });
        }
        public void MessageReceive(NetFrame.UserToken token, NetFrame.auto.SocketModel message) {
            switch(message.Command)
            {
                case UserProtocol.INFO_CREQ:
                    info(token);
                    break;
                case UserProtocol.CREAT_CREQ:
                    creat(token, message.GetMessage<string>());
                    break;
                case UserProtocol.ONLINE_CREQ:
                    online(token);
                    break;
            }
        }
        public void ClientClose(NetFrame.UserToken token, string error) {
            ExcutorPool.Instance.excute(delegate()
            {
                userBiz.offline(token);//账号下线的同时用户下线
            });
        }
        public void creat(UserToken token, string message) {
            //跳转至单线程执行
            ExcutorPool.Instance.excute(delegate()
            {
               bool result=userBiz.Create(token, message);
               //Console.WriteLine(result);
               write(token, UserProtocol.CREAT_SRES, result);
            });
        }
        public void info(UserToken token) {
            ExcutorPool.Instance.excute(delegate()
            {
                write(token, UserProtocol.INFO_SRES, convert(userBiz.getByAccount(token)));
            });
        }
        public void online(UserToken token) {
            ExcutorPool.Instance.excute(delegate()
            {
                write(token, UserProtocol.ONLINE_SRES,convert(userBiz.online(token)));
            });
        }
        UserModel convert(UserModelDao dao)
        {
            //
            if (dao != null)
            {
                return new UserModel(dao.id, dao.name, dao.level, dao.exp, dao.winCount, dao.loseCount, dao.runCount,dao.heroList.ToArray());
            }
            else
                return null;
        }
    }
}
