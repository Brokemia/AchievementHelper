using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections.Generic;
using MDraw = Monocle.Draw;

namespace Celeste.Mod.AchievementHelper {
    public class AchievementGameComponent : DrawableGameComponent {
        private const float AchievementMinWidth = 400;
        private const float AchievementHeight = 80;
        private const float IconSize = 80;
        private const float PopupTime = 5;
        private const float TransitionTime = 0.3f;
        private const float IconTextSeparation = 10;
        private const float MinimumRightPadding = 20;
        private const float NameScale = 0.7f;
        private const float ModNameScale = 0.5f;

        private Achievement current;
        private Queue<Achievement> achievements = new();
        private float achievementTimer = 0;
        private float transitionTimer = 0;
        private float frame = 0;

        public AchievementGameComponent(Game game) : base(game) {
        }

        public void ShowPopup(Achievement achievement) {
            achievements.Enqueue(achievement);
        }

        public void ShowNext() {
            current = achievements.Dequeue();
            achievementTimer = PopupTime;
            transitionTimer = 0;
            frame = 0;
        }

        public override void Draw(GameTime gameTime) {
            base.Draw(gameTime);
            if(current != null) {
                MDraw.SpriteBatch.Begin(
                    SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    SamplerState.LinearClamp,
                    DepthStencilState.None,
                    RasterizerState.CullNone,
                    null,
                    Engine.ScreenMatrix
                );

                string name = Dialog.Clean("Achievement_" + current.Mod + "_" + current.Name + "_Name");
                string modName = Dialog.Clean("Achievement_" + current.Mod + "_" + current.Name + "_Description");

                float width = Math.Max(AchievementMinWidth, AchievementHeight + IconTextSeparation + MinimumRightPadding + Math.Max(ActiveFont.Measure(name).X * NameScale, ActiveFont.Measure(modName).X * ModNameScale));
                Vector2 topRight = Vector2.Lerp(new(Celeste.TargetWidth - width, Celeste.TargetHeight), new(Celeste.TargetWidth - width, Celeste.TargetHeight - AchievementHeight), transitionTimer / TransitionTime);
                MDraw.Rect(topRight, width, AchievementHeight, Color.DarkSlateBlue);

                if (current.IconTextures != null) {
                    MTexture tex = current.IconTextures[(int)frame];
                    tex.Draw(topRight + new Vector2((AchievementHeight - IconSize) / 2), Vector2.Zero, Color.White, IconSize / Math.Max(tex.Width, tex.Height));
                }

                ActiveFont.DrawOutline(name, topRight + new Vector2(AchievementHeight + IconTextSeparation, AchievementHeight / 3), new Vector2(0, 0.5f), new Vector2(NameScale), Color.White, 2, Color.Black);
                ActiveFont.DrawOutline(modName, topRight + new Vector2(AchievementHeight + IconTextSeparation, AchievementHeight * 3 / 4), new Vector2(0, 0.5f), new Vector2(ModNameScale), Color.LightGray, 2, Color.Black);

                MDraw.HollowRect(topRight, width, AchievementHeight, Color.DimGray);
                MDraw.HollowRect(topRight + Vector2.One, width - 2, AchievementHeight - 2, Color.DimGray);

                MDraw.SpriteBatch.End();
            }
        }
        
        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            if (current != null) {
                // Animate icon
                if (current.IconTextures != null && current.IconTextures.Count > 1) {
                    frame += current.AnimationSpeed * Engine.RawDeltaTime;
                    frame %= current.IconTextures.Count;
                }

                if (achievementTimer > 0) {
                    if (transitionTimer >= TransitionTime) {
                        transitionTimer = TransitionTime;
                        achievementTimer -= Engine.RawDeltaTime;
                    } else {
                        transitionTimer += Engine.RawDeltaTime;
                    }
                } else if (transitionTimer > 0) {
                    transitionTimer -= Engine.RawDeltaTime;
                } else {
                    current = null;
                }
            } else if(achievements.Count > 0) {
                ShowNext();
            }
        }
    }
}
