using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using Soukoku.ExpressionParser;

namespace Celeste.Mod.AchievementHelper.Triggers {
    [CustomEntity("achievementHelper/triggerAchievement")]
    public class AchievementTrigger : Trigger {
        private string modName, achievementName, condition;

        public AchievementTrigger(EntityData data, Vector2 offset) : base(data, offset) {
            condition = data.Attr("condition", "1");
            modName = data.Attr("modName");
            achievementName = data.Attr("achievementName");
        }

        public override void Awake(Scene scene) {
            base.Awake(scene);
            if (AchievementManager.Instance.HasAchievement(modName, achievementName)) {
                RemoveSelf();
            }
        }

        public override void OnEnter(Player player) {
            base.OnEnter(player);
            if (condition.Equals("") || AchievementHelperModule.Instance.ExpressionEvaluator.Evaluate(condition, true).Equals(ExpressionToken.True)) {
                AchievementManager.Instance.TriggerAchievement(modName, achievementName);
                RemoveSelf();
            }
        }

    }
}
