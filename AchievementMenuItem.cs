using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.AchievementHelper {
    public class AchievementMenuItem : TextMenu.Item {
		private const float AchievementMinWidth = 400;
		private const float AchievementHeight = 80;
		private const float IconSize = 80;
		private const float IconTextSeparation = 10;
		private const float MinimumRightPadding = 20;
		private const float NameScale = 0.7f;
		private const float ModNameScale = 0.5f;
		private const float MenuEntryPadding = 20f;

		private float frame = 0;
		private bool selected = false;

		public Achievement Achievement { get; private set; }
		private bool collected;

		private MTexture secretIcon;

		public float MinWidth = 0;

		public AchievementMenuItem(Achievement achievement, bool collected) {
			Achievement = achievement;
			this.collected = collected;
			secretIcon = GFX.Gui["areas/null"];
			Selectable = true;
			OnEnter += delegate {
				selected = true;
				frame = 0;
			};
			OnLeave += delegate {
				selected = false;
				frame = 0;
			};
		}

		/// <inheritdoc />
		public override void ConfirmPressed() {
			//if (!string.IsNullOrEmpty(ConfirmSfx)) {
			//	Audio.Play(ConfirmSfx);
			//}
			base.ConfirmPressed();
		}

		/// <inheritdoc />
		public override float LeftWidth() {
			string name = Dialog.Clean("Achievement_" + Achievement.Mod + "_" + Achievement.Name + "_Name");
			string description = Dialog.Clean("Achievement_" + Achievement.Mod + "_" + Achievement.Name + "_Description");
			return Calc.Max(AchievementMinWidth, MinWidth, AchievementHeight + IconTextSeparation + MinimumRightPadding + Math.Max(ActiveFont.Measure(name).X * NameScale, ActiveFont.Measure(description).X * ModNameScale));
		}

		/// <inheritdoc />
		public override float Height() {
			return AchievementHeight + MenuEntryPadding;
		}

		public override void Render(Vector2 position, bool highlighted) {
			float alpha = Container.Alpha;
			string name = (!collected && Achievement.SecretName) ? "???" : Dialog.Clean("Achievement_" + Achievement.Mod + "_" + Achievement.Name + "_Name");
			string description = (!collected && Achievement.SecretDescription) ? "???" : Dialog.Clean("Achievement_" + Achievement.Mod + "_" + Achievement.Name + "_Description");

			float width = LeftWidth();
			Vector2 topRight = position - new Vector2(0, AchievementHeight / 2);
			Draw.Rect(topRight, width, AchievementHeight, Color.DarkSlateBlue * alpha);

			if (Achievement.IconTextures != null) {
				MTexture tex = (!collected && Achievement.SecretIcon) ? secretIcon : Achievement.IconTextures[selected ? (int)frame : 0];
				tex.Draw(topRight + new Vector2((AchievementHeight - IconSize) / 2), Vector2.Zero, Color.White * alpha, IconSize / Math.Max(tex.Width, tex.Height));
			}

			ActiveFont.DrawOutline(name, topRight + new Vector2(AchievementHeight + IconTextSeparation, AchievementHeight / 3), new Vector2(0, 0.5f), new Vector2(NameScale), Color.White * alpha, 2, Color.Black * alpha * alpha * alpha);
			ActiveFont.DrawOutline(description, topRight + new Vector2(AchievementHeight + IconTextSeparation, AchievementHeight * 3 / 4), new Vector2(0, 0.5f), new Vector2(ModNameScale), Color.LightGray * alpha, 2, Color.Black * alpha * alpha * alpha);

			Draw.HollowRect(topRight, width, AchievementHeight, (selected ? Color.Yellow : Color.DimGray) * alpha);
			Draw.HollowRect(topRight + Vector2.One, width - 2, AchievementHeight - 2, (selected ? Color.Yellow : Color.DimGray) * alpha);

			if (!collected) {
				Draw.Rect(topRight, width, AchievementHeight, Color.Black * 0.5f * alpha);
			}
		}

        public override void Update() {
            base.Update();
			if (Achievement.IconTextures != null && Achievement.IconTextures.Count > 1) {
				frame += Achievement.AnimationSpeed * Engine.RawDeltaTime;
				frame %= Achievement.IconTextures.Count;
			}
		}

    }
}
