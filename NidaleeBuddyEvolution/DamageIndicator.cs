namespace NidaleeBuddyEvolution
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
            private const float XOffset = 2;
            private const float YOffset = 9;
            public float CheckDistance = 1200;

            public DamageIndicator()
            {
                Drawing.OnEndScene += Drawing_OnDraw;
            }

            private static void Drawing_OnDraw(EventArgs args)
            {
                if (!NidaleeMenu.DrawingMenu["draw.Damage"].Cast<CheckBox>().CurrentValue) return;

                foreach (var aiHeroClient in EntityManager.Heroes.Enemies)
                {
                    if (!aiHeroClient.IsHPBarRendered) continue;

                    var pos = new Vector2(
                        aiHeroClient.HPBarPosition.X + XOffset,
                        aiHeroClient.HPBarPosition.Y + YOffset);

                    var fullbar = (BarLength) * (aiHeroClient.HealthPercent / 100);

                    var drawQ = NidaleeMenu.DrawingMenu["draw.Q"].Cast<CheckBox>().CurrentValue;

                    var drawW = NidaleeMenu.DrawingMenu["draw.W"].Cast<CheckBox>().CurrentValue;

                    var drawE = NidaleeMenu.DrawingMenu["draw.E"].Cast<CheckBox>().CurrentValue;

                    var drawR = NidaleeMenu.DrawingMenu["draw.R"].Cast<CheckBox>().CurrentValue;

                    var damage = (BarLength)
                                 * ((Essentials.DamageLibrary.CalculateDamage(aiHeroClient, drawQ, drawW, drawE, drawR)
                                    / aiHeroClient.MaxHealth) > 1
                                     ? 1
                                     : (Essentials.DamageLibrary.CalculateDamage(
                                         aiHeroClient,
                                         drawQ,
                                         drawW,
                                         drawE,
                                         drawR) / aiHeroClient.MaxHealth));

                    var A = NidaleeMenu.DrawingMenu["draw_Alpha"].Cast<Slider>().CurrentValue;
                    var R = NidaleeMenu.DrawingMenu["draw_Red"].Cast<Slider>().CurrentValue;
                    var G = NidaleeMenu.DrawingMenu["draw_Green"].Cast<Slider>().CurrentValue;
                    var B = NidaleeMenu.DrawingMenu["draw_Blue"].Cast<Slider>().CurrentValue;

                    Line.DrawLine(
                        Color.FromArgb(A, R, G, B),
                        9f,
                        new Vector2(pos.X, pos.Y),
                        new Vector2(pos.X + (damage > fullbar ? fullbar : damage), pos.Y));

                    Line.DrawLine(
                        Color.FromArgb(A, R, G, B),
                        3,
                        new Vector2(pos.X + (damage > fullbar ? fullbar : damage), pos.Y),
                        new Vector2(pos.X + (damage > fullbar ? fullbar : damage), pos.Y));
                }
            }
        }
    }
}
