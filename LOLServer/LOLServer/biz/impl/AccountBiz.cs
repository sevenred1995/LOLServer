using LOLServer.cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOLServer.biz.impl {
    /// <summary>
    /// 用户账号具体事务实现类
    /// </summary>
    public class AccountBiz:IAccountBiz {
        IAccountCache accountCache = CacheFactory.accountCache;
        /// <summary>
        /// 0:成功 1:账号重复 2:账号不合法 3:密码不合法
        /// </summary>
        /// <param name="token"></param>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public int creat(NetFrame.UserToken token, string account, string password) {
            if (account.Length < 2)
                return 2;
            if (password.Length < 4)
                return 3;
            if (accountCache.hasAccount(account))
                return 1;//账号存在
            accountCache.add(account, password);//账号添加
            accountCache.online(token, account);
            return 0;//创建成功
        }
        public int login(NetFrame.UserToken token, string account, string password) {
            if (!accountCache.hasAccount(account))
                return 1;//不存在账号
            if (!accountCache.match(account, password))
                return 2;//不匹配
            if (accountCache.isOnline(account))
                return 3;//账号已经在线
            accountCache.online(token, account);//账号上线
            return 0;
        }
        public void close(NetFrame.UserToken token) {
            accountCache.offline(token);
        }
        public int get(NetFrame.UserToken token) {
            return accountCache.getId(token);
        }
    }
}
