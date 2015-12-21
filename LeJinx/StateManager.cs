namespace Jinx
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
            #region Q Logic

            var useQ = JinXxxMenu.ComboMenu["useQ"].Cast<CheckBox>().CurrentValue;
            var manaQ = JinXxxMenu.ComboMenu["manaQ"].Cast<Slider>().CurrentValue;

            // If the player has a minigun
            if (useQ && Player.Instance.ManaPercent >= manaQ && Program.Q.IsReady() && !Essentials.FishBones())
            {
                var target = TargetSelector.GetTarget(Essentials.FishBonesRange(), DamageType.Physical);

                if (target != null && target.IsValidTarget())
                {
                    if (!Player.Instance.IsInAutoAttackRange(target) &&
                        Player.Instance.Distance(target) <= Essentials.FishBonesRange())
                    {
                        Program.Q.Cast();
                        Orbwalker.ForcedTarget = target;
                    }

                    if (Player.Instance.IsInAutoAttackRange(target) && target.CountEnemiesInRange(100) >= JinXxxMenu.ComboMenu["qCountC"].Cast<Slider>().CurrentValue)
                    {
                        Program.Q.Cast();
                        Orbwalker.ForcedTarget = target;
                    }
                }
            }

            #endregion

            #region Spell Logic

            var useW = JinXxxMenu.ComboMenu["useW"].Cast<CheckBox>().CurrentValue;
            var useE = JinXxxMenu.ComboMenu["useE"].Cast<CheckBox>().CurrentValue;
            var useR = JinXxxMenu.ComboMenu["useR"].Cast<CheckBox>().CurrentValue;
            var wRange = JinXxxMenu.MiscMenu["wRange"].Cast<CheckBox>().CurrentValue;
            var wRange2 = JinXxxMenu.ComboMenu["wRange2"].Cast<Slider>().CurrentValue;
            var eRange = JinXxxMenu.ComboMenu["eRange"].Cast<Slider>().CurrentValue;
            var manaW = JinXxxMenu.ComboMenu["manaW"].Cast<Slider>().CurrentValue;
            var manaE = JinXxxMenu.ComboMenu["manaE"].Cast<Slider>().CurrentValue;
            var manaR = JinXxxMenu.ComboMenu["manaR"].Cast<Slider>().CurrentValue;
            var wSlider = JinXxxMenu.ComboMenu["wSlider"].Cast<Slider>().CurrentValue;
            var eSlider = JinXxxMenu.ComboMenu["eSlider"].Cast<Slider>().CurrentValue;
            var rSlider = JinXxxMenu.ComboMenu["rSlider"].Cast<Slider>().CurrentValue;
            var rCountC = JinXxxMenu.ComboMenu["rCountC"].Cast<Slider>().CurrentValue;

            if (useW && Player.Instance.ManaPercent >= manaW && Program.W.IsReady())
            {
                var target = TargetSelector.GetTarget(Program.W.Range, DamageType.Physical);

                if (target != null && target.IsValidTarget())
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
                    if (Player.Instance.Distance(target) <= eRange)
                    {
                        var ePrediction = Program.E.GetPrediction(target);

                        if (ePrediction != null && ePrediction.HitChancePercent >= eSlider)
                        {
                            Program.E.Cast(ePrediction.CastPosition);
                        }
                    }
                }
            }

            if (useR && Player.Instance.ManaPercent >= manaR && Program.R.IsReady())
            {
                var rRange = JinXxxMenu.MiscMenu["rRange"].Cast<Slider>().CurrentValue;
                var rRange2 = JinXxxMenu.ComboMenu["rRange2"].Cast<Slider>().CurrentValue;
                var target = TargetSelector.GetTarget(rRange2, DamageType.Physical);

                if (target != null)
                {
                    if (Player.Instance.Distance(target) >= rRange)
                    {
                        var rPrediction = Program.R.GetPrediction(target);

                        if (rPrediction != null && rPrediction.HitChancePercent >= rSlider && EntityManager.Heroes.Enemies.Count(t => t.Distance(rPrediction.CastPosition) <= 100) >= rCountC)
                        {
                            Program.R.Cast(rPrediction.CastPosition);
                        }
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// Does LastHit Method
        /// </summary>
        public static void LastHit()
        {
            var useQ = JinXxxMenu.LastHitMenu["useQ"].Cast<CheckBox>().CurrentValue;
            var manaQ = JinXxxMenu.LastHitMenu["manaQ"].Cast<Slider>().CurrentValue;
            var qCountM = JinXxxMenu.LastHitMenu["qCountM"].Cast<Slider>().CurrentValue;

            // Force Minigun if there is a lasthittable minion in minigun range and there is no targets more than the setting amount.
            var kM = Orbwalker.LasthittableMinions.Where(
                t => t.IsEnemy &&
                     t.Health <= (Player.Instance.GetAutoAttackDamage(t) * 0.9) && t.IsValidTarget() &&
                     t.Distance(Player.Instance) <= Essentials.MinigunRange);
            if (useQ && Essentials.FishBones() && kM.Count() < qCountM)
            {
                Program.Q.Cast();
            }

            // Out of Range
            if (useQ && Player.Instance.ManaPercent >= manaQ && !Essentials.FishBones())
            {
                var minionOutOfRange = Orbwalker.LasthittableMinions.FirstOrDefault(
                    m => m.IsValidTarget() && m.Distance(Player.Instance) > Essentials.MinigunRange && m.Distance(Player.Instance) <= Essentials.FishBonesRange());

                if (minionOutOfRange != null)
                {
                    var minion = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,
                        Player.Instance.ServerPosition,
                        Essentials.FishBonesRange())
                        .Where(
                            t =>
                                t.Distance(minionOutOfRange
                                    ) <=
                                100 && t.Health <= (Player.Instance.GetAutoAttackDamage(t) * 1.1f)).ToArray();

                    if (minion.Count() >= qCountM)
                    {
                        foreach (var m in minion)
                        {
                            Program.Q.Cast();
                            Orbwalker.ForcedTarget = m;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Does Harass Method
        /// </summary>
        public static void Harass()
        {
            #region Variables

            var useQ = JinXxxMenu.HarassMenu["useQ"].Cast<CheckBox>().CurrentValue;
            var useW = JinXxxMenu.HarassMenu["useW"].Cast<CheckBox>().CurrentValue;
            var wRange = JinXxxMenu.MiscMenu["wRange"].Cast<CheckBox>().CurrentValue;
            var wRange2 = JinXxxMenu.HarassMenu["wRange2"].Cast<Slider>().CurrentValue;
            var manaQ = JinXxxMenu.HarassMenu["manaQ"].Cast<Slider>().CurrentValue;
            var manaW = JinXxxMenu.HarassMenu["manaW"].Cast<Slider>().CurrentValue;
            var qCountM = JinXxxMenu.HarassMenu["qCountM"].Cast<Slider>().CurrentValue;
            var wSlider = JinXxxMenu.HarassMenu["wSlider"].Cast<Slider>().CurrentValue;

            #endregion

            #region Last Hitting Section

            // Force Minigun if there is a lasthittable minion in minigun range and there is no targets more than the setting amount.
            var kM = Orbwalker.LasthittableMinions.Where(
                t => t.IsEnemy &&
                     t.Health <= (Player.Instance.GetAutoAttackDamage(t)*0.9) && t.IsValidTarget() &&
                     t.Distance(Player.Instance) <= Essentials.MinigunRange);
            if (useQ && Essentials.FishBones() && kM.Count() < qCountM)
            {
                Program.Q.Cast();
            }

            // Out of Range
            if (useQ && Player.Instance.ManaPercent >= manaQ && !Essentials.FishBones())
            {
                var minionOutOfRange = Orbwalker.LasthittableMinions.FirstOrDefault(
                    m => m.IsValidTarget() && m.Distance(Player.Instance) > Essentials.MinigunRange && m.Distance(Player.Instance) <= Essentials.FishBonesRange());

                if (minionOutOfRange != null)
                {
                    var minion = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,
                        Player.Instance.ServerPosition,
                        Essentials.FishBonesRange())
                        .Where(
                            t =>
                                t.Distance(minionOutOfRange
                                    ) <=
                                100 && t.Health <= (Player.Instance.GetAutoAttackDamage(t) * 1.1f)).ToArray();

                    if (minion.Count() >= qCountM)
                    {
                        foreach (var m in minion)
                        {
                            Program.Q.Cast();
                            Orbwalker.ForcedTarget = m;
                        }
                    }
                }
            }

            // In Range
            if (useQ && Player.Instance.ManaPercent >= manaQ && !Essentials.FishBones())
            {
                var minionInRange = Orbwalker.LasthittableMinions.FirstOrDefault(
                    m => m.IsValidTarget() && m.Distance(Player.Instance) <= Essentials.MinigunRange);

                if (minionInRange != null)
                {
                    var minion = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,
                        Player.Instance.ServerPosition,
                        Essentials.FishBonesRange())
                        .Where(
                            t =>
                                t.Distance(minionInRange
                                    ) <=
                                100 && t.Health <= (Player.Instance.GetAutoAttackDamage(t) * 1.1f)).ToArray();

                    if (minion.Count() >= qCountM)
                    {
                        foreach (var m in minion)
                        {
                            Program.Q.Cast();
                            Orbwalker.ForcedTarget = m;
                        }
                    }
                }
            }

            #endregion

            #region Harassing Section

            // If the player has a minigun
            if (useQ && Player.Instance.ManaPercent >= manaQ && Program.Q.IsReady() && !Essentials.FishBones())
            {
                var target = TargetSelector.GetTarget(Essentials.FishBonesRange(), DamageType.Physical);

                if (target != null && target.IsValidTarget())
                {
                    if (!Player.Instance.IsInAutoAttackRange(target) &&
                        Player.Instance.Distance(target) <= Essentials.FishBonesRange())
                    {
                        Program.Q.Cast();
                        Orbwalker.ForcedTarget = target;
                    }

                    if (Player.Instance.IsInAutoAttackRange(target) && target.CountEnemiesInRange(100) >= JinXxxMenu.HarassMenu["qCountC"].Cast<Slider>().CurrentValue)
                    {
                        Program.Q.Cast();
                        Orbwalker.ForcedTarget = target;
                    }
                }
            }

            // If the player has the rocket
            if (useQ && Program.Q.IsReady() && Essentials.FishBones())
            {
                var target = TargetSelector.GetTarget(Essentials.FishBonesRange(), DamageType.Physical);

                if (target != null && target.IsValidTarget())
                {
                    if (Player.Instance.Distance(target) <= Essentials.MinigunRange && target.CountEnemiesInRange(100) < JinXxxMenu.HarassMenu["qCountC"].Cast<Slider>().CurrentValue)
                    {
                        Program.Q.Cast();
                        Orbwalker.ForcedTarget = target;
                    }
                }
            }

            if (useW && Player.Instance.ManaPercent >= manaW && Program.W.IsReady())
            {
                var target = TargetSelector.GetTarget(Program.W.Range, DamageType.Physical);

                if (target != null && target.IsValidTarget())
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

            #endregion
        }

        /// <summary>
        /// Does LaneClear Method
        /// </summary>
        public static void LaneClear()
        {
            var useQ = JinXxxMenu.ComboMenu["useQ"].Cast<CheckBox>().CurrentValue;
            var manaQ = JinXxxMenu.ComboMenu["manaQ"].Cast<Slider>().CurrentValue;

            if (useQ && Program.Q.IsReady())
            {
                var minion = EntityManager.MinionsAndMonsters.GetLaneMinions(
                    EntityManager.UnitTeam.Enemy,
                    Player.Instance.ServerPosition,
                    Essentials.FishBonesRange()).OrderByDescending(t => t.Health);

                if (Essentials.FishBones())
                {
                    if (!minion.Any())
                    {
                        Program.Q.Cast();
                        return;
                    }
                }

                if (!Essentials.FishBones() && Player.Instance.ManaPercent >= manaQ)
                {
                    foreach (var m in minion)
                    {
                        var minionsAoe =
                            EntityManager.MinionsAndMonsters.EnemyMinions.Count(
                                t => t.IsValidTarget() && t.Distance(m) <= 100 && t.Health < (Player.Instance.GetAutoAttackDamage(m) * 1.1f));

                        if (m.Distance(Player.Instance) <= Essentials.FishBonesRange() && m.IsValidTarget() &&
                        minionsAoe >= JinXxxMenu.LaneClearMenu["qCountM"].Cast<Slider>().CurrentValue)
                        {
                            Program.Q.Cast();
                            Orbwalker.ForcedTarget = m;
                        }
                        else if (m.Distance(Player.Instance) >= Essentials.MinigunRange &&
                                 Orbwalker.LasthittableMinions.Contains(m))
                        {
                            Program.Q.Cast();
                            Orbwalker.ForcedTarget = m;
                        }
                    }
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

                    foreach (var mob in mobs.Where(mob => mob != null && Player.Instance.Distance(mob) <= Essentials.MinigunRange))
                    {
                        Program.Q.Cast();
                        Orbwalker.ForcedTarget = mob;
                    }
                }
                else if (!Essentials.FishBones())
                {
                    var mobs = EntityManager.MinionsAndMonsters.GetJungleMonsters(
                        Player.Instance.ServerPosition,
                        Essentials.FishBonesRange());

                    foreach (var mob in mobs.Where(mob => mob != null).Where(mob => !Player.Instance.IsInAutoAttackRange(mob) && Player.Instance.Distance(mob) <= Essentials.FishBonesRange()))
                    {
                        Program.Q.Cast();
                        Orbwalker.ForcedTarget = mob;
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