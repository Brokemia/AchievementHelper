using System;
using System.Collections.Generic;

namespace Celeste.Mod.AchievementHelper {
    public class AchievementHelperModuleSettings : EverestModuleSettings {

        [SettingName("AchievementHelper_UI_MenuBinding")]
        public ButtonBinding AchievementMenuBinding { get; set; } = new(Microsoft.Xna.Framework.Input.Buttons.Back, Microsoft.Xna.Framework.Input.Keys.Back);

    }
}
