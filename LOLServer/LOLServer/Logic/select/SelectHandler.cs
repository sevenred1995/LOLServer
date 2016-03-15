using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LOLServer.tool;

namespace LOLServer.Logic.select {
    public class SelectHandler:AbsOnceHandler,HandlerInterface {
        /// <summary>
        /// 多线程处理类中，防止数据竞争导致脏数据 使用线程安全字典  用户id与区域码映射
        /// </summary>
        ConcurrentDictionary<int, int> userRoom = new ConcurrentDictionary<int, int>();
        /// <summary>
        /// 房间区域码与模型映射
        /// </summary>
        ConcurrentDictionary<int, SelectRoom> roomMap = new ConcurrentDictionary<int, SelectRoom>();
        /// <summary>
        /// 回收利用房间对象再次利用
        /// </summary>
        ConcurrentStack<SelectRoom> cache = new ConcurrentStack<SelectRoom>();

        ConcurrentInteger index = new ConcurrentInteger();
        public SelectHandler() {
            EventUtil.createSelect = create;
            EventUtil.destorySelect = destory;
        }
        public void create(List<int> teamOne, List<int> teamTwo) {
            SelectRoom room;
            //如果缓存中没有房间对象
            if(!cache.TryPop(out room))
            {
                room = new SelectRoom();
                //添加唯一标识码
                room.SetArea(index.GetAdd());
            }
            //内部初始化房间
            room.init(teamOne, teamTwo);
            //item 队伍中角色id号
            foreach(int item in teamOne)
            {
                userRoom.TryAdd(item, room.Area);
            }
            foreach(int item in teamTwo)
            {
                userRoom.TryAdd(item, room.Area);
            }
            roomMap.TryAdd(room.Area, room);
        }
        public void destory(int roomID) {
            SelectRoom room;
            if(roomMap.TryRemove(roomID,out room))
            {
                //移除角色和房间之间的绑定关系
                int temp = 0;
                foreach(int item in room.teamOne.Keys)
                {
                    userRoom.TryRemove(item, out temp);
                }
                foreach(int item in room.teamTwo.Keys)
                {
                    userRoom.TryRemove(item, out temp);
                }
                room.list.Clear();
                room.teamOne.Clear();
                room.teamTwo.Clear();
                room.readyList.Clear();
                //将房间丢入缓存
                cache.Push(room);
            }
        }
        public void ClientConnect(NetFrame.UserToken token) {
             
        }

        public void MessageReceive(NetFrame.UserToken token, NetFrame.auto.SocketModel message) {
            //if(roomMap.ContainsKey(message.Area))
            //{
            //    roomMap[message.Area].MessageReceive(token, message);
            //}
            int userID = getUserID(token);
            if (userRoom.ContainsKey(userID))
            {
                int roomID = userRoom[userID];
                if (roomMap.ContainsKey(roomID))
                {
                    roomMap[roomID].MessageReceive(token, message);
                }
            }
        }

        public void ClientClose(NetFrame.UserToken token, string error) {
            int userID = getUserID(token);
            //该用户在房间里
            if(userRoom.ContainsKey(userID))
            {
                int roomID;
                userRoom.TryRemove(userID, out roomID);
                if(roomMap.ContainsKey(roomID))
                {
                    roomMap[roomID].ClientClose(token, error);
                }
            }
        }
    }
}
