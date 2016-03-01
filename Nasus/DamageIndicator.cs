using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

using SharpDX;

using Color = System.Drawing.Color;

namespace Nasus
{
    internal class DamageIndicator
    {
        private const int BarWidth = 106;
        private const float LineThickness = 9.8f;

        public DamageIndicator()
        {
            Drawing.OnEndScene += OnEndScene;
        }

        public static void OnEndScene(EventArgs args)
        {
            if (Config.DrawingMenu["draw.Damage"].Cast<CheckBox>().CurrentValue)
            {
                foreach (var unit in EntityManager.Heroes.Enemies.Where(u => u.IsValidTarget() && u.IsHPBarRendered))
                {
                    var drawQ = Config.DrawingMenu["draw.Q"].Cast<CheckBox>().CurrentValue;

                    var drawE = Config.DrawingMenu["draw.E"].Cast<CheckBox>().CurrentValue;

                    var damage = Extensions.DamageLibrary.CalculateDamage(unit, drawQ, drawE);

                    if (damage <= 0)
                    {
                        continue;
                    }
                    var damagePercentage = ((unit.TotalShieldHealth() - damage) > 0
                        ? (unit.TotalShieldHealth() - damage)
                        : 0)/
                                           (unit.MaxHealth + unit.AllShield + unit.AttackShield + unit.MagicShield);
                    var currentHealthPercentage = unit.TotalShieldHealth()/
                                                  (unit.MaxHealth + unit.AllShield + unit.AttackShield +
                                                   unit.MagicShield);

                    var startPoint = new Vector2((int) (unit.HPBarPosition.X + damagePercentage*BarWidth),
                        (int) unit.HPBarPosition.Y - 5 + 14);
                    var endPoint = new Vector2((int) (unit.HPBarPosition.X + currentHealthPercentage*BarWidth) + 1,
                        (int) unit.HPBarPosition.Y - 5 + 14);

                    var a = Config.DrawingMenu["draw_Alpha"].Cast<Slider>().CurrentValue;
                    var r = Config.DrawingMenu["draw_Red"].Cast<Slider>().CurrentValue;
                    var g = Config.DrawingMenu["draw_Green"].Cast<Slider>().CurrentValue;
                    var b = Config.DrawingMenu["draw_Blue"].Cast<Slider>().CurrentValue;

                    var colorH = Color.FromArgb(a - 120, r,
                        g, b);

                    Drawing.DrawLine(startPoint, endPoint, LineThickness, colorH);
                }
            }
        }
    }
}