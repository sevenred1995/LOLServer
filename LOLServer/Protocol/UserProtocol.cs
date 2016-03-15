using System;
using System.Collections.Generic;
using System.Text;

namespace GameProtocol {
    public class UserProtocol {
        /// <summary>
        /// 用户信息获取
        /// </summary>
        public const int INFO_CREQ = 0;
        public const int INFO_SRES = 1;

        /// <summary>
        /// 创建角色请求
        /// </summary>
        public const int CREAT_CREQ = 2;
        public const int CREAT_SRES = 3;

        /// <summary>
        /// 上线获取结果
        /// </summary>
        public const int ONLINE_CREQ = 4;
        public const int ONLINE_SRES = 5;
    }
}
