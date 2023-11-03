using MonoMod.ModInterop;

namespace Celeste.Mod.AchievementHelper {
    [ModExportName("AchievementHelper")]
    public static class AchievementHelperExports {
        public static void TriggerAchievement(string mod, string name) {
            AchievementManager.Instance.TriggerAchievement(mod, name);
        }

        public static bool HasAchievement(string mod, string name) {
            return AchievementManager.Instance.HasAchievement(mod, name);
        }

        public static void ConditionChanged(string condition) {
            ConditionHelperImports.ConditionChanged(condition);
        }
    }
}
