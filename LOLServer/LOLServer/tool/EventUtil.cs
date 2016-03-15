using GameProtocol.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 创建选人模块事件
/// </summary>
/// <param name="teamOne"></param>
/// <param name="teamTwo"></param>
public delegate void CreateSelect(List<int> teamOne,List<int> teamTwo);
/// <summary>
/// 
/// </summary>
/// <param name="roomID"></param>
public delegate void DestorySelect(int roomID);

public delegate void CreateFight(SelectModel[] teamOne,SelectModel[] teamTwo);

public delegate void DestoryFight(int roomID);
namespace LOLServer.tool {
    public class EventUtil {
        public static CreateSelect createSelect;
        public static DestorySelect destorySelect;
        public static DestoryFight destoryFight;
        public static CreateFight createFight;
    }
}
