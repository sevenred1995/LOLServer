using NetFrame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOLServer.biz{
    public interface IAccountBiz {
        /// <summary>
        /// 账号创建
        /// </summary>
        /// <param name="token"></param>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns>Returncode{0:成功 1:账号重复 2:账号不合法 3:密码不合法} </returns>
        int creat(UserToken token, string account, string password);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns>logins returncode {0:登陆成功：1:不存在账号,2:密码错误,3:账号已经登陆 }</returns>
        int login(UserToken token, string account, string password);
        /// <summary>
        /// 客户端断开--失去连接
        /// </summary>
        /// <param name="token"></param>
        void close(UserToken token);
        /// <summary>
        /// 获取用户的id
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        int get(UserToken token);
    }
}
