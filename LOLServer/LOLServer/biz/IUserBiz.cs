using LOLServer.dao;
using NetFrame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOLServer.biz {
    public interface IUserBiz {
        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="token"></param>
        /// <param name="accout"></param>
        /// <returns></returns>
        bool Create(UserToken token, string name);
        /// <summary>
        /// 通过连接对象获取连接对应的用户信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        UserModelDao get(UserToken token);
        /// <summary>
        /// 根据id获取用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        UserModelDao get(int id);
        /// <summary>
        /// 用户上线
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        UserModelDao online(UserToken token);
        /// <summary>
        /// 用户下线
        /// </summary>
        /// <param name="token"></param>
        void offline(UserToken token);
        /// <summary>
        /// 通过id获取连接
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        UserToken getToken(int id);
        /// <summary>
        /// 通过账号的连接对象获取，仅在初始登陆验证角色
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        UserModelDao getByAccount(UserToken token);
    }
}
