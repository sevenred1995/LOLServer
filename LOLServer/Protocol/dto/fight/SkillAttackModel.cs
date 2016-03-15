using System;
using System.Collections.Generic;
using System.Text;

namespace GameProtocol.dto.fight {
    [Serializable]
    public class SkillAttackModel {
        public int userId;
        public int type;//0表示目标攻击 1表示指定点
        public int skill;//技能code;
        public float[] position;//攻击点
        public int target;//目标
    }
}
