using System;
using System.Collections.Generic;
using System.Text;

namespace GameProtocol {
    /// <summary>
    /// 网络二级协议
    /// </summary>
    public class LoginProtocol {
        public const int LOGIN_CREQ = 0;//客户端申请登陆
        public const int LOGIN_SRES = 1;//服务器返回登陆结果

        public const int REG_CREQ = 2;
        public const int REG_SRES = 3;
    }
}
