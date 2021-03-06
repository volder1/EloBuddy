﻿namespace Lux
{
    namespace DamageIndicator
    {
        using System;

        using EloBuddy;
        using EloBuddy.SDK;
        using EloBuddy.SDK.Menu.Values;
        using EloBuddy.SDK.Rendering;

        using SharpDX;

        using Color = System.Drawing.Color;

        /// <summary>
        /// Credits to Fluxy for Original Code.
        /// </summary>
        public class DamageIndicator
        {
            private const float BarLength = 104;
            private const float XOffset = 0;
            private const float YOffset = 11;
            public float CheckDistance = 1200;

            public DamageIndicator()
            {
                Drawing.OnEndScene += Drawing_OnDraw;
            }

            private static void Drawing_OnDraw(EventArgs args)
            {
                if (!Program.DrawingMenu["draw.Damage"].Cast<CheckBox>().CurrentValue) return;

                foreach (var aiHeroClient in EntityManager.Heroes.Enemies)
                {
                    if (!aiHeroClient.IsHPBarRendered) continue;

                    var pos = new Vector2(
                        aiHeroClient.HPBarPosition.X + XOffset,
                        aiHeroClient.HPBarPosition.Y + YOffset);

                    var fullbar = (BarLength) * (aiHeroClient.HealthPercent / 100);

                    var drawQ = Program.DrawingMenu["draw.Q"].Cast<CheckBox>().CurrentValue;

                    var drawW = Program.DrawingMenu["draw.W"].Cast<CheckBox>().CurrentValue;

                    var drawE = Program.DrawingMenu["draw.E"].Cast<CheckBox>().CurrentValue;

                    var drawR = Program.DrawingMenu["draw.R"].Cast<CheckBox>().CurrentValue;

                    var damage = (BarLength)
                                 * ((Program.DamageLibrary.CalculateDamage(aiHeroClient, drawQ, drawW, drawE, drawR)
                                     / aiHeroClient.MaxHealth) > 1
                                        ? 1
                                        : (Program.DamageLibrary.CalculateDamage(
                                            aiHeroClient,
                                            drawQ,
                                            drawW,
                                            drawE,
                                            drawR) / aiHeroClient.MaxHealth));

                    Line.DrawLine(
                        Color.FromArgb(100, Color.Black),
                        9f,
                        new Vector2(pos.X, pos.Y),
                        new Vector2(pos.X + (damage > fullbar ? fullbar : damage), pos.Y));

                    Line.DrawLine(
                        Color.Black,
                        3,
                        new Vector2(pos.X + (damage > fullbar ? fullbar : damage), pos.Y),
                        new Vector2(pos.X + (damage > fullbar ? fullbar : damage), pos.Y));
                }
            }
        }
    }
}
