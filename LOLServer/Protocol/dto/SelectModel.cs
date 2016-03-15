using System;
using System.Collections.Generic;
using System.Text;

namespace GameProtocol.dto {
    [Serializable]
    public class SelectModel {
        public int userID;//用户ID
        public string name;
        public int hero;//所选英雄
        public bool enter;//是否进入
        public bool ready;//是否已准备
    }
}
