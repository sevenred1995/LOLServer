using System;
using System.Collections.Generic;
using System.Text;

namespace GameProtocol {
    /// <summary>
    /// 网络一级模块协议
    /// </summary>
    public class Protocol {
        /// <summary>
        /// 登陆模块
        /// </summary>
        public const byte TYPE_LOGIN = 0;//登陆模块
        /// <summary>
        /// 用户模块
        /// </summary>
        public const byte TYPE_USER = 1;//用户模块
        /// <summary>
        /// 战斗匹配模块
        /// </summary>
        public const byte TYPE_MATCH = 2;
        /// <summary>
        /// 选择模块
        /// </summary>
        public const byte TYPE_SELECT = 3;
        /// <summary>
        /// 战斗模块
        /// </summary>
        public const byte TYPE_FIGHT = 4;
    }
}
