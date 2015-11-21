﻿namespace Jinx
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;

    /// <summary>
    /// Class that executes the modes.
    /// </summary>
    internal class StateManager
    {
        /// <summary>
        /// Does Combo Method
        /// </summary>
        public static void Combo()
        {
            var useQ = JinXxxMenu.ComboMenu["useQ"].Cast<CheckBox>().CurrentValue;
            var useW = JinXxxMenu.ComboMenu["useW"].Cast<CheckBox>().CurrentValue;
            var useE = JinXxxMenu.ComboMenu["useE"].Cast<CheckBox>().CurrentValue;
            var useR = JinXxxMenu.ComboMenu["useR"].Cast<CheckBox>().CurrentValue;
            var wRange = JinXxxMenu.MiscMenu["wRange"].Cast<CheckBox>().CurrentValue;
            var wRange2 = JinXxxMenu.ComboMenu["wRange2"].Cast<Slider>().CurrentValue;
            var manaQ = JinXxxMenu.ComboMenu["manaQ"].Cast<Slider>().CurrentValue;
            var manaW = JinXxxMenu.ComboMenu["manaW"].Cast<Slider>().CurrentValue;
            var manaE = JinXxxMenu.ComboMenu["manaE"].Cast<Slider>().CurrentValue;
            var manaR = JinXxxMenu.ComboMenu["manaR"].Cast<Slider>().CurrentValue;
            var wSlider = JinXxxMenu.ComboMenu["wSlider"].Cast<Slider>().CurrentValue;
            var eSlider = JinXxxMenu.ComboMenu["eSlider"].Cast<Slider>().CurrentValue;
            var rSlider = JinXxxMenu.ComboMenu["rSlider"].Cast<Slider>().CurrentValue;
            var rCountC = JinXxxMenu.ComboMenu["rCountC"].Cast<Slider>().CurrentValue;

            if (useQ && Player.Instance.ManaPercent >= manaQ && Program.Q.IsReady() && !Essentials.FishBones())
            {
                Essentials.MinigunQLogic();
            }

            if (useQ && Program.Q.IsReady() && Essentials.FishBones())
            {
                Essentials.FishbonesQLogic();
            }

            if (useW && Player.Instance.ManaPercent >= manaW && Program.W.IsReady())
            {
                var target = TargetSelector.GetTarget(Program.W.Range, DamageType.Physical);

                if (target != null)
                {
                    if (Player.Instance.Distance(target) >= wRange2)
                    {
                        var wPrediction = Program.W.GetPrediction(target);

                        if (wPrediction != null && !wPrediction.Collision && wPrediction.HitChancePercent >= wSlider)
                        {
                            if (wRange && Player.Instance.IsInAutoAttackRange(target))
                            {
                                Program.W.Cast(wPrediction.CastPosition);
                            }
                            else if (!wRange)
                            {
                                Program.W.Cast(wPrediction.CastPosition);
                            }
                        }
                    }
                }
            }

            if (useE && Player.Instance.ManaPercent >= manaE && Program.E.IsReady())
            {
                var target = TargetSelector.GetTarget(Program.E.Range, DamageType.Physical);

                if (target != null)
                {
                    var ePrediction = Program.E.GetPrediction(target);

                    if (ePrediction != null && ePrediction.HitChancePercent >= eSlider)
                    {
                        Program.E.Cast(ePrediction.CastPosition);
                    }
                }
            }

            if (useR && Player.Instance.ManaPercent >= manaR && Program.R.IsReady())
            {
                var rRange = JinXxxMenu.MiscMenu["rRange"].Cast<Slider>().CurrentValue;
                var rRange2 = JinXxxMenu.ComboMenu["rRange2"].Cast<Slider>().CurrentValue;
                var target = TargetSelector.GetTarget(rRange, DamageType.Physical);

                if (target != null)
                {
                    if (Player.Instance.Distance(target) >= rRange2)
                    {
                        var rPrediction = Program.R.GetPrediction(target);

                        if (rPrediction != null && rPrediction.HitChancePercent >= rSlider && EntityManager.Heroes.Enemies.Count(t => t.Distance(rPrediction.CastPosition) <= 100) >= rCountC)
                        {
                            Program.R.Cast(rPrediction.CastPosition);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Does LastHit Method
        /// </summary>
        public static void LastHit()
        {
            var useQ = JinXxxMenu.LastHitMenu["useQ"].Cast<CheckBox>().CurrentValue;
            var manaQ = JinXxxMenu.LastHitMenu["manaQ"].Cast<Slider>().CurrentValue;
            var minions =
                EntityManager.MinionsAndMonsters.GetLaneMinions(
                    EntityManager.UnitTeam.Enemy,
                    Player.Instance.ServerPosition,
                    Essentials.FishBonesRange()).OrderByDescending(t => t.Distance(Player.Instance));

            if (useQ && Program.Q.IsReady() && Essentials.FishBones())
            {
                foreach (
                    var minion in
                        minions.Where(
                            minion =>
                            Player.Instance.Distance(minion) <= Essentials.MinigunRange
                            && Orbwalker.LasthitableMinions.Contains(minion)
                            && Essentials.QModeSelector(minion, JinXxxMenu.LastHitMenu) == "Minigun"))
                {
                    Program.Q.Cast();
                    Orbwalker.ForcedTarget = minion;
                }
            }
            else if (useQ && Program.Q.IsReady() && !Essentials.FishBones() && Player.Instance.ManaPercent >= manaQ)
            {
                foreach (
                    var minion in
                        minions.Where(
                            minion =>
                            !Player.Instance.IsInAutoAttackRange(minion)
                            && Player.Instance.Distance(minion) <= Essentials.FishBonesRange()
                            && Orbwalker.LasthitableMinions.Contains(minion)
                            && Essentials.QModeSelector(minion, JinXxxMenu.LastHitMenu) == "FishBones"))
                {
                    Program.Q.Cast();
                    Orbwalker.ForcedTarget = minion;
                }
            }
        }

        /// <summary>
        /// Does Harass Method
        /// </summary>
        public static void Harass()
        {
            var useQ = JinXxxMenu.HarassMenu["useQ"].Cast<CheckBox>().CurrentValue;
            var useW = JinXxxMenu.HarassMenu["useW"].Cast<CheckBox>().CurrentValue;
            var wRange = JinXxxMenu.MiscMenu["wRange"].Cast<CheckBox>().CurrentValue;
            var wRange2 = JinXxxMenu.HarassMenu["wRange2"].Cast<Slider>().CurrentValue;
            var manaQ = JinXxxMenu.HarassMenu["manaQ"].Cast<Slider>().CurrentValue;
            var manaW = JinXxxMenu.HarassMenu["manaW"].Cast<Slider>().CurrentValue;
            var wSlider = JinXxxMenu.HarassMenu["wSlider"].Cast<Slider>().CurrentValue;
            
            if (useQ && Player.Instance.ManaPercent >= manaQ && Program.Q.IsReady() && !Essentials.FishBones())
            {
                Essentials.MinigunQLogic();
            }

            if (useQ && Program.Q.IsReady() && Essentials.FishBones())
            {
                Essentials.FishbonesQLogic();
            }

            if (useW && Player.Instance.ManaPercent >= manaW && Program.W.IsReady())
            {
                var target = TargetSelector.GetTarget(Program.W.Range, DamageType.Physical);

                if (target != null)
                {
                    if (Player.Instance.Distance(target) >= wRange2)
                    {
                        var wPrediction = Program.W.GetPrediction(target);

                        if (wPrediction.HitChancePercent >= wSlider && !wPrediction.Collision)
                        {
                            if (wRange && Player.Instance.IsInAutoAttackRange(target))
                            {
                                Program.W.Cast(wPrediction.CastPosition);
                            }
                            else if (!wRange)
                            {
                                Program.W.Cast(wPrediction.CastPosition);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Does LaneClear Method
        /// </summary>
        public static void LaneClear()
        {
            var useQ = JinXxxMenu.ComboMenu["useQ"].Cast<CheckBox>().CurrentValue;
            var manaQ = JinXxxMenu.ComboMenu["manaQ"].Cast<Slider>().CurrentValue;

            if (!useQ || !Program.Q.IsReady())
            {
                return;
            }

            var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(
                EntityManager.UnitTeam.Enemy,
                Player.Instance.ServerPosition,
                Essentials.FishBonesRange()).OrderByDescending(t => t.Distance(Player.Instance));

            if (Essentials.FishBones())
            {
                if (Orbwalker.LastTarget.IsStructure())
                {
                    Program.Q.Cast();
                    Orbwalker.ForcedTarget = Orbwalker.LastTarget;
                }

                foreach (
                    var minion in
                        minions.Where(
                            minion =>
                            Player.Instance.Distance(minion) <= Essentials.MinigunRange
                            && Essentials.QModeSelector(minion, JinXxxMenu.LaneClearMenu) == "Minigun"))
                {
                    Program.Q.Cast();
                    Orbwalker.ForcedTarget = minion;
                }
            }

            if (!Essentials.FishBones() && Player.Instance.ManaPercent >= manaQ)
            {
                foreach (
                    var minion in
                        minions.Where(
                            minion =>
                            !Player.Instance.IsInAutoAttackRange(minion)
                            && Player.Instance.Distance(minion) <= Essentials.FishBonesRange()
                            && Essentials.QModeSelector(minion, JinXxxMenu.LaneClearMenu) == "FishBones"))
                {
                    Program.Q.Cast();
                    Orbwalker.ForcedTarget = minion;
                }
            }
        }

        /// <summary>
        /// Does JungleClear Method
        /// </summary>
        public static void JungleClear()
        {
            var useQ = JinXxxMenu.JungleClearMenu["useQ"].Cast<CheckBox>().CurrentValue;
            var useW = JinXxxMenu.JungleClearMenu["useW"].Cast<CheckBox>().CurrentValue;
            var wRange = JinXxxMenu.MiscMenu["wRange"].Cast<CheckBox>().CurrentValue;
            var manaQ = JinXxxMenu.JungleClearMenu["manaQ"].Cast<Slider>().CurrentValue;
            var manaW = JinXxxMenu.JungleClearMenu["manaW"].Cast<Slider>().CurrentValue;
            var wSlider = JinXxxMenu.JungleClearMenu["wSlider"].Cast<Slider>().CurrentValue;

            if (useQ && Player.Instance.ManaPercent >= manaQ && Program.Q.IsReady())
            {
                if (Essentials.FishBones())
                {
                    var mobs = EntityManager.MinionsAndMonsters.GetJungleMonsters(
                        Player.Instance.ServerPosition,
                        Essentials.FishBonesRange());

                    foreach (var mob in mobs)
                    {
                        if (mob == null || !Player.Instance.IsInAutoAttackRange(mob))
                        {
                            return;
                        }

                        if (Player.Instance.Distance(mob) <= Essentials.MinigunRange && Essentials.QModeSelector(mob, JinXxxMenu.JungleClearMenu) == "Minigun")
                        {
                            Program.Q.Cast();
                            Orbwalker.ForcedTarget = mob;
                        }
                    }
                }
                else
                {
                    var mobs = EntityManager.MinionsAndMonsters.GetJungleMonsters(
                        Player.Instance.ServerPosition,
                        Essentials.FishBonesRange());

                    foreach (var mob in mobs)
                    {
                        if (mob == null)
                        {
                            return;
                        }

                        if (!Player.Instance.IsInAutoAttackRange(mob) && Player.Instance.Distance(mob) <= Essentials.FishBonesRange())
                        {
                            Program.Q.Cast();
                            Orbwalker.ForcedTarget = mob;
                        }

                        else if (Player.Instance.Distance(mob) <= Essentials.FishBonesRange() && Essentials.QModeSelector(mob, JinXxxMenu.JungleClearMenu) == "FishBones")
                        {
                            Program.Q.Cast();
                            Orbwalker.ForcedTarget = mob;
                        }
                    }
                }
            }

            if (useW && Player.Instance.ManaPercent >= manaW && Program.W.IsReady())
            {
                var target =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.ServerPosition, Program.W.Range)
                        .OrderByDescending(t => t.Health)
                        .FirstOrDefault();
                if (target != null)
                {
                    var wPrediction = Program.W.GetPrediction(target);

                    if (!(wPrediction.HitChancePercent >= wSlider))
                    {
                        return;
                    }
                    if (wRange && Player.Instance.IsInAutoAttackRange(target))
                    {
                        Program.W.Cast(wPrediction.CastPosition);
                    }
                    else if (!wRange)
                    {
                        Program.W.Cast(wPrediction.CastPosition);
                    }
                }
            }
        }
    }
}