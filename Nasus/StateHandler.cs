using System;

namespace Nasus
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Menu.Values;

    class StateHandler
    {
        /// <summary>
        /// Does KillSteal
        /// </summary>
        public static void KillSteal(EventArgs args)
        {
            var useE = Config.KillStealMenu["useE"].Cast<CheckBox>().CurrentValue;

            if (!useE)
            {
                return;
            }
            var target = TargetSelector.GetTarget(
                EntityManager.Heroes.Enemies.Where(
                    t => t.IsValidTarget(Program.E.Range) &&
                        Extensions.DamageLibrary.CalculateDamage(t, false, true)
                        >= t.Health), DamageType.Magical);

            if (target == null) return;

            Program.E.Cast(target);
        }

        /// <summary>
        /// Does LastHit
        /// </summary>
        public static void LastHit()
        {
            if (!Program.Q.IsReady() || !Orbwalker.CanAutoAttack)
            {
                return;
            }

            var minion = EntityManager.MinionsAndMonsters.EnemyMinions.Where(
                t => t.IsValidTarget(Program.Q.Range) &&
                     Extensions.DamageLibrary.CalculateDamage(t, true, false) >= t.Health)
                .OrderByDescending(t => t.Distance(Player.Instance))
                .FirstOrDefault();

            if (minion == null) return;

            Program.Q.Cast();
            Orbwalker.ForcedTarget = minion;
        }

        /// <summary>
        /// Does Harass
        /// </summary>
        public static void Harass()
        {
            var useEh = Config.FarmMenu["useEH"].Cast<CheckBox>().CurrentValue;
            var manaEh = Config.FarmMenu["manaEH"].Cast<Slider>().CurrentValue;

            if (!Program.E.IsReady() || !useEh || !(Player.Instance.ManaPercent >= manaEh))
            {
                return;
            }
            var target = EntityManager.Heroes.Enemies.Where(t => Program.E.IsInRange(t) && t.IsValidTarget());

            foreach (
                var pred in
                    target.Select(t => Program.E.GetPrediction(t))
                        .Where(pred => pred != null)
                        .Where(pred => pred.HitChance >= HitChance.High))
            {
                Program.E.Cast(pred.CastPosition);
            }
        }

        /// <summary>
        /// Does LaneClear
        /// </summary>
        public static void LaneClear()
        {
            var useElc = Config.FarmMenu["useELC"].Cast<CheckBox>().CurrentValue;
            var useElcs = Config.FarmMenu["useELCS"].Cast<Slider>().CurrentValue;
            var manaElc = Config.FarmMenu["manaELC"].Cast<Slider>().CurrentValue;

            if (!Program.E.IsReady() || !useElc || !(Player.Instance.ManaPercent >= manaElc))
            {
                return;
            }
            var minion =
                EntityManager.MinionsAndMonsters.EnemyMinions.Where(
                    t =>
                        Program.E.IsInRange(t) && t.IsValidTarget() &&
                        Extensions.DamageLibrary.CalculateDamage(t, false, true) >= t.Health)
                    .ToArray();

            var pred = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(minion, Program.E.Width,
                (int) Program.E.Range);

            if (pred.HitNumber >= useElcs)
            {
                Program.E.Cast(pred.CastPosition);
            }
        }

        /// <summary>
        /// Does Combo
        /// </summary>
        public static void Combo()
        {
            var useW = Config.ComboMenu["useW"].Cast<CheckBox>().CurrentValue;
            var manaW = Config.ComboMenu["manaW"].Cast<Slider>().CurrentValue;

            if (Program.W.IsReady()
                && useW
                && Player.Instance.ManaPercent >= manaW)
            {
                var target =
                    EntityManager.Heroes.Enemies.Where(t => t.IsValidTarget(Program.W.Range))
                        .OrderBy(t => t.Health)
                        .FirstOrDefault();

                if (target != null)
                {
                    Program.W.Cast(target);
                }
            }

            var useE = Config.ComboMenu["useE"].Cast<CheckBox>().CurrentValue;
            var manaE = Config.ComboMenu["manaE"].Cast<Slider>().CurrentValue;

            if (Program.E.IsReady()
                && useE
                && Player.Instance.ManaPercent >= manaE)
            {
                var target = EntityManager.Heroes.Enemies.Where(t => Program.E.IsInRange(t)
                                                                     && t.IsValidTarget());

                foreach (
                    var pred in
                        target.Select(t => Program.E.GetPrediction(t))
                            .Where(pred => pred != null)
                            .Where(pred => pred.HitChance >= HitChance.High))
                {
                    Program.E.Cast(pred.CastPosition);
                }
            }

            var useR = Config.ComboMenu["useR"].Cast<CheckBox>().CurrentValue;
            var hpR = Config.ComboMenu["hpR"].Cast<Slider>().CurrentValue;
            var rangeR = Config.ComboMenu["rangeR"].Cast<Slider>().CurrentValue;
            var intR = Config.ComboMenu["intR"].Cast<Slider>().CurrentValue;

            if (!Program.R.IsReady() || !useR || !(Player.Instance.HealthPercent <= hpR))
            {
                return;
            }
            var enemies = EntityManager.Heroes.Enemies.Count(t => Player.Instance.Distance(t) <= rangeR);

            if (enemies >= intR)
            {
                Program.R.Cast();
            }
        }
    }
}