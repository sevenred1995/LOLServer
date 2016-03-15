using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetFrame;
using LOLServer.dao;

namespace LOLServer.cache.impl {
    /// <summary>
    /// 账号缓存层的设计
    /// </summary>
    public class AccountCache:IAccountCache {
        public int index = 0;
        /// <summary>
        /// 用户连接与账号之间的映射绑定
        /// </summary>
        Dictionary<UserToken, string> onlineAccMap = new Dictionary<UserToken, string>();
        /// <summary>
        /// 账号与自身具体属性的映射绑定
        /// </summary>
        Dictionary<string, AccountModelDao> accMap = new Dictionary<string, AccountModelDao>();

        public bool hasAccount(string account) {
            return accMap.ContainsKey(account);
        }
        public bool match(string account, string password) {
            if (!hasAccount(account))
                return false;
            return accMap[account].password.Equals(password);
        }
        public bool isOnline(string account) {
            return onlineAccMap.ContainsValue(account);
        }

        public int getId(NetFrame.UserToken token) {
            if (!onlineAccMap.ContainsKey(token))//判断是否存在连接记录
                return -1;//账号不再线，无法获取账号id
            return accMap[onlineAccMap[token]].id;
        }

        public void online(NetFrame.UserToken token, string account) {
            onlineAccMap.Add(token, account);
        }

        public void offline(NetFrame.UserToken token) {
            onlineAccMap.Remove(token);
        }
        public void add(string account, string password) {
            AccountModelDao model = new AccountModelDao();
            model.account = account;
            model.password = password;
            model.id = index;
            accMap.Add(account, model);
            index++;
        }
    } 
}
