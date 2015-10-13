namespace Nasus
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Menu.Values;

    internal class StateHandler
    {
        /// <summary>
        /// Does KillSteal
        /// </summary>
        public static void KillSteal()
        {
            var useE = Program.KillStealMenu["useE"].Cast<CheckBox>().CurrentValue;

            if (!useE)
            {
                return;
            }
            var target = EntityManager.Heroes.Enemies.Where(t => Program.E.IsInRange(t) && t.IsValidTarget()).OrderBy(t => t.Health).FirstOrDefault();

            if (target == null)
            {
                return;
            }
            var pred = Program.E.GetPrediction(target);

            if (pred == null)
            {
                return;
            }
            if (
                !(Player.Instance.GetSpellDamage(target, SpellSlot.E)
                  >= target.Health))
            {
                return;
            }
            if (pred.HitChance == HitChance.High)
            {
                Program.E.Cast(pred.CastPosition);
            }
        }

        /// <summary>
        /// Does LastHit
        /// </summary>
        public static void LastHit()
        {
            var minion = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.ServerPosition, Program.Q.Range).FirstOrDefault();

            if (minion == null)
            {
                return;
            }
            if (!Program.Q.IsReady() || !Program.Q.IsInRange(minion) || !(Program.GetDamage(minion) >= minion.Health))
            {
                return;
            }
            Program.Q.Cast();
            Orbwalker.DisableAttacking = true;
            Orbwalker.DisableMovement = true;
            Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
            Orbwalker.DisableAttacking = false;
            Orbwalker.DisableMovement = false;
        }

        /// <summary>
        /// Does Harass
        /// </summary>
        public static void Harass()
        {
            var useEH = Program.FarmMenu["useEH"].Cast<CheckBox>().CurrentValue;
            var manaEH = Program.FarmMenu["manaEH"].Cast<Slider>().CurrentValue;

            if (!Program.E.IsReady() || !useEH || !(Player.Instance.ManaPercent >= manaEH))
            {
                return;
            }
            var target = EntityManager.Heroes.Enemies.Where(t => Program.E.IsInRange(t) && t.IsValidTarget());

            foreach (var pred in target.Select(t => Program.E.GetPrediction(t)).Where(pred => pred != null).Where(pred => pred.HitChance == HitChance.High))
            {
                Program.E.Cast(pred.CastPosition);
            }
        }

        /// <summary>
        /// Does LaneClear
        /// </summary>
        public static void LaneClear()
        {
            var useELC = Program.FarmMenu["useELC"].Cast<CheckBox>().CurrentValue;
            var manaELC = Program.FarmMenu["manaELC"].Cast<Slider>().CurrentValue;

            if (!Program.E.IsReady() || !useELC || !(Player.Instance.ManaPercent >= manaELC))
            {
                return;
            }
            var minion = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => Program.E.IsInRange(t) && t.IsValidTarget()).ToArray();

            var pred = Prediction.Position.PredictCircularMissileAoe(minion, Program.E.Range, Program.E.Radius, Program.E.CastDelay, Program.E.Speed, Player.Instance.ServerPosition);

            if (pred == null)
            {
                return;
            }
            foreach(var p in pred)
            {
                Program.E.Cast(p.CastPosition);
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
                var target = EntityManager.Heroes.Enemies.Where(t => t.IsValidTarget(Program.W.Range)).OrderBy(t => t.Health).FirstOrDefault();
                
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
                var target = EntityManager.Heroes.Enemies.Where(t => Program.E.IsInRange(t)
                    && t.IsValidTarget());

                foreach (var pred in target.Select(t => Program.E.GetPrediction(t)).Where(pred => pred != null).Where(pred => pred.HitChance == HitChance.High))
                {
                    Program.E.Cast(pred.CastPosition);
                }
            }

            var useR = Program.ComboMenu["useR"].Cast<CheckBox>().CurrentValue;
            var hpR = (float)Program.ComboMenu["hpR"].Cast<Slider>().CurrentValue;
            var rangeR = (float)Program.ComboMenu["rangeR"].Cast<Slider>().CurrentValue;
            var intR = Program.ComboMenu["intR"].Cast<Slider>().CurrentValue;

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
