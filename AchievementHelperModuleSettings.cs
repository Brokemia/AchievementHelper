using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Celeste.Mod.AchievementHelper {
    public class AchievementHelperModuleSettings : EverestModuleSettings {

        [SettingName("AchievementHelper_UI_MenuBinding")]
        public ButtonBinding AchievementMenuBinding { get; set; } = new(Buttons.Back, Keys.Back);

    }
}
