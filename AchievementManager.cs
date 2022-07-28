using Soukoku.ExpressionParser;
using System.Collections.Generic;

namespace Celeste.Mod.AchievementHelper {
    public class AchievementManager {
        public static AchievementManager Instance { get; private set; } = new AchievementManager();

        private AchievementManager() { }

        private readonly Dictionary<string, Dictionary<string, Achievement>> RegisteredAchievements = new();

        private readonly Dictionary<string, Dictionary<string, int>> watchingIDs = new();

        public void RegisterAchievement(Achievement achievement) {
            if(!RegisteredAchievements.ContainsKey(achievement.Mod)) {
                RegisteredAchievements[achievement.Mod] = new();
            }
            // Unregister any old callback
            if(watchingIDs.ContainsKey(achievement.Mod) && watchingIDs[achievement.Mod].ContainsKey(achievement.Name)) {
                AchievementHelperModule.Instance.ConditionWatcher.RemoveCallback(watchingIDs[achievement.Mod][achievement.Name]);
                watchingIDs[achievement.Mod].Remove(achievement.Name);
            }
            RegisteredAchievements[achievement.Mod][achievement.Name] = achievement;
            // Register callbacks for a condition
            if (achievement.Condition != null && !achievement.Condition.Equals("")) {
                if(!watchingIDs.ContainsKey(achievement.Mod)) {
                    watchingIDs[achievement.Mod] = new();
                }
                watchingIDs[achievement.Mod][achievement.Name] = AchievementHelperModule.Instance.ConditionWatcher.WatchConditions(achievement.Condition, () => {
                    if (AchievementHelperModule.Instance.ExpressionEvaluator.Evaluate(achievement.Condition, true).Equals(ExpressionToken.True)) {
                        TriggerAchievement(achievement.Mod, achievement.Name);
                    }
                });
            }
        }

        public void TriggerAchievement(string mod, string name) {
            if (!HasAchievement(mod, name)) {
                if(!AchievementHelperModule.ModSaveData.Achievements.ContainsKey(mod)) {
                    AchievementHelperModule.ModSaveData.Achievements.Add(mod, new());
                }
                AchievementHelperModule.ModSaveData.Achievements[mod].Add(name);
                if (TryGet(mod, name, out Achievement achievement)) {
                    if (!achievement.Invisible) {
                        AchievementHelperModule.Instance.Component.ShowPopup(achievement);
                    }
                    foreach (var implied in achievement.Implies) {
                        TriggerAchievement(implied.Item1, implied.Item2);
                    }
                } else {
                    AchievementHelperModule.Instance.Component.ShowPopup(new Achievement { Name = "Achievement Not Found" });
                }
            }
        }

        public Dictionary<string, Dictionary<string, Achievement>>.KeyCollection GetAllMods() {
            return RegisteredAchievements.Keys;
        }

        public Dictionary<string, Achievement>.KeyCollection GetAllAchievementsForMod(string mod) {
            return RegisteredAchievements[mod].Keys;
        }

        public Achievement Get(string mod, string name) => RegisteredAchievements[mod][name];

        public bool TryGet(string mod, string name, out Achievement achievement) {
            if(RegisteredAchievements.TryGetValue(mod, out Dictionary<string, Achievement> modAchievements) && modAchievements.TryGetValue(name, out achievement)) {
                return true;
            }
            achievement = null;
            return false;
        }

        public bool HasAchievement(string mod, string name) {
            return AchievementHelperModule.ModSaveData.Achievements.ContainsKey(mod) && AchievementHelperModule.ModSaveData.Achievements[mod].Contains(name);
        }

    }
}
