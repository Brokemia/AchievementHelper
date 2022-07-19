using Soukoku.ExpressionParser;
using Soukoku.ExpressionParser.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.AchievementHelper.Conditions {
    public class ConditionWatcher {
        public const string FLAG_FCN = "flag";

        private struct LabeledCallback {
            public int Label { get; set; }
            public Action Callback { get; set; }
        }

        private readonly Dictionary<string, List<LabeledCallback>> watching = new();

        private readonly InfixTokenizer tokenizer = new();

        private static int ID = 0;

        // Returns the unique ID for this callback
        public int WatchConditions(string expr, Action callback) {
            ExpressionToken[] tokens = tokenizer.Tokenize(expr);
            List<string> alreadyAdded = new();
            
            foreach(ExpressionToken token in tokens) {
                if(token.TokenType == ExpressionTokenType.Function && !alreadyAdded.Contains(token.Value)) {
                    if(!watching.ContainsKey(token.Value)) {
                        watching[token.Value] = new();
                    }
                    watching[token.Value].Add(new() { Label = ID, Callback = callback });
                    alreadyAdded.Add(token.Value);
                }
            }
            ID++;
            return ID - 1;
        }

        public void RemoveCallback(int id) {
            foreach (string key in watching.Keys) {
                watching[key].RemoveAll(lcb => lcb.Label == id);
            }
        }

        public void Load() {
            On.Celeste.Session.SetFlag += Session_SetFlag;
        }

        private void Session_SetFlag(On.Celeste.Session.orig_SetFlag orig, Session self, string flag, bool setTo) {
            if (watching.ContainsKey(FLAG_FCN)) {
                foreach (LabeledCallback cb in watching[FLAG_FCN]) {
                    cb.Callback?.Invoke();
                }
            }
            orig(self, flag, setTo);
        }
    }
}
