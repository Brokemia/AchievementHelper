using System.Collections.Generic;

namespace Celeste.Mod.AchievementHelper {
    public class AchievementHelperModuleSaveData : EverestModuleSaveData {

        public Dictionary<string, List<string>> Achievements { get; set; } = new();

    }
}
