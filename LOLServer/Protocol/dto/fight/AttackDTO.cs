using System;
using System.Collections.Generic;
using System.Text;

namespace GameProtocol.dto.fight {
    [Serializable]
    public class AttackDTO {
        public int userID;//攻击者
        public int targetID;//被攻击者
    }
}
