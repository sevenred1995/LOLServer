using GameProtocol.data.Skill;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProtocol.data {
    public class SkillProcessMap {
        static Dictionary<int, ISkill> skills = new Dictionary<int, ISkill>();
        static SkillProcessMap() {
            put(-1, new SkillAtk());
        }

        static void put(int code, ISkill skill) {
            skills.Add(code, skill);
        }

        public static bool has(int code) {
            return skills.ContainsKey(code);
        }
        public static ISkill get(int code) {
            return skills[code];
        }
    }
}
