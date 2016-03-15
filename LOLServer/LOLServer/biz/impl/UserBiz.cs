using LOLServer.cache;
using LOLServer.dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOLServer.biz.impl {
    //用户事物 实现层
    class UserBiz:IUserBiz {
        IAccountBiz accBiz = BizFactory.accountBiz;
        //IAccountCache accCache = CacheFactory.accountCache;
        IUserCache userCache = CacheFactory.userCache;
        public bool Create(NetFrame.UserToken token, string name) {
            //判断账号是否在线
            int accountId = accBiz.get(token);//如果账号不在线获取到-1
            //int accountId = accCache.getId(token);
            if (accountId == -1)
                return false;
            //判断是否有角色
            if(userCache.hasuserByAccountID(accountId))
                return false;
            return userCache.creat(token, name,accountId);
        }
        public dao.UserModelDao get(NetFrame.UserToken token) {
            return userCache.get(token);
        }
        public UserModelDao getByAccount(NetFrame.UserToken token) {
            int accountId = accBiz.get(token);//获得当前登录账号id
               if (accountId == -1)
                 return null;//如果账号不在线--获取不到角色
                 //判断是否有角色
               if (!userCache.hasuserByAccountID(accountId))
                return null;
               return userCache.get(token,accountId);
               //return userCache.get(token);
        }
        public dao.UserModelDao get(int id) {
            return userCache.get(id);  
        }
        public dao.UserModelDao online(NetFrame.UserToken token) {
            //判断账号是否在线
            int accountId = accBiz.get(token);
            if (accountId == -1)
                return null;
            UserModelDao user = userCache.getByAccountID(accountId);//通过账号ID获取用户
            if (userCache.isOnline(user.id))
                return null;
            return userCache.online(token,user.id);
        }
        public void offline(NetFrame.UserToken token) {
            userCache.offline(token);
        }
        public NetFrame.UserToken getToken(int id) {
            return userCache.getToken(id);
        }
    }
}
