using System.Net;

namespace Jinx
{
    using System;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Events;
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
            var toggleJ = JinXxxMenu.JungleStealMenu["toggle"].Cast<CheckBox>().CurrentValue;
            var toggleaW = JinXxxMenu.MiscMenu["autoW"].Cast<CheckBox>().CurrentValue;

            if (toggleK)
            {
                KillSteal();
            }

            if (toggleJ)
            {
                JungleSteal();
            }

            if (toggleaW)
            {
                AutoW();
            }
        }

        /// <summary>
        /// Executes the Auto W Method
        /// </summary>
        private static void AutoW()
        {
            var stunW = JinXxxMenu.MiscMenu["stunW"].Cast<CheckBox>().CurrentValue;
            var dashW = JinXxxMenu.MiscMenu["dashW"].Cast<CheckBox>().CurrentValue;
            var charmW = JinXxxMenu.MiscMenu["charmW"].Cast<CheckBox>().CurrentValue;
            var tauntW = JinXxxMenu.MiscMenu["tauntW"].Cast<CheckBox>().CurrentValue;
            var fearW = JinXxxMenu.MiscMenu["fearW"].Cast<CheckBox>().CurrentValue;
            var snareW = JinXxxMenu.MiscMenu["snareW"].Cast<CheckBox>().CurrentValue;
            var wRange = JinXxxMenu.MiscMenu["wRange"].Cast<CheckBox>().CurrentValue;
            var wSlider = JinXxxMenu.MiscMenu["wSlider"].Cast<Slider>().CurrentValue;
            var enemy = EntityManager.Heroes.Enemies.Where(t => t.IsValidTarget() && Program.W.IsInRange(t)).OrderByDescending(t => t.Distance(Player.Instance));

            foreach (var target in enemy)
            {
                if (wRange && Player.Instance.Distance(target) <= Player.Instance.GetAutoAttackRange())
                {
                    return;
                }

                if (stunW && target.IsStunned)
                {
                    var prediction = Program.W.GetPrediction(target);

                    if (prediction.HitChancePercent >= wSlider && !prediction.Collision)
                    {
                        Program.W.Cast(prediction.CastPosition);
                    }
                }

                else if (dashW && target.IsDashing())
                {
                    var prediction = Program.W.GetPrediction(target);

                    if (prediction.HitChancePercent >= wSlider && !prediction.Collision)
                    {
                        Program.W.Cast(prediction.CastPosition);
                    }
                }

                else if (charmW && target.IsCharmed)
                {
                    var prediction = Program.W.GetPrediction(target);

                    if (prediction.HitChancePercent >= wSlider && !prediction.Collision)
                    {
                        Program.W.Cast(prediction.CastPosition);
                    }
                }

                else if (tauntW && target.IsTaunted)
                {
                    var prediction = Program.W.GetPrediction(target);

                    if (prediction.HitChancePercent >= wSlider && !prediction.Collision)
                    {
                        Program.W.Cast(prediction.CastPosition);
                    }
                }

                else if (fearW && target.IsFeared)
                {
                    var prediction = Program.W.GetPrediction(target);

                    if (prediction.HitChancePercent >= wSlider && !prediction.Collision)
                    {
                        Program.W.Cast(prediction.CastPosition);
                    }
                }

                else if (snareW && target.IsRooted)
                {
                    var prediction = Program.W.GetPrediction(target);

                    if (prediction.HitChancePercent >= wSlider && !prediction.Collision)
                    {
                        Program.W.Cast(prediction.CastPosition);
                    }
                }
            }
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
                            t.IsValidTarget() && Program.W.IsInRange(t) &&
                            Player.Instance.Distance(t) >=
                            JinXxxMenu.KillStealMenu["rRange"].Cast<Slider>().CurrentValue
                            && Essentials.DamageLibrary.CalculateDamage(t, false, true, false, true) >= t.Health)
                        .OrderByDescending(t => t.Health)
                        .FirstOrDefault();

                if (enemy != null)
                {
                    var pred = Program.W.GetPrediction(enemy);
                    var predR = Program.R.GetPrediction(enemy);

                    if (pred != null && pred.HitChancePercent >= wSlider && !pred.Collision)
                    {
                        Program.W.Cast(pred.CastPosition);
                    }

                    if (predR != null && predR.HitChancePercent >= rSlider)
                    {
                        Program.R.Cast(predR.CastPosition);
                    }
                }
                else
                {
                    KillSteal_2();
                }
            }
            else
            {
                KillSteal_2();
            }
        }

        /// <summary>
        /// Executes the Kill Steal Method
        /// </summary>
        private static void KillSteal_2()
        {
            var useW = JinXxxMenu.KillStealMenu["useW"].Cast<CheckBox>().CurrentValue;
            var useR = JinXxxMenu.KillStealMenu["useR"].Cast<CheckBox>().CurrentValue;
            var manaW = JinXxxMenu.KillStealMenu["manaW"].Cast<Slider>().CurrentValue;
            var manaR = JinXxxMenu.KillStealMenu["manaR"].Cast<Slider>().CurrentValue;
            var wSlider = JinXxxMenu.KillStealMenu["wSlider"].Cast<Slider>().CurrentValue;
            var rSlider = JinXxxMenu.KillStealMenu["rSlider"].Cast<Slider>().CurrentValue;

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

                    if (pred != null && pred.HitChancePercent >= wSlider && !pred.Collision)
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
                            &&
                            Player.Instance.Distance(t) >=
                            JinXxxMenu.KillStealMenu["rRange"].Cast<Slider>().CurrentValue
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
        
        /// <summary>
        /// Executes the Jungle Steal Method
        /// </summary>
        private static void JungleSteal()
        {
            var manaR = JinXxxMenu.JungleStealMenu["manaR"].Cast<Slider>().CurrentValue;
            var rRange = JinXxxMenu.JungleStealMenu["rRange"].Cast<Slider>().CurrentValue;

            if (Player.Instance.ManaPercent >= manaR)
            {
                if (Game.MapId == GameMapId.SummonersRift)
                {
                    var jungleMob =
                        EntityManager.MinionsAndMonsters.Monsters.FirstOrDefault(
                            u =>
                            u.IsVisible && Essentials.JungleMobsList.Contains(u.BaseSkinName)
                            && Essentials.DamageLibrary.CalculateDamage(u, false, false, false, true) >= u.Health);

                    if (jungleMob == null)
                    {
                        return;
                    }

                    if (!JinXxxMenu.JungleStealMenu[jungleMob.BaseSkinName].Cast<CheckBox>().CurrentValue)
                    {
                        return;
                    }

                    var enemy = EntityManager.Heroes.Enemies.Where(t => t.Distance(jungleMob) <= 100).OrderByDescending(t => t.Distance(jungleMob));

                    if (enemy.Any())
                    {
                        foreach (var target in enemy.Where(target => Player.Instance.Distance(target) < rRange))
                        {
                            if (target.Distance(jungleMob) <= 100)
                            {
                                Program.R.Cast(jungleMob.ServerPosition);
                            }
                        }
                    }
                }
                if (Game.MapId == GameMapId.TwistedTreeline)
                {
                    var jungleMob =
                        EntityManager.MinionsAndMonsters.Monsters.FirstOrDefault(
                            u =>
                            u.IsVisible && Essentials.JungleMobsListTwistedTreeline.Contains(u.BaseSkinName)
                            && Essentials.DamageLibrary.CalculateDamage(u, false, false, false, true) >= u.Health);

                    if (jungleMob == null)
                    {
                        return;
                    }

                    if (!JinXxxMenu.JungleStealMenu[jungleMob.BaseSkinName].Cast<CheckBox>().CurrentValue)
                    {
                        return;
                    }

                    var enemy = EntityManager.Heroes.Enemies.Where(t => t.Distance(jungleMob) <= 100).OrderByDescending(t => t.Distance(jungleMob));

                    if (enemy.Any())
                    {
                        foreach (var target in enemy.Where(target => Player.Instance.Distance(target) < rRange))
                        {
                            if (target.Distance(jungleMob) <= 100)
                            {
                                Program.R.Cast(jungleMob.ServerPosition);
                            }
                        }
                    }
                }
            }
        }
    }
}