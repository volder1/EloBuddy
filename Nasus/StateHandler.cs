namespace Nasus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Drawing;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using SharpDX;

    internal class StateHandler
    {
        /// <summary>
        /// Does KillSteal
        /// </summary>
        public static void KillSteal()
        {
            var useE = Program.KillStealMenu["useE"].Cast<CheckBox>().CurrentValue;
            
            if (useE)
            {
                var target = HeroManager.Enemies.Where(t => Program.E.IsInRange(t) && t.IsValidTarget()).OrderBy(t => t.Health).FirstOrDefault();

                if (target != null)
                {
                    var pred = Program.E.GetPrediction(target);

                    if (pred != null)
                    {
                        if (Player.Instance.GetSpellDamage(target, SpellSlot.E) >= target.Health)
                        {
                            if (pred.HitChance == HitChance.High)
                            {
                                Program.E.Cast(pred.CastPosition);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Does Harass
        /// </summary>
        public static void Harass()
        {
            var useEH = Program.FarmMenu["useEH"].Cast<CheckBox>().CurrentValue;
            var manaEH = Program.FarmMenu["manaEH"].Cast<Slider>().CurrentValue;

            if (Program.E.IsReady() 
                && useEH 
                && Player.Instance.ManaPercent >= manaEH)
            {
                var target = HeroManager.Enemies.Where(t => Program.E.IsInRange(t) 
                    && t.IsValidTarget());

                if (target != null)
                {
                    foreach (var t in target)
                    {
                        var pred = Program.E.GetPrediction(t);

                        if (pred != null)
                        {
                            if (pred.HitChance == HitChance.High)
                            {
                                Program.E.Cast(pred.CastPosition);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Does LaneClear
        /// </summary>
        public static void LaneClear()
        {
            var useELC = Program.FarmMenu["useELC"].Cast<CheckBox>().CurrentValue;
            var manaELC = Program.FarmMenu["manaELC"].Cast<Slider>().CurrentValue;

            if (Program.E.IsReady()
                && useELC
                && Player.Instance.ManaPercent >= manaELC)
            {
                var minion = ObjectManager.Get<Obj_AI_Base>().Where(t => Program.E.IsInRange(t) && t.IsValidTarget());//EntityManager.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.Position.To2D(), Program.E.Range, true);

                if (minion != null)
                {
                    var pred = Prediction.Position.PredictCircularMissileAoe(minion.ToArray(), Program.E.Range, Program.E.Radius, Program.E.CastDelay, Program.E.Speed, Player.Instance.Position);

                    if (pred != null)
                    {
                        foreach(var p in pred)
                        {
                            Program.E.Cast(p.CastPosition);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Does Combo
        /// </summary>
        public static void Combo()
        {
            var useW = Program.ComboMenu["useW"].Cast<CheckBox>().CurrentValue;
            var manaW = Program.ComboMenu["manaW"].Cast<Slider>().CurrentValue;

            if (Program.W.IsReady()
                && useW
                && Player.Instance.ManaPercent >= manaW)
            {
                var target = HeroManager.Enemies.Where(t => t.IsValidTarget(Program.W.Range)).OrderBy(t => t.Health).FirstOrDefault();
                
                if (target != null)
                {
                    Program.W.Cast(target);
                }
            }

            var useE = Program.ComboMenu["useE"].Cast<CheckBox>().CurrentValue;
            var manaE = Program.ComboMenu["manaE"].Cast<Slider>().CurrentValue;

            if (Program.E.IsReady()
                && useE
                && Player.Instance.ManaPercent >= manaE)
            {
                var target = HeroManager.Enemies.Where(t => Program.E.IsInRange(t)
                    && t.IsValidTarget());

                if (target != null)
                {
                    foreach (var t in target)
                    {
                        var pred = Program.E.GetPrediction(t);

                        if (pred != null)
                        {
                            if (pred.HitChance == HitChance.High)
                            {
                                Program.E.Cast(pred.CastPosition);
                            }
                        }
                    }
                }
            }

            var useR = Program.ComboMenu["useR"].Cast<CheckBox>().CurrentValue;
            var hpR = (float)Program.ComboMenu["hpR"].Cast<Slider>().CurrentValue;
            var rangeR = (float)Program.ComboMenu["rangeR"].Cast<Slider>().CurrentValue;
            var intR = Program.ComboMenu["intR"].Cast<Slider>().CurrentValue;

            if (Program.R.IsReady()
                && useR
                && Player.Instance.HealthPercent <= hpR)
            {
                var enemies = HeroManager.Enemies.Where(t => Player.Instance.Distance(t) <= rangeR).Count();
                
                if (enemies >= intR)
                {
                    Program.R.Cast();
                }
            }
        }
    }
}
