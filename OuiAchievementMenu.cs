using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.AchievementHelper {
    class OuiAchievementMenu : Oui {
		private TextMenu menu;

		private const float onScreenX = 960;

		private const float offScreenX = 2880;

		private float alpha;

		public override void Added(Scene scene) {
			base.Added(scene);
		}

		public override IEnumerator Enter(Oui from) {
			AchievementManager.Instance.TriggerAchievement("AchievementHelper", "Open_Achievements");
			menu = new();
			menu.Add(new TextMenu.Header(Dialog.Clean("AchievementHelper_UI_Achievements")));
			foreach (string mod in AchievementManager.Instance.GetAllMods()) {
				List<string> achievements = AchievementManager.Instance.GetAllAchievementsForMod(mod).ToList();
				if(achievements.Count > 0) {
					menu.Add(new TextMenu.SubHeader(Dialog.Clean("Achievement_" + mod + "_ModName")));
                }

				foreach(string name in achievements) {
					if(AchievementManager.Instance.TryGet(mod, name, out Achievement achievement)) {
						bool collected = AchievementManager.Instance.HasAchievement(mod, name);
						if ((collected || !achievement.SuperSecret) && !achievement.Invisible) {
							menu.Add(new AchievementMenuItem(achievement, collected));
						}
                    }
                }
            }
			
			Scene.Add(menu);

			menu.Visible = (Visible = true);
			menu.Focused = false;
			Overworld.Maddy.Hide();
			for (float p = 0f; p < 1f; p += Engine.DeltaTime * 4f) {
				menu.X = offScreenX + -1920f * Ease.CubeOut(p);
				alpha = Ease.CubeOut(p);
				yield return null;
			}
			menu.Focused = true;
		}

		public override IEnumerator Leave(Oui next) {
			Audio.Play("event:/ui/main/whoosh_large_out");
			menu.Focused = false;
			for (float p = 0f; p < 1f; p += Engine.DeltaTime * 4f) {
				menu.X = onScreenX + 1920f * Ease.CubeIn(p);
				alpha = 1f - Ease.CubeIn(p);
				yield return null;
			}
			menu.Visible = Visible = false;
			menu.RemoveSelf();
			menu = null;
		}

		public override void Update() {
			if (menu != null && menu.Focused && Selected && Input.MenuCancel.Pressed) {
				Audio.Play("event:/ui/main/button_back");
				Overworld.Goto<OuiChapterSelect>();
			}
			base.Update();
		}

		public override void Render() {
			if (alpha > 0f) {
				Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * alpha * 0.4f);
			}

			base.Render();
		}
	}
}
