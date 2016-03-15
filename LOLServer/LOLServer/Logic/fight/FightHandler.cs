using GameProtocol.dto;
using LOLServer.tool;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOLServer.Logic.fight {
    public class FightHandler:AbsOnceHandler,HandlerInterface {
        /// <summary>
        /// 多线程处理类中，防止数据竞争导致脏数据 使用线程安全字典  用户id与区域码映射
        /// </summary>
        ConcurrentDictionary<int, int> userRoom = new ConcurrentDictionary<int, int>();
        /// <summary>
        /// 房间区域码与模型映射
        /// </summary>
        ConcurrentDictionary<int, FightRoom> roomMap = new ConcurrentDictionary<int, FightRoom>();
        /// <summary>
        /// 回收利用房间对象再次利用
        /// </summary>
        ConcurrentStack<FightRoom> cache = new ConcurrentStack<FightRoom>();
        /// <summary>
        /// 安全自增器
        /// </summary>
        ConcurrentInteger index = new ConcurrentInteger();
        public FightHandler() {
            EventUtil.createFight = create;
            EventUtil.destoryFight = destory;
        }
        public void ClientConnect(NetFrame.UserToken token) {

        }
        void create(SelectModel[] teamOne, SelectModel[] teamTwo) {
            FightRoom room;
            if(!cache.TryPop(out room))
            {
                room = new FightRoom();
                room.SetArea(index.GetAdd());
            }
            //房间数据初始化
            room.init(teamOne, teamTwo);
            //绑定映射关系
            foreach(SelectModel item in teamOne)
            {
                userRoom.TryAdd(item.userID, room.Area);
            }
            foreach(SelectModel item in teamTwo)
            {
                userRoom.TryAdd(item.userID, room.Area);
            }
            roomMap.TryAdd(room.Area, room);
        }
        void destory(int roomID) {
            FightRoom room;
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
                //放入回收器中
                cache.Push(room);
            }
        }

        public void MessageReceive(NetFrame.UserToken token, NetFrame.auto.SocketModel message) {    
            roomMap[userRoom[getUserID(token)]].MessageReceive(token, message);
        }

        public void ClientClose(NetFrame.UserToken token, string error) {
           
            if(userRoom.ContainsKey(getUserID(token)))
            {
                roomMap[userRoom[getUserID(token)]].ClientClose(token, error);
            }

        }
    }
}
