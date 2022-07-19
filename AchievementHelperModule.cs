using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.AchievementHelper.Conditions;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using Soukoku.ExpressionParser;

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

        public Evaluator ExpressionEvaluator { get; private set; }

        public ConditionWatcher ConditionWatcher { get; private set; } = new();

        public AchievementHelperModule() {
            Instance = this;
        }

        public override void Load() {
            Celeste.Instance.Components.Add(Component = new(Celeste.Instance));

            On.Celeste.OuiChapterSelect.Render += OuiChapterSelect_Render;
            On.Celeste.OuiChapterSelect.Enter += OuiChapterSelect_Enter;
            On.Celeste.OuiChapterSelect.Leave += OuiChapterSelect_Leave;
            On.Celeste.OuiChapterSelect.Update += OuiChapterSelect_Update;

            Everest.Content.OnUpdate += onModAssetUpdate;

            LoadEvaluator();
        }

        private void LoadEvaluator() {
            var context = new EvaluationContext();

            context.RegisterFunction("flag", new FunctionRoutine(1, (ctx, args) => (Engine.Scene is Level level && level.Session.GetFlag(args[0].Value)) ? ExpressionToken.True : ExpressionToken.False));
            context.RegisterFunction("chapterStrawberries", new FunctionRoutine(1, (ctx, args) => 
                (SaveData.Instance == null || Engine.Scene is not Level level || level.Session is not Session session) ? new ExpressionToken("-1") 
                : new(SaveData.Instance.Areas[session.Area.ID].Modes[(int)session.Area.Mode].TotalStrawberries.ToString())
            ));
            context.RegisterFunction("levelSetStrawberries", new FunctionRoutine(1, (ctx, args) =>
                (SaveData.Instance == null) ? new ExpressionToken("-1")
                : new(SaveData.Instance.LevelSetStats.TotalStrawberries.ToString())
            ));

            ExpressionEvaluator = new Evaluator(context);
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
            Logger.Log(LogLevel.Verbose, "AchievementHelper", "Found AchievementHelperAchievements.yaml");
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

            Everest.Content.OnUpdate -= onModAssetUpdate;
        }
    }
}