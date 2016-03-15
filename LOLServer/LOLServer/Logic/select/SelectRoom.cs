using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameProtocol;
using GameProtocol.dto;
using LOLServer.tool;
using NetFrame;
using LOLServer.dao;

namespace LOLServer.Logic.select {
    public class SelectRoom :AbsMulitHandler, HandlerInterface {
        public override byte Type {
            get {
                return Protocol.TYPE_SELECT;
            }
        }
        /// <summary>
        /// 队伍角色ID和选择模型之间的映射
        /// </summary>
        public ConcurrentDictionary<int, SelectModel> teamOne = new ConcurrentDictionary<int, SelectModel>();
        public ConcurrentDictionary<int, SelectModel> teamTwo = new ConcurrentDictionary<int, SelectModel>();
        public List<int> readyList = new List<int>();
        /// <summary>
        /// 当前进入人物数量
        /// </summary>
        int enterCount = 0;
        /// <summary>
        /// 计时任务ID
        /// </summary>
        int missionID = -1;
        /// <summary>
        /// 传入的是匹配的时候队伍人员(满员)
        /// </summary>
        /// <param name="teamOne"></param>
        /// <param name="teamTwo"></param>
        public void init(List<int> teamOne, List<int> teamTwo) {
            this.teamOne.Clear();
            this.teamTwo.Clear();
            enterCount = 0;
            //初始化对象
            foreach(int item in teamOne)
            {
                SelectModel select = new SelectModel();
                select.userID = item;
                select.name = getUserName(item);
                select.hero = -1;
                select.ready = false;
                select.enter = false;
                this.teamOne.TryAdd(item, select);
            }
            foreach(int item in teamTwo)
            {
                SelectModel select = new SelectModel();
                select.userID = item;
                select.name = getUserName(item);
                select.hero = -1;
                select.ready = false;
                select.enter = false;
                this.teamTwo.TryAdd(item, select);
            }
            //Console.WriteLine("选择房间初始化成功......."+(this.teamOne.Count+this.teamTwo.Count));
            //初始化完成  开始进入计时
            missionID=ScheduleUtil.Instance.schedule(delegate { 
                 //30s后进入判断，如果不是全员 则解散房间
                //Console.WriteLine("1---30s后计算....");
                if(enterCount<teamOne.Count+teamTwo.Count)
                {
                    Destory();
                }
                else
                {
                    missionID = ScheduleUtil.Instance.schedule(delegate
                    {
                        //Console.WriteLine("2---30s后计算....");
                        bool selectAll = true;
                        foreach (SelectModel item in this.teamOne.Values)
                        {
                            if (item.hero == -1)
                            {
                                selectAll = false;
                                //Console.WriteLine("队伍1有人没选");
                            }
                                
                            break;
                        }
                        if (selectAll)
                        {
                            foreach (SelectModel item in this.teamTwo.Values)
                            {
                                if (item.hero == -1)
                                {
                                    selectAll = false;
                                   // Console.WriteLine("队伍2有人没选");
                                }
                                    
                                break;
                            }
                        }
                        //如果所有的角色都选择了英雄
                        if (selectAll)
                        {
                            //全部英雄选择了英雄却没有点击准备按钮
                            //Console.WriteLine("所有人选择了英雄.....");
                            StartFight();
                        }
                        else
                        {
                            Destory();
                            //Console.WriteLine("有人没有选择解散房间");
                        }
                        //missionID = -1;
                    }, 300*1000);
                }
            }, 300*1000);

        }
        public void Destory() {
            //通知所有用户退出选择英雄界面
            brocast(SelectProtocol.DESTORY_BRO, null);
            EventUtil.destorySelect(Area);
            if (missionID != -1)
            {
                ScheduleUtil.Instance.removeMission(missionID);
            }
            enterCount = 0;
        }
        public void ClientClose(NetFrame.UserToken token, string error) {
            leave(token);
            if (missionID != -1)
            {
                ScheduleUtil.Instance.removeMission(missionID);
            }
            brocast(SelectProtocol.DESTORY_BRO, null);
            EventUtil.destorySelect(Area);
        }
        public void ClientConnect(NetFrame.UserToken token) {
        }
        public void MessageReceive(NetFrame.UserToken token, NetFrame.auto.SocketModel message) {
            //当客户端在发来请求
            switch(message.Command)
            {
                case SelectProtocol.ENTER_CREQ:
                    Enter(token);
                    break;
                case SelectProtocol.SELECT_CREQ:
                    select(token,message.GetMessage<int>());
                    break;
                case SelectProtocol.TALK_CREQ:
                    talk(token, message.GetMessage<string>());
                    break;
                case SelectProtocol.READY_CREQ:
                    ready(token);
                    break;
            }
        }
        private void ready(UserToken token) {
            if (!base.isEntered(token))
                return;
            int userID = getUserID(token);
            if (readyList.Contains<int>(userID))
                return;
            SelectModel selectmodel = null;
            if(teamOne.ContainsKey(userID))
            {
                selectmodel=teamOne[userID];
            }else if(teamTwo.ContainsKey(userID))
            {
                selectmodel=teamTwo[userID];
            }
            //Console.WriteLine("客户端发来准备请求.." + userID);
            if (selectmodel.hero == -1)
                return;
            else
            {
                selectmodel.ready = true;
                brocast(SelectProtocol.READY_BRO, selectmodel);
                readyList.Add(userID);
                if(readyList.Count>=teamOne.Count+teamTwo.Count)
                {
                    //所有人准备 
                    //Console.WriteLine("所有人进入,进入战斗");
                    StartFight();
                }
            }
        }
        private void talk(UserToken token,string value) {
            if (!base.isEntered(token))
                return;
            UserModelDao user = getUser(token);
            brocast(SelectProtocol.TALK_BRO, user.name + ":" + value);

            //TODO队伍聊天


        }
        private void select(UserToken token, int value) {
            if (!base.isEntered(token)) { return; }
            UserModelDao user = getUser(token);
            if(!user.heroList.Contains(value))
            {
                write(token, SelectProtocol.SELECT_SRES, null);
                return;
            }
            SelectModel selectModel = null;
            if (teamOne.ContainsKey(user.id))
            {
                foreach(SelectModel item in teamOne.Values)
                {
                    if (item.hero == value)
                        return;
                }
                selectModel = teamOne[user.id];
            }
            else if (teamTwo.ContainsKey(user.id))
            {
                foreach (SelectModel item in teamTwo.Values)
                {
                    if (item.hero == value)
                        return;
                }
                selectModel = teamTwo[user.id];
            }
            selectModel.hero = value;
            brocast(SelectProtocol.SELECT_BRO, selectModel);
        }
        private void StartFight() {
            if(missionID!=-1)
            {
                ScheduleUtil.Instance.removeMission(missionID);
                missionID = -1;
            }
            //通知战斗模块 创建战斗房间
            EventUtil.createFight(teamOne.Values.ToArray(), teamTwo.Values.ToArray());
            brocast(SelectProtocol.FIGHT_BRO, null);
            //通知选择房间管理器 销毁当前房价
            EventUtil.destorySelect(Area);
        }
        private void Enter(UserToken token) {
            int userID = getUserID(token);
            if (teamOne.ContainsKey(userID))
            {
                teamOne[userID].enter = true;
            }
            else if (teamTwo.ContainsKey(userID))
            {
                teamTwo[userID].enter = true;
            }
            else
            {
                return;
            }
            //判断用户是否进入,没有则计算累加
            if(base.enter(token))
            {
                enterCount++;
            }

            SelectRoomDTO dto = new SelectRoomDTO();
            dto.teamOne = teamOne.Values.ToArray();
            dto.teamTwo = teamTwo.Values.ToArray();
            //告诉这个玩家房间的信息
            write(token, SelectProtocol.ENTER_SRES, dto);
            //告诉其他玩家这个用户进入房间
            brocast(SelectProtocol.ENTER_EXBRO, userID, token);//申请进入  返回选择的房间
        }
    }
}
