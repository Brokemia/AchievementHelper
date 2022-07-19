using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.AchievementHelper.Conditions {
    public class ConditionAND : Condition {
        private Condition[] conditions;


        public ConditionAND(params Condition[] conditions) {
            this.conditions = conditions;
        }

        public override bool Evaluate() {
            return conditions.All(c => c.Evaluate());
        }
    }
}
