using System;
using System.Collections.Generic;

namespace Celeste.Mod.AchievementHelper {
    public class AchievementHelperModuleSettings : EverestModuleSettings {

        public ButtonBinding AchievementMenuBinding { get; set; } = new(Microsoft.Xna.Framework.Input.Buttons.Back, Microsoft.Xna.Framework.Input.Keys.Back);

    }
}
