using Celeste.Mod.Entities;
using Monocle;
using Soukoku.ExpressionParser;

namespace Celeste.Mod.AchievementHelper.Entities {
    [CustomEntity("achievementHelper/achievementController")]
    [Tracked]
    public class AchievementController : Entity {
        private string condition;
        private string modName, achievementName;
        private int watchingID;

        public AchievementController(EntityData data) {
            data.Attr("condition", "false");
            modName = data.Attr("modName");
            achievementName = data.Attr("achievementName");
        }

        public override void Added(Scene scene) {
            base.Added(scene);
            watchingID = AchievementHelperModule.Instance.ConditionWatcher.WatchConditions(condition, Check);
        }

        public override void Removed(Scene scene) {
            base.Removed(scene);
            AchievementHelperModule.Instance.ConditionWatcher.RemoveCallback(watchingID);
        }

        public override void Awake(Scene scene) {
            base.Awake(scene);
            if(AchievementManager.Instance.HasAchievement(modName, achievementName)) {
                RemoveSelf();
            }
            Check();
        }

        private void Check() {
            if (AchievementHelperModule.Instance.ExpressionEvaluator.Evaluate(condition, true).Equals(ExpressionToken.True)) {
                AchievementManager.Instance.TriggerAchievement(modName, achievementName);
                RemoveSelf();
            }
        }

    }
}
