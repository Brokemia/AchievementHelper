using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.AchievementHelper.Conditions {
    public class ConditionFLAG : Condition {
        private string flag;

        public override void ParseArguments(string condition, int position) {
            position = SkipSpaces(condition, position);
            flag = "";
            while(condition[position] != ']') {
                flag += condition[position];
                position++;
            }
        }

        public override bool Evaluate() {
            return true;
        }
    }
}
