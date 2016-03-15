using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using GameProtocol.dto.fight;
using GameProtocol.dto;
using GameProtocol.data;
using LOLServer.tool;
using GameProtocol;

namespace LOLServer.Logic.fight {
    public class FightRoom:AbsMulitHandler,HandlerInterface {
        /// <summary>
        /// 保存队伍一的所有单位
        /// </summary>
        public ConcurrentDictionary<int, AbsFightModel> teamOne = new ConcurrentDictionary<int, AbsFightModel>();
        /// <summary>
        /// 保存队伍二的所有单位
        /// </summary>
        public ConcurrentDictionary<int, AbsFightModel> teamTwo = new ConcurrentDictionary<int, AbsFightModel>();
        /// <summary>
        /// 玩家离开时添加到这个列表
        /// </summary>
        public List<int> off = new List<int>();

        public List<int> enterList = new List<int>();

        public int heroCount;

        private int monsterID = -21;
        public override byte Type {
            get {
                return GameProtocol.Protocol.TYPE_FIGHT;
            }
        }
        /// <summary>
        /// 初始化战斗场景数据
        /// </summary>
        /// <param name="teamOne"></param>
        /// <param name="teamTwo"></param>
        public void init(GameProtocol.dto.SelectModel[] teamOne, GameProtocol.dto.SelectModel[] teamTwo) {
            heroCount = teamTwo.Length + teamOne.Length;
            this.teamOne.Clear();
            this.teamTwo.Clear();
            off.Clear();
            foreach(SelectModel item in teamOne)
            {
                this.teamOne.TryAdd(item.userID, create(item,1));
            }
            foreach(SelectModel item in teamTwo)
            {
                this.teamTwo.TryAdd(item.userID, create(item,2));
            }
            ///实例化队伍一的建筑
            ///预留 ID段 -1到-10为队伍1建筑
            for (int i = -1; i >= -3; i--)
            {
                this.teamOne.TryAdd(i, createBuild(i, Math.Abs(i), 1));
            }
            ///实例化队伍二的建筑
            ///预留 ID段 -11到-20为队伍2建筑
            for (int i = -11; i >= -13; i--)
            {
                this.teamTwo.TryAdd(i, createBuild(i, Math.Abs(i) - 10, 2));
            }
            enterList.Clear();
        }
        /// <summary>
        /// 创建建筑单位
        /// </summary>
        /// <param name="id"></param>
        /// <param name="code"></param>
        /// <param name="team"></param>
        /// <returns></returns>
        FightBuildModel createBuild(int id, int code, int team) {
            //从配置文件读取防御塔数据
            BuildDataModel data = BuildData.buildMap[code];
            FightBuildModel build = new FightBuildModel(id,code,data.hp,data.hp,data.atk,data.def,data.reborn,data.rebornTime,data.initiative,data.infrared,data.name);
            //build.type = ModelType.BUILD;
            build.team = team;
            if(team==1)
            {
                switch(code)
                {
                    case 1:
                        build.defaultVec = new BuildVector(57.53f, 65.40f, 152.86f);
                        break;
                    case 2:
                        build.defaultVec = new BuildVector(69.66f, 65.76f, 140.3086f);
                        break;
                    case 3:
                        build.defaultVec = new BuildVector(78.65204f, 65.82565f, 130.9393f);
                        break;
                }
            }
            else if(team==2)
            {
                switch (code)
                {
                    case 1:
                        build.defaultVec = new BuildVector(154.323f, 66.3f, 55.37466f);
                        break;
                    case 2:
                        build.defaultVec = new BuildVector(144.4072f, 65.4998f, 65.06526f);
                        break;
                    case 3:
                        build.defaultVec = new BuildVector(134.5275f, 65.85461f, 75.14024f);
                        break;
                }
            }
            return build;
        }
        /// <summary>
        /// 创建英雄单位
        /// </summary>
        /// <param name="model"></param>
        /// <param name="team"></param>
        /// <returns></returns>
        FightPlayerModel create(SelectModel model, int team) {
            FightPlayerModel player = new FightPlayerModel();
            player.id = model.userID;
            player.code = model.hero;
            player.type = ModelType.HUMAN;
            player.name = getUserName(model.userID);
            
            player.exp = 0;
            player.level = 1;
            player.free = 1;
            player.money = 0;
            player.team = team;
            //从配置表里 去出对应的英雄数据
            HeroDataModel data = HeroData.heroMap[model.hero];
            player.hp = data.hpBase;
            player.maxHp = data.hpBase;
            player.atk = data.atkBase;
            player.def = data.defBase;
            player.aSpeed = data.aSpeed;
            player.speed = data.speed;
            player.aRange = data.range;
            player.eyeRange = data.eyeRange;
            player.skills = initSkill(data.skills);
            player.equs = new int[3];


            return player;
        }
        /// <summary>
        /// 初始化英雄的技能
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        FightSkill[] initSkill(int[] value) {
            FightSkill[] skills = new FightSkill[value.Length];
            for (int i = 0; i < value.Length;i++ )
            {
                int skillCode = value[i];
                SkillDataModel data=SkillData.skillMap[skillCode];
                SkillLevelData levelData = data.levels[0];
                FightSkill skill = new FightSkill(skillCode, 0, levelData.level, levelData.time, data.name, levelData.range, data.info, data.target, data.type);
                skills[i] = skill;
            }
            return skills;
        }
        public void ClientConnect(NetFrame.UserToken token) {
            
        }
        public void ClientClose(NetFrame.UserToken token, string error) {
            //当有客户端离开
            leave(token);

            int userId = getUserID(token);
            if (teamOne.ContainsKey(userId) || teamTwo.ContainsKey(userId))
            {
                if (!off.Contains(userId))
                    off.Add(userId);
            }
            if (off.Count == heroCount)
            {
                ScheduleUtil.Instance.removeMission(refeshID);
                EventUtil.destoryFight(Area);
            }
        }
        public void MessageReceive(NetFrame.UserToken token, NetFrame.auto.SocketModel message) {
           switch(message.Command)
           {
               case FightProtocol.ENTER_CREQ:
                   enterBattle(token);
                   break;
               case FightProtocol.MOVE_CREQ:
                   move(token, message.GetMessage<MoveDTO>());
                   break;
               case FightProtocol.ATTACK_CREQ:
                   attack(token, message.GetMessage<int>());
                   break;
               case FightProtocol.DAMAGE_CREQ:
                   damage(token, message.GetMessage<DamageDTO>());
                   break;
               case FightProtocol.SKILL_UP_CREQ:
                   skillUp(token, message.GetMessage<int>());
                   break;
               case FightProtocol.SKILL_CREQ:
                   skill(token, message.GetMessage<SkillAttackModel>());
                   break;
           }
        }
        public int refeshID=-1;
        public void refeshMonster() {
            refeshID=ScheduleUtil.Instance.timeSchedule(delegate
            {
                List<FightMonsterModel> mons = new List<FightMonsterModel>();
                //刷新怪物
                FightMonsterModel monster = new FightMonsterModel();
                monster.id = monsterID;
                monsterID--;
                monster.name = "1-小兵";
                monster.team = 1;
                mons.Add(monster);
                teamOne.TryAdd(monsterID,monster);

                monster = new FightMonsterModel();
                monster.id = monsterID;
                monsterID--;
                monster.name = "2-小兵";
                monster.team = 2;

                refeshMonster();
                mons.Add(monster);
                teamTwo.TryAdd(monsterID, monster);

                brocast(FightProtocol.REFESH_BRO,mons.ToArray());

            }, 30 * 1000);
        }
        void skill(NetFrame.UserToken token, SkillAttackModel value) {
            value.userId =getUserID(token);
            brocast(FightProtocol.SKILL_BRO, value);
        }
        void skillUp(NetFrame.UserToken token, int value) {
            int userId = getUserID(token);
            FightPlayerModel player;
            if (teamOne.ContainsKey(userId))
            {
                player = (FightPlayerModel)teamOne[userId];
            }
            else
            {
                player = (FightPlayerModel)teamTwo[userId];
            }
             if (player.free > 0)
             {
                 foreach(FightSkill item in player.skills)
                 {
                     if(item.code==value)
                     {
                         if(item.nextLevel!=-1&&item.nextLevel<=player.level)
                         {
                             player.free -= 1;
                             int level = item.level + 1;
                             SkillLevelData data = SkillData.skillMap[value].levels[level];
                             item.nextLevel = data.level;
                             item.range = data.range;
                             item.time = data.time;
                             item.level = level;
                             write(token, FightProtocol.SKILL_UP_SRES, item);
                         }
                     }
                     return;
                 }
             }
        }
        void damage(NetFrame.UserToken token,DamageDTO value) {
            int userID = getUserID(token);
            AbsFightModel atkModel;
            int skillLevel = 0;
            //判断攻击者
            if(value.userId>=0)
            {
                if (value.userId != userID)
                    return;
                atkModel = getPlayer(userID);
                if (value.skill > 0)
                {
                    skillLevel = (atkModel as FightPlayerModel).SkillLevel(value.skill);
                    if (skillLevel == -1)
                    {
                        return;
                    }
                }
            }else
            {
                atkModel = getPlayer(userID);
            }
            //TODO
            //获取技能算法
            //循环获取目标数据 和攻击者数据 进行伤害计算 得出伤害数值
            if (!SkillProcessMap.has(value.skill))
                return;
            ISkill skill = SkillProcessMap.get(value.skill);
            List<int[]> damages = new List<int[]>();
            foreach (int[] item in value.target)
            {
                AbsFightModel target = getPlayer(item[0]);
                skill.damage(skillLevel, ref atkModel, ref target, ref damages);
                //Console.WriteLine("计算伤害值");
                if (target.hp == 0)
                {
                    switch (target.type)
                    {
                        case ModelType.HUMAN:
                            if (target.id > 0)
                            {
                                //击杀英雄
                                //启动定时任务 指定时间之后发送英雄复活信息 并且将英雄数据设置为满状态
                            }
                            else
                            {
                                //击杀小兵
                                //移除小兵数据
                            }
                            break;
                        case ModelType.BUILD:
                            //打破了建筑 给钱
                            break;
                    }
                }
            }
            value.target = damages.ToArray();
            brocast(FightProtocol.DAMAGE_BRO, value);
        }
        AbsFightModel getPlayer(int userId) {
            if (teamOne.ContainsKey(userId))
            {
                return teamOne[userId];
            }
            return teamTwo[userId];
        }
        void attack(NetFrame.UserToken token, int value) {
            AttackDTO dto = new AttackDTO();
            dto.userID = getUserID(token);
            dto.targetID = value;
            brocast(FightProtocol.ATTACK_BRO, dto);
        }
        void move(NetFrame.UserToken token, MoveDTO value) {
            int userId = getUserID(token);
            value.userId = userId;
            brocast(FightProtocol.MOVE_BRO, value);
        }
        void enterBattle(NetFrame.UserToken token) {
            int userID = getUserID(token);
            if (isEntered(token))
                return;
            base.enter(token);
            if (!enterList.Contains(userID))
                enterList.Add(userID);
            //所有人准备了 发送房间信息
            if (enterList.Count == heroCount)
            {
                FightRoomModel room = new FightRoomModel();
                room.teamOne = teamOne.Values.ToArray();
                room.teamTwo = teamTwo.Values.ToArray();
                brocast(FightProtocol.START_BRO, room);
                //刷新小兵
                refeshMonster();
            }
        }
    }
}
