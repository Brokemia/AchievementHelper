using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.ModInterop;

namespace Celeste.Mod.AchievementHelper {
    public class AchievementHelperModule : EverestModule {
        public static AchievementHelperModule Instance { get; private set; }

        public override Type SettingsType => typeof(AchievementHelperModuleSettings);
        public static AchievementHelperModuleSettings Settings => (AchievementHelperModuleSettings) Instance._Settings;

        public override Type SaveDataType => typeof(AchievementHelperModuleSaveData);
        public static AchievementHelperModuleSaveData ModSaveData => (AchievementHelperModuleSaveData) Instance._SaveData;

        public AchievementGameComponent Component { get; private set; }

        private float mainMenuEasing;
        private Wiggler mainMenuWiggle = Wiggler.Create(0.4f, 4f);

        public AchievementHelperModule() {
            Instance = this;
        }

        public override void Load() {
            typeof(AchievementHelperExports).ModInterop();
            typeof(ConditionHelperImports).ModInterop();
            Celeste.Instance.Components.Add(Component = new(Celeste.Instance));

            On.Celeste.OuiChapterSelect.Render += OuiChapterSelect_Render;
            On.Celeste.OuiChapterSelect.Enter += OuiChapterSelect_Enter;
            On.Celeste.OuiChapterSelect.Leave += OuiChapterSelect_Leave;
            On.Celeste.OuiChapterSelect.Update += OuiChapterSelect_Update;

            On.Celeste.SaveData.InitializeDebugMode += SaveData_InitializeDebugMode;

            Everest.Content.OnUpdate += onModAssetUpdate;
        }

        private void SaveData_InitializeDebugMode(On.Celeste.SaveData.orig_InitializeDebugMode orig, bool loadExisting) {
            orig(loadExisting);
            ModSaveData.Achievements.Clear();
        }

        public override void Initialize() {
            base.Initialize();
            foreach (ModAsset asset in
                Everest.Content.Mods
                .Select(mod => mod.Map.TryGetValue("AchievementHelperAchievements", out ModAsset asset) ? asset : null)
                .Where(asset => asset != null && asset.Type == typeof(AssetTypeYaml))
            ) {
                LoadAchievements(asset);
            }
        }

        private void onModAssetUpdate(ModAsset oldAsset, ModAsset newAsset) {
            if (newAsset?.PathVirtual == "AchievementHelperAchievements" && newAsset?.Type == typeof(AssetTypeYaml)) {
                LoadAchievements(newAsset);
            }
        }

        private void LoadAchievements(ModAsset asset) {
            Logger.Log(LogLevel.Verbose, "AchievementHelper", "Found AchievementHelperAchievements.yaml in " + asset.Source?.Name);
            List<Achievement> achievements = asset.Deserialize<List<Achievement>>();
            foreach (Achievement achievement in achievements) {
                AchievementManager.Instance.RegisterAchievement(achievement);
            }
        }

        private void OuiChapterSelect_Update(On.Celeste.OuiChapterSelect.orig_Update orig, OuiChapterSelect self) {
            if (self.Selected && self.Focused && Settings.AchievementMenuBinding.Pressed) {
                self.Focused = false;
                Audio.Play("event:/ui/main/button_select");
                Audio.Play("event:/ui/main/whoosh_large_in");
                mainMenuWiggle.Start();
                self.Overworld.Goto<OuiAchievementMenu>();
            }

            orig(self);
        }

        private IEnumerator OuiChapterSelect_Leave(On.Celeste.OuiChapterSelect.orig_Leave orig, OuiChapterSelect self, Oui next) {
            Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, 0.2f, start: true);
            self.Add(tween);
            tween.OnUpdate = delegate (Tween t) {
                mainMenuEasing = t.Eased;
            };
            return orig(self, next);
        }

        private IEnumerator OuiChapterSelect_Enter(On.Celeste.OuiChapterSelect.orig_Enter orig, OuiChapterSelect self, Oui from) {
            Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, 0.4f, start: true);
            self.Add(tween);
            tween.OnUpdate = delegate (Tween t)
            {
                mainMenuEasing = 1 - t.Eased;
            };
            return orig(self, from);
        }

        private void OuiChapterSelect_Render(On.Celeste.OuiChapterSelect.orig_Render orig, OuiChapterSelect self) {
            orig(self);
            float scale = 0.5f;
            int num2 = 32;
            string label = Dialog.Clean("AchievementHelper_UI_Achievements", null);
            float width = ButtonUI.Width(label, Input.MenuCancel);
            Vector2 position = new Vector2(1880f, 56f);
            position.X += (40f + width * scale + num2) * mainMenuEasing;
            ButtonUI.Render(position, label, Settings.AchievementMenuBinding.Button, scale, 1f, mainMenuWiggle.Value * 0.05f);
        }

        public override void Unload() {
            Celeste.Instance.Components.Remove(Component);
            Component?.Dispose();

            On.Celeste.OuiChapterSelect.Render -= OuiChapterSelect_Render;
            On.Celeste.OuiChapterSelect.Enter -= OuiChapterSelect_Enter;
            On.Celeste.OuiChapterSelect.Leave -= OuiChapterSelect_Leave;
            On.Celeste.OuiChapterSelect.Update -= OuiChapterSelect_Update;

            On.Celeste.SaveData.InitializeDebugMode -= SaveData_InitializeDebugMode;

            Everest.Content.OnUpdate -= onModAssetUpdate;
        }
    }
}