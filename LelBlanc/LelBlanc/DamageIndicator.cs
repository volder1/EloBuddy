using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

using SharpDX;

namespace LelBlanc
{
    /// <summary>
    /// Credits to MarioGK. From KickAss AIO
    /// </summary>
    class DamageIndicator
    {
        private const int BarWidth = 106;
        private const float LineThickness = 9.8f;

        public DamageIndicator()
        {
            Drawing.OnEndScene += OnEndScene;
        }

        private static void OnEndScene(EventArgs args)
        {
            if (Config.DrawingMenu["draw.Damage"].Cast<CheckBox>().CurrentValue)
            {
                foreach (var unit in EntityManager.Heroes.Enemies.Where(u => u.IsValidTarget() && u.IsHPBarRendered))
                {
                    var drawQ = Config.DrawingMenu["draw.Q"].Cast<CheckBox>().CurrentValue;

                    var drawW = Config.DrawingMenu["draw.W"].Cast<CheckBox>().CurrentValue;

                    var drawE = Config.DrawingMenu["draw.E"].Cast<CheckBox>().CurrentValue;

                    var drawR = Config.DrawingMenu["draw.R"].Cast<CheckBox>().CurrentValue;

                    var damage = Extension.DamageLibrary.CalculateDamage(unit, drawQ, drawW, drawE, drawR);

                    if (damage <= 0)
                    {
                        continue;
                    }
                    var damagePercentage = ((unit.TotalShieldHealth() - damage) > 0 ? (unit.TotalShieldHealth() - damage) : 0) /
                                            (unit.MaxHealth + unit.AllShield + unit.AttackShield + unit.MagicShield);
                    var currentHealthPercentage = unit.TotalShieldHealth() / (unit.MaxHealth + unit.AllShield + unit.AttackShield + unit.MagicShield);

                    var startPoint = new Vector2((int)(unit.HPBarPosition.X + damagePercentage * BarWidth), (int)unit.HPBarPosition.Y - 5);
                    var endPoint = new Vector2((int)(unit.HPBarPosition.X + currentHealthPercentage * BarWidth) + 1, (int)unit.HPBarPosition.Y - 5);

                    var A = Config.DrawingMenu["draw_Alpha"].Cast<Slider>().CurrentValue;
                    var R = Config.DrawingMenu["draw_Red"].Cast<Slider>().CurrentValue;
                    var G = Config.DrawingMenu["draw_Green"].Cast<Slider>().CurrentValue;
                    var B = Config.DrawingMenu["draw_Blue"].Cast<Slider>().CurrentValue;

                    var colorH = System.Drawing.Color.FromArgb(A - 120, R,
                        G, B);

                    Drawing.DrawLine(startPoint, endPoint, LineThickness, colorH);
                }
            }
        }
    }
}
