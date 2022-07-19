using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.AchievementHelper.Conditions {
    public class ConditionParser {
        private int position;

        public void Parse(string condition) {
            position = 0;
            ParseInternal(condition);
        }

        private Condition ParseInternal(string condition) {
            if (condition.Length <= 0 || position >= condition.Length || SkipSpaces(condition)) {
                return null;
            }

            Condition lastCondition = NextCondition(condition);

            while (!SkipSpaces(condition)) {
                switch (condition[position]) {
                    case ')':
                        position++;
                        return lastCondition;
                    case '&':
                        position++;
                        lastCondition = new ConditionAND(lastCondition, NextCondition(condition));
                        break;
                    case '|':
                        position++;
                        lastCondition = new ConditionOR(lastCondition, NextCondition(condition));
                        break;
                }
            }

            return lastCondition;
        }

        // Returns true if it reaches the end of the condition
        private bool SkipSpaces(string condition) {
            while (condition[position] == ' ') {
                position++;
                if (position >= condition.Length) {
                    return true;
                }
            }
            return false;
        }

        private Condition NextCondition(string condition) {
            if(SkipSpaces(condition)) {
                return null;
            }

            switch(condition[position]) {
                case '(':
                    position++;
                    return ParseInternal(condition);
                case '[':
                    position++;
                    SkipSpaces(condition);
                    string keyword = "";
                    while(condition[position] != ' ' || condition[position] != ']') {
                        keyword += condition[position];
                        position++;
                    }
                    Condition res = (Condition)Type.GetType("Celeste.Mod.AchievementHelper.Conditions.Condition" + keyword.ToUpper()).GetConstructor(new Type[0]).Invoke(new object[0]);
                    res.ParseArguments(condition, position);
                    return res;
            }

            return null;
        }

    }
}
