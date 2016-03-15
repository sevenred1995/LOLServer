using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOLServer.dao {
    public class UserModelDao {
        public int id;
        public string name;
        public int level=0;
        public int exp=0;
        public int winCount=0;
        public int loseCount=0;
        public int runCount=0;
        /// <summary>
        /// 该角色的账号ID
        /// </summary>
        public int accountID;

        public List<int> heroList=new List<int>();
        public UserModelDao() {
            level = 1;
            exp = 0;
            winCount = 0;
            loseCount = 0;
            loseCount = 0;
            runCount = 0;
        }
        public UserModelDao(int id, string name, int level, int exp, int wincount, int losecount, int runcount,int accountID) {
            this.id = id;
            this.name = name;
            this.level = level;
            this.exp = exp;
            this.winCount = wincount;
            this.loseCount = losecount;
            this.runCount = runcount;
            this.accountID = accountID;
        }
    }
}
