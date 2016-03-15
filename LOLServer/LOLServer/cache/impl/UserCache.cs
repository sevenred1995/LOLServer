using LOLServer.dao;
using NetFrame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOLServer.cache.impl {
    //保存从数据库读取出来的数据
    class UserCache:IUserCache {
        /// <summary>
        /// 用户ID和模型
        /// </summary>
        private Dictionary<int, UserModelDao> idToModel = new Dictionary<int, UserModelDao>();
        /// <summary>
        /// 账号ID和角色ID之间绑定
        /// </summary>
        private Dictionary<int, int> accTouid = new Dictionary<int, int>();
        /// <summary>
        /// id:用户ID与连接之间的绑定
        /// </summary>
        private Dictionary<UserToken, int> tokenToid = new Dictionary<UserToken, int>();
        private Dictionary<int, UserToken> idTotoken = new Dictionary<int, UserToken>();
        int index=0;
        public bool creat(NetFrame.UserToken token, string name,int accountID) {
            UserModelDao user = new UserModelDao();
            user.id = index;
            index++;
            user.name = name;
            user.accountID = accountID;
            //向缓存中添加
            accTouid.Add(accountID,user.id);
            idToModel.Add(user.id, user);

            for (int i = 0; i < 10;i++)
            {
                user.heroList.Add(i + 1);
            }

                //继续向数据库中添加
                //TODO
                return true;
        }
        public bool hasuser(NetFrame.UserToken token) {
            return tokenToid.ContainsKey(token);
        }
        /// <summary>
        /// 根据账号id判断是否有对应的角色id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool hasuserByAccountID(int id) {
            return accTouid.ContainsKey(id);//账号ID与用户ID之间的对应
        }
        public bool has(UserToken token) {
            return tokenToid.ContainsKey(token);
        }

        public UserModelDao get(UserToken token) {
            if (!has(token))
                return null;
            return idToModel[tokenToid[token]];
        }
        public dao.UserModelDao get(NetFrame.UserToken token,int accountID) {
            if (!hasuserByAccountID(accountID))
                return null;
            return idToModel[accTouid[accountID]];
        }
        public dao.UserModelDao online(NetFrame.UserToken token,int userID) {
            if(!idTotoken.ContainsKey(userID))
            {
                idTotoken.Add(userID, token);
                tokenToid.Add(token, userID);
            }
            return idToModel[userID];
        }
        public void offline(NetFrame.UserToken token) {
            if(tokenToid.ContainsKey(token))//添加判断防止未登录退出发生异常
            {
                idTotoken.Remove(tokenToid[token]);
                tokenToid.Remove(token); 
            }
            
        }
        public NetFrame.UserToken getToken(int id) {
            return idTotoken[id];//通过用户id获取当前的连接
        }
        public dao.UserModelDao get(int id) {//id为用户id
            if(idToModel.ContainsKey(id))
                return idToModel[id];
            return null;//获取不成功
        }
        public dao.UserModelDao getByAccountID(int accountID) {
            //是否有与账号ID对应的用户ID
            if (accTouid.ContainsKey(accountID))
            {
                return get(accTouid[accountID]);
            }
            return null;
        }
        public bool isOnline(int userid) {
            if (idTotoken.ContainsKey(userid))
                return false;
            return idTotoken.ContainsKey(userid);
        }



    }
}
