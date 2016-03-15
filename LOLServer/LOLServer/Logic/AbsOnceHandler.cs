using LOLServer.biz;
using LOLServer.dao;
using NetFrame;
using NetFrame.auto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOLServer.Logic {
    public class AbsOnceHandler {
        private byte type;
        private int area;

        public IUserBiz userBiz = BizFactory.userBiz;
        public virtual byte Type {
            get { return type; }
        }
        public virtual int Area {
            get { return area; }
        }
        public void SetType(byte value) {
            type = value;
        }
        public void SetArea(int value) {
            area = value;
        }
        public UserModelDao getUser(UserToken token) {
            return userBiz.get(token);
        }
        public int getUserID(UserToken token) {
            if(getUser(token)!=null)
               return getUser(token).id;
            return -1;
        }
        public string getUserName(int id) {
            return userBiz.get(id).name;
        }
        public UserToken getToken(int id) {
            return userBiz.getToken(id);
        }
        #region 通过连接对象发送
        public void write(UserToken token,int command) {
            write(token, command, null);
        }
        public void write(UserToken token, int command, object message) {
            write(token, Area,command, message);
        }
        public void write(UserToken token, int area, int command, object message) {
            write(token, Type, Area, command, message);
        }
        public void write(UserToken token, byte type, int area, int command, object message) {
            SocketModel model = CreateModel(type, area, command, message);
            byte[] value = MessageEncoding.Encode(model);
            value = LengthEncoding.encode(value);
            token.write(value);
        }
       #endregion

        #region ID发送
        public void write(int id, int command) {
            write(id, command, null);
        }
        public void write(int id, int command, object message) {
            write(id, Area, command, message);
        }
        public void write(int id, int area, int command, object message) {
            write(id, Type, area, message);
        }
        public void write(int id, byte type, int area, int command, object message) {
            UserToken token=getToken(id);
            if(token==null)
                return;
            write(token, type, area, command, message);
        }
        public void writeToUsers(int[] users, byte type, int area, int command, object message) {
            SocketModel model = CreateModel(type, area, command, message);
            byte[] value = MessageEncoding.Encode(model);
            value = LengthEncoding.encode(value);
            foreach (int item in users)
            {
                UserToken token = userBiz.getToken(item);
                if (token == null)
                    continue;
                byte[] bs = new byte[value.Length];
                Array.Copy(value, 0, bs, 0, value.Length);
                token.write(bs);
            }
        }
        #endregion
        public SocketModel CreateModel(byte type, int area, int command, object message){
            return new SocketModel(type,area,command,message);
        }
    }
}
