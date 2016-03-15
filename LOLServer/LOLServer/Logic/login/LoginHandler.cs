using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetFrame;
using NetFrame.auto;
using GameProtocol;
using GameProtocol.dto;
using LOLServer.tool;
using LOLServer.biz;
namespace LOLServer.Logic.login {
    public class LoginHandler:AbsOnceHandler,HandlerInterface {
        IAccountBiz accountBiz = BizFactory.accountBiz;//调用事物层

        public override byte Type {
            get {
                return Protocol.TYPE_LOGIN;
            }
        }
        public void ClientConnect(NetFrame.UserToken token) {
            ExcutorPool.Instance.excute(delegate()
            {

            });
        }
        public void ClientClose(NetFrame.UserToken token, string error) {
            ExcutorPool.Instance.excute(delegate()
            {
                accountBiz.close(token);
            });
        }
        public void MessageReceive(NetFrame.UserToken token, SocketModel message) {
            switch(message.Command)
            {
                case LoginProtocol.LOGIN_CREQ:
                    Login(token,message.GetMessage<AccountModel>());
                    break;
                case LoginProtocol.REG_CREQ:
                    Reg(token, message.GetMessage<AccountModel>());
                    break;
            }
        }
        //在一个单独的线程里处理逻辑
        void Login(UserToken token,AccountModel value) {
            ExcutorPool.Instance.excute(delegate()
            {
               int result=accountBiz.login(token, value.account, value.password);
               write(token, LoginProtocol.LOGIN_SRES, result);
            });
        }
        void Reg(UserToken token, AccountModel value) {
            ExcutorPool.Instance.excute(delegate()
            {
                int result = accountBiz.creat(token, value.account, value.password);
                write(token, LoginProtocol.REG_SRES, result);
            });
        }
    }
}
