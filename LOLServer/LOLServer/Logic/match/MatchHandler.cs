using GameProtocol;
using GameProtocol.dto;
using LOLServer.tool;
using NetFrame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace LOLServer.Logic.match {
    public class MatchHandler:AbsOnceHandler,HandlerInterface {
        /// <summary>
        /// 多线程处理类中，防止数据竞争导致脏数据 使用线程安全字典
        /// </summary>
        ConcurrentDictionary<int, int> userRoom = new ConcurrentDictionary<int, int>();
        /// <summary>
        /// 房间id与模型映射
        /// </summary>
        ConcurrentDictionary<int, MatchRoom> roomMap = new ConcurrentDictionary<int, MatchRoom>();
        /// <summary>
        /// 回收利用房间对象再次利用
        /// </summary>
        ConcurrentStack<MatchRoom> cache = new ConcurrentStack<MatchRoom>();

        ConcurrentInteger index =new ConcurrentInteger();
        public override byte Type {
            get {
                return Protocol.TYPE_MATCH;
            }
        } 
        public void ClientConnect(NetFrame.UserToken token) {
         
        }
        public void ClientClose(NetFrame.UserToken token, string error) {
            leave(token);
        }

        public void MessageReceive(NetFrame.UserToken token, NetFrame.auto.SocketModel message) {
             switch(message.Command)
             {
                 case MatchProtocol.ENTER_CREQ:
                     //Console.WriteLine("角色发来匹配请求...");
                     enter(token);
                     break;
                 case MatchProtocol.LEAVE_CREQ:
                     //Console.WriteLine("申请取消匹配...");
                     leave(token);
                     break;
             }
        }

        private void leave(UserToken token) {
            int userId = getUserID(token);
            //Console.WriteLine("客户端取消匹配##"+userId);
            if(!userRoom.ContainsKey(userId))
            {
                return;
            }
            int roomID;
            userRoom.TryGetValue(userId, out roomID);
            if(roomMap.ContainsKey(roomID))
            {
                MatchRoom room = roomMap[roomID];
                if (room.teamOne.Contains(userId))
                    room.teamOne.Remove(userId);
                if (room.teamTwo.Contains(userId))
                    room.teamTwo.Remove(userId);
                userRoom.TryRemove(userId, out roomID);
                if(room.teamOne.Count+room.teamTwo.Count==0)
                {
                    roomMap.TryRemove(roomID, out room);
                    cache.Push(room);
                    Console.WriteLine("客户取消匹配成功，房间解散" + userId);
                }
            }
            //todo通知客户端退出匹配成功
            write(token, MatchProtocol.LEAVE_SRES, null);
        }
        /// <summary>
        /// 进入房间逻辑
        /// </summary>
        /// <param name="token"></param>
        private void enter(UserToken token) {
            int userId = getUserID(token);
            //Console.WriteLine("客户端开始匹配"+userId);
            //判断玩家是否在匹配队伍中
            if(!userRoom.ContainsKey(userId))//玩家没有在队伍内
            {
                MatchRoom room = null;
                //判断队伍等待队列中是否有匹配队列
                if(roomMap.Count>0)
                {
                    //Console.WriteLine("存在匹配房间...");
                    foreach(MatchRoom item in roomMap.Values)//遍历所有的匹配房间
                    {
                        //匹配队伍人员未满
                        if(item.teamMax*2>item.teamOne.Count+item.teamTwo.Count)
                        {
                            room = item;
                            if(room.teamOne.Count<room.teamMax)
                            {
                                room.teamOne.Add(userId);
                            }
                            else if(room.teamTwo.Count<room.teamMax)
                            {
                                room.teamTwo.Add(userId);
                                //Console.WriteLine("加入队伍2....");
                            }
                            //匹配到队伍
                            userRoom.TryAdd(userId, room.id);
                            break;
                        }
                    }
                    //等待房间已经满员
                    if (!userRoom.ContainsKey(userId))
                    {
                        if (cache.Count > 0)
                        {
                            cache.TryPop(out room);
                            room.teamOne.Add(userId);
                            userRoom.TryAdd(userId, room.id);
                            roomMap.TryAdd(room.id, room);
                        }
                        else
                        {
                            //Console.WriteLine("客户端创建房间" + userId);
                            room = new MatchRoom();
                            room.id = index.GetAdd();
                            room.teamOne.Add(userId);
                            userRoom.TryAdd(userId, room.id);
                            roomMap.TryAdd(room.id, room);
                        }
                    }
                    
                }
                else
                {
                    //等待中的房间已经全部满员
                    if (cache.Count > 0)
                    {
                        cache.TryPop(out room);
                        room.teamOne.Add(userId);
                        userRoom.TryAdd(userId, room.id);
                        roomMap.TryAdd(room.id, room);
                    }
                    else
                    {
                        //Console.WriteLine("客户端创建房间" + userId);
                        room = new MatchRoom();
                        room.id = index.GetAdd();
                        room.teamOne.Add(userId);
                        userRoom.TryAdd(userId, room.id);
                        roomMap.TryAdd(room.id, room);
                    }
                }
                
                //进入房间成功 判断房间是否满员 满员则将房间丢进缓存队列
                if(room.teamOne.Count==room.teamMax&&room.teamTwo.Count==room.teamMax)
                {
                    //Console.WriteLine("匹配队伍满员...."+getUserName(room.teamOne[0])+" : "+getUserName(room.teamTwo[0]));
                    //匹配队伍满员后  在服务器端对选择房间进行初始化
                    EventUtil.createSelect(room.teamOne, room.teamTwo);
                    //通知队伍类所有玩家进行选人
                    Console.WriteLine("可以进入选人界面..");
                    writeToUsers(room.teamOne.ToArray(), Type, 0, MatchProtocol.ENTER_SELECT_BRO, null);
                    writeToUsers(room.teamTwo.ToArray(), Type, 0, MatchProtocol.ENTER_SELECT_BRO, null);
                    
                    //移除玩家与房间的映射
                    foreach(int item in room.teamOne)
                    {
                        int id;
                        userRoom.TryRemove(item, out id);
                    }
                    foreach (int item in room.teamTwo)
                    {
                        int id;
                        userRoom.TryRemove(item, out id);
                    }
                    room.teamOne.Clear();
                    room.teamTwo.Clear();
                    roomMap.TryRemove(room.id, out room);
                    cache.Push(room);
                }
            }
        }
    }
}
