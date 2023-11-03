using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.AchievementHelper.Entities {
    [CustomEntity("achievementHelper/achievementController")]
    [Tracked]
    public class AchievementController : Entity {
        private string condition;
        private string modName, achievementName;
        private int watchingID;

        public AchievementController(EntityData data, Vector2 offset) {
            condition = data.Attr("condition", "0");
            modName = data.Attr("modName");
            achievementName = data.Attr("achievementName");
        }

        public override void Added(Scene scene) {
            base.Added(scene);
            watchingID = ConditionHelperImports.WatchConditions(condition, Check);
        }

        public override void Removed(Scene scene) {
            base.Removed(scene);
            ConditionHelperImports.RemoveCallback(watchingID);
        }

        public override void Awake(Scene scene) {
            base.Awake(scene);
            if(AchievementManager.Instance.HasAchievement(modName, achievementName)) {
                RemoveSelf();
            }
            Check();
        }

        private void Check() {
            if (condition.Equals("") || ConditionHelperImports.EvaluateConditionExpression(condition)) {
                AchievementManager.Instance.TriggerAchievement(modName, achievementName);
                RemoveSelf();
            }
        }

    }
}
