using System.Collections.Generic;

namespace Celeste.Mod.AchievementHelper {
    public class AchievementManager {
        public static AchievementManager Instance { get; private set; } = new AchievementManager();

        private AchievementManager() { }

        private readonly Dictionary<string, Dictionary<string, Achievement>> RegisteredAchievements = new();

        public void RegisterAchievement(Achievement achievement) {
            if(!RegisteredAchievements.ContainsKey(achievement.Mod)) {
                RegisteredAchievements[achievement.Mod] = new();
            }
            RegisteredAchievements[achievement.Mod][achievement.Name] = achievement;
        }

        public void TriggerAchievement(string mod, string name) {
            if (!HasAchievement(mod, name)) {
                if(!AchievementHelperModule.SaveData.Achievements.ContainsKey(mod)) {
                    AchievementHelperModule.SaveData.Achievements.Add(mod, new());
                }
                AchievementHelperModule.SaveData.Achievements[mod].Add(name);
                if (TryGet(mod, name, out Achievement achievement)) {
                    AchievementHelperModule.Instance.Component.ShowPopup(achievement);
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
            return AchievementHelperModule.SaveData.Achievements.ContainsKey(mod) && AchievementHelperModule.SaveData.Achievements[mod].Contains(name);
        }

    }
}
