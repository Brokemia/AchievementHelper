using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.AchievementHelper.Conditions {
    public abstract class Condition {
        public virtual void ParseArguments(string condition, int position) { }

        // Returns new position
        protected int SkipSpaces(string condition, int position) {
            while (condition[position] == ' ') {
                position++;
            }
            return position;
        }

        public abstract bool Evaluate();
    }
}
