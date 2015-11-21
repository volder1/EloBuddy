namespace Jinx
{
    using System;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;

    /// <summary>
    /// Class that executes Kill Steal and Jungle Steal
    /// </summary>
    internal class ActiveStates
    {
        /// <summary>
        /// Called every time the Game Ticks
        /// </summary>
        /// <param name="args">The Args</param>
        public static void Game_OnUpdate(EventArgs args)
        {
            if (Player.Instance.IsDead || Player.Instance.HasBuff("Recall") || Player.Instance.IsCharmed
                || Player.Instance.IsStunned || Player.Instance.IsRooted)
            {
                return;
            }

            var toggleK = JinXxxMenu.KillStealMenu["toggle"].Cast<CheckBox>().CurrentValue;
            //var toggleJ = JinXxxMenu.JungleStealMenu["toggle"].Cast<CheckBox>().CurrentValue;

            if (toggleK)
            {
                KillSteal();
            }

            /*if (toggleJ)
            {
                JungleSteal();
            }*/
        }

        /// <summary>
        /// Executes the Kill Steal Method
        /// </summary>
        private static void KillSteal()
        {
            var useW = JinXxxMenu.KillStealMenu["useW"].Cast<CheckBox>().CurrentValue;
            var useR = JinXxxMenu.KillStealMenu["useR"].Cast<CheckBox>().CurrentValue;
            var manaW = JinXxxMenu.KillStealMenu["manaW"].Cast<Slider>().CurrentValue;
            var manaR = JinXxxMenu.KillStealMenu["manaR"].Cast<Slider>().CurrentValue;
            var wSlider = JinXxxMenu.KillStealMenu["wSlider"].Cast<Slider>().CurrentValue;
            var rSlider = JinXxxMenu.KillStealMenu["rSlider"].Cast<Slider>().CurrentValue;

            if (useW && useR && Player.Instance.ManaPercent >= manaW && Player.Instance.ManaPercent >= manaR
                && Program.W.IsReady() && Program.R.IsReady())
            {
                var enemy =
                    EntityManager.Heroes.Enemies.Where(
                        t =>
                        t.IsValidTarget() && Program.W.IsInRange(t) && Program.R.IsInRange(t)
                        && Essentials.DamageLibrary.CalculateDamage(t, false, true, false, true) >= t.Health)
                        .OrderByDescending(t => t.Health)
                        .FirstOrDefault();

                if (enemy != null)
                {
                    var pred = Program.W.GetPrediction(enemy);
                    var predR = Program.R.GetPrediction(enemy);

                    if (pred != null && pred.HitChancePercent >= wSlider)
                    {
                        Program.W.Cast(pred.CastPosition);
                    }

                    if (predR != null && predR.HitChancePercent >= rSlider)
                    {
                        Program.R.Cast(predR.CastPosition);
                    }
                }
            }


            if (useW && Player.Instance.ManaPercent >= manaW && Program.W.IsReady())
            {
                var enemy =
                    EntityManager.Heroes.Enemies.Where(
                        t =>
                        t.IsValidTarget() && Program.W.IsInRange(t)
                        && Essentials.DamageLibrary.CalculateDamage(t, false, true, false, false) >= t.Health)
                        .OrderByDescending(t => t.Health)
                        .FirstOrDefault();

                if (enemy != null)
                {
                    var pred = Program.W.GetPrediction(enemy);

                    if (pred != null && pred.HitChancePercent >= wSlider)
                    {
                        Program.W.Cast(pred.CastPosition);
                    }
                }
            }

            if (useR && Player.Instance.ManaPercent >= manaR && Program.R.IsReady())
            {
                var enemy =
                    EntityManager.Heroes.Enemies.Where(
                        t =>
                        t.IsValidTarget()
                        && Player.Instance.Distance(t) <= JinXxxMenu.KillStealMenu["rRange"].Cast<Slider>().CurrentValue
                        && Essentials.DamageLibrary.CalculateDamage(t, false, false, false, true) >= t.Health)
                        .OrderByDescending(t => t.Health)
                        .FirstOrDefault();

                if (enemy != null)
                {
                    var pred = Program.R.GetPrediction(enemy);

                    if (pred != null && pred.HitChancePercent >= rSlider)
                    {
                        Program.R.Cast(pred.CastPosition);
                    }
                }
            }
        }

        /*
        /// <summary>
        /// Executes the Jungle Steal Method
        /// </summary>
        private static void JungleSteal()
        {
            
        }*/
    }
}