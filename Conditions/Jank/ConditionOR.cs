using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.AchievementHelper.Conditions {
    public class ConditionOR : Condition {
        private Condition[] conditions;


        public ConditionOR(params Condition[] conditions) {
            this.conditions = conditions;
        }

        public override bool Evaluate() {
            return conditions.Any(c => c.Evaluate());
        }
    }
}
