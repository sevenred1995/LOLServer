using System;
using System.Collections.Generic;
using System.Text;

namespace GameProtocol.dto {
    [Serializable]
    public class UserModel {
        public int id;
        public string name;
        public int level;
        public int exp;
        public int winCount;
        public int loseCount;
        public int runCount;
        public int[] heroList;
        public UserModel() {
        }
        public UserModel(int id, string name, int level, int exp, int wincount, int losecount, int runcount,int[] heros) {
            this.id = id;
            this.name = name;
            this.level = level;
            this.exp = exp;
            this.winCount = wincount;
            this.loseCount = losecount;
            this.runCount = runcount;
            heroList = heros;
        }
    }
}
