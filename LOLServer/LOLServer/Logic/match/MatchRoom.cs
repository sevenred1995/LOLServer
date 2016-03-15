using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOLServer.Logic.match {
    public class MatchRoom{
        /// <summary>
        /// 房间唯一id
        /// </summary>
        public int id;
        /// <summary>
        /// 每支队伍最大人数
        /// </summary>
        public int teamMax = 1;
        /// <summary>
        /// 队伍列表
        /// </summary>
        public List<int> teamOne = new List<int>();
        public List<int> teamTwo = new List<int>();
    }
}
