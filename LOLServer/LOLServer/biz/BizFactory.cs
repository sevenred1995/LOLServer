using System;
using System.Collections.Generic;
using System.Text;

namespace LOLServer.biz {
    public class BizFactory {
        public readonly static IAccountBiz accountBiz;
        public readonly static IUserBiz userBiz;
        static BizFactory() {
            accountBiz = Activator.CreateInstance(Type.GetType("LOLServer.biz.impl.AccountBiz")) as IAccountBiz;
            userBiz = Activator.CreateInstance(Type.GetType("LOLServer.biz.impl.UserBiz")) as IUserBiz;
            //accountBiz = new AccountBiz();
            //userBiz=new UserBiz();
        }
    }
}
