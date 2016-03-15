using NetFrame;
using NetFrame.auto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOLServer.Logic {
    /// <summary>
    /// 广播功能，同时继承单发模块
    /// </summary>
    public class AbsMulitHandler:AbsOnceHandler {
        public List<UserToken> list = new List<UserToken>();
        /// <summary>
        /// 用户进入当前子模块
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool enter(UserToken token) {
            if(list.Contains(token))
            {
                return false;
            }
            list.Add(token);
            return true;
        }
        /// <summary>
        /// 用户是否在此模块
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool isEntered(UserToken token) {
            return list.Contains(token);
        }
        /// <summary>
        /// 用户离开当前子模块
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool leave(UserToken token) {
            if(list.Contains(token))
            {
                list.Remove(token);
                return true;
            }
            return false;
        }
        #region 消息群发API
        public void brocast(int command, object message,UserToken exToken=null) {
            brocast(Area, command, message,exToken);
        }
        public void brocast(int area, int command, object message, UserToken exToken = null) {
            brocast(Type, area, command, message, exToken);
        }
        public void brocast(byte type, int area, int command, object message, UserToken exToken = null) {
            SocketModel model = CreateModel(type, area, command, message);
            byte[] value = MessageEncoding.Encode(model);
            value = LengthEncoding.encode(value);
            foreach(UserToken item in list)
            {
                if(exToken!=item)
                {
                    byte[] bs = new byte[value.Length];
                    Array.Copy(value, 0, bs, 0, value.Length);
                    item.write(bs);
                }
            }
        }
        #endregion
    }
}
