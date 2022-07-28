using Monocle;
using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Celeste.Mod.AchievementHelper {
    public class Achievement {
        public string Mod { get; set; }

        public string Name { get; set; }

        // These achievements will be automatically earned when this achievement is earned
        public List<Tuple<string, string>> Implies { get; } = new();

        private string _icon;

        public string Icon {
            get => _icon;
            set {
                _icon = value;
                IconTextures = GFX.Gui.GetAtlasSubtextures(Icon.Replace('\\', '/'));
            }
        }

        [YamlIgnore]
        public List<MTexture> IconTextures { get; private set; }

        public float AnimationSpeed { get; set; } = 12f;

        public bool Secret { get; set; }

        public bool SuperSecret { get; set; }

        public bool Invisible { get; set; }

        public string Condition { get; set; }
    }
}
