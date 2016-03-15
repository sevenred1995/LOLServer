using LOLServer.dao;
using NetFrame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOLServer.cache {
    public interface IUserCache {
        bool creat(UserToken token,string name,int accountId);
        bool hasuser(UserToken token);
        /// <summary>
        /// 通过账号ID判断是否有角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool hasuserByAccountID(int id);
        bool has(UserToken token);
        UserModelDao get(UserToken token,int accountID);
        UserModelDao get(UserToken token);
        UserModelDao get(int id);//这个接口是为游戏中设计
        /// <summary>
        /// 用户上线----即进入游戏
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        UserModelDao online(UserToken token,int userID);
        /// <summary>
        /// 用户下线---退出游戏
        /// </summary>
        /// <param name="token"></param>
        void offline(UserToken token);
        /// <summary>
        /// 通过用户id获取连接
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        UserToken getToken(int id);//为游戏中设计
        /// <summary>
        /// 通过账号ID获取用户信息
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns></returns>
        UserModelDao getByAccountID(int accountID);
        /// <summary>
        /// 角色是否在线
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        bool isOnline(int userid);
    }
}
