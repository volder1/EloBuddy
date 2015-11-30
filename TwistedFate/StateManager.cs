namespace TwistedBuddy
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;

    internal class StateManager
    {
        /// <summary>
        /// Selects a Card
        /// </summary>
        /// <param name="t">The Target</param>
        /// <param name="selectedCard">The Card that is selected.</param>
        public static void SelectCard(Obj_AI_Base t, string selectedCard)
        {
            if (t == null)
            {
                return;
            }

            switch (selectedCard)
            {
                case "Red":
                    CardSelector.StartSelecting(Cards.Red);
                    break;
                case "Yellow":
                    CardSelector.StartSelecting(Cards.Yellow);
                    break;
                case "Blue":
                    CardSelector.StartSelecting(Cards.Blue);
                    break;
                default:
                    return;
            }
        }

        /// <summary>
        /// Does LaneClear
        /// </summary>
        public static void LaneClear()
        {
            var qMinion =
                EntityManager.MinionsAndMonsters.GetLaneMinions(
                    EntityManager.UnitTeam.Enemy,
                    Player.Instance.ServerPosition,
                    Program.Q.Range).OrderBy(t => t.Health);
            var useQ = Essentials.LaneClearMenu["useQ"].Cast<CheckBox>().CurrentValue;
            var qPred = Essentials.LaneClearMenu["qPred"].Cast<Slider>().CurrentValue;
            var manaManagerQ = Essentials.LaneClearMenu["manaManagerQ"].Cast<Slider>().CurrentValue;

            if (useQ && (Program.Q.IsReady() && Essentials.ManaPercent() >= manaManagerQ))
            {
                var minionPrediction = EntityManager.MinionsAndMonsters.GetLineFarmLocation(
                    qMinion,
                    Program.Q.Width,
                    (int)Program.Q.Range);

                if (minionPrediction.HitNumber >= qPred)
                {
                    Program.Q.Cast(minionPrediction.CastPosition);
                }
            }

            var minion =
                EntityManager.MinionsAndMonsters.GetLaneMinions(
                    EntityManager.UnitTeam.Enemy,
                    Player.Instance.ServerPosition,
                    Player.Instance.AttackRange + 100).FirstOrDefault();
            var useCard = Essentials.LaneClearMenu["useCard"].Cast<CheckBox>().CurrentValue;
            var chooser = Essentials.LaneClearMenu["chooser"].Cast<Slider>().DisplayName;

            if (useCard && minion != null)
            {
                switch (chooser)
                {
                    case "Smart":
                        var selectedCard = Essentials.MinionCardSelection(minion);
                        SelectCard(minion, selectedCard);
                        break;
                    default:
                        SelectCard(minion, chooser);
                        break;
                }
            }
        }

        /// <summary>
        /// Does JungleSteal
        /// </summary>
        public static void JungleClear()
        {
            var qMinion =
                EntityManager.MinionsAndMonsters.GetJungleMonsters(
                    Player.Instance.ServerPosition,
                    Program.Q.Range,
                    false).OrderByDescending(t => t.Health).FirstOrDefault();
            var useQ = Essentials.JungleClearMenu["useQ"].Cast<CheckBox>().CurrentValue;
            var qPred = Essentials.JungleClearMenu["qPred"].Cast<Slider>().CurrentValue;
            var manaManagerQ = Essentials.JungleClearMenu["manaManagerQ"].Cast<Slider>().CurrentValue;

            if (useQ && qMinion != null)
            {
                if (Program.Q.IsReady() && Essentials.ManaPercent() >= manaManagerQ)
                {
                    var minionPrediction = Program.Q.GetPrediction(qMinion);

                    if (minionPrediction != null)
                    {
                        if (minionPrediction.HitChancePercent >= qPred)
                        {
                            Program.Q.Cast(minionPrediction.CastPosition);
                        }
                    }
                }
            }

            var minion = EntityManager.MinionsAndMonsters.GetJungleMonsters(
                Player.Instance.ServerPosition,
                Program.Q.Range).FirstOrDefault();
            var useCard = Essentials.JungleClearMenu["useCard"].Cast<CheckBox>().CurrentValue;

            if (useCard && minion != null)
            {
                var chooser = Essentials.JungleClearMenu["chooser"].Cast<Slider>().DisplayName;

                switch (chooser)
                {
                    case "Smart":
                        var selectedCard = Essentials.MinionCardSelection(minion);
                        SelectCard(minion, selectedCard);
                        break;
                    default:
                        SelectCard(minion, chooser);
                        break;
                }
            }
        }

        /// <summary>
        /// Does Harass
        /// </summary>
        public static void Harass()
        {
            var wSlider = Essentials.HarassMenu["wSlider"].Cast<Slider>().CurrentValue;
            var t = TargetSelector.GetTarget(
                Player.Instance.AttackRange + wSlider,
                DamageType.Mixed,
                Player.Instance.ServerPosition);
            var m = EntityManager.MinionsAndMonsters.GetLaneMinions(
                EntityManager.UnitTeam.Enemy,
                Player.Instance.ServerPosition,
                Player.Instance.AttackRange + wSlider).FirstOrDefault();
            var useCard = Essentials.HarassMenu["useCard"].Cast<CheckBox>().CurrentValue;
            var chooser = Essentials.HarassMenu["chooser"].Cast<Slider>().DisplayName;

            if (useCard && m != null)
            {
                switch (chooser)
                {
                    case "Smart":
                        var selectedCard = Essentials.MinionCardSelection(m);
                        SelectCard(m, selectedCard);
                        break;
                    default:
                        SelectCard(m, chooser);
                        break;
                }
            }

            if (useCard && t != null)
            {
                if (t.Health <= ObjectManager.Player.GetAutoAttackDamage(t) + Player.Instance.GetSpellDamage(t, SpellSlot.W))
                {
                    switch (chooser)
                    {
                        case "Smart":
                            var selectedCard = Essentials.HeroCardSelection(t);
                            SelectCard(t, selectedCard);
                            break;
                        default:
                            SelectCard(t, chooser);
                            break;
                    }
                }
            }

            var qTarget = TargetSelector.GetTarget(Program.Q.Range, DamageType.Magical, Player.Instance.ServerPosition);
            var useQ = Essentials.HarassMenu["useQ"].Cast<CheckBox>().CurrentValue;
            var qPred = Essentials.HarassMenu["qPred"].Cast<Slider>().CurrentValue;
            var manaManagerQ = Essentials.HarassMenu["manaManagerQ"].Cast<Slider>().CurrentValue;

            if (!useQ)
            {
                return;
            }

            if (qTarget == null)
            {
                return;
            }

            if (!Program.Q.IsInRange(qTarget) || !Program.Q.IsReady()
                || !(Essentials.ManaPercent() >= manaManagerQ))
            {
                return;
            }
            var pred = Program.Q.GetPrediction(qTarget);

            if (pred.HitChancePercent >= qPred)
            {
                Program.Q.Cast(pred.CastPosition);
            }
        }

        /// <summary>
        /// Does Combo
        /// </summary>
        public static void Combo()
        {
            var wSlider = Essentials.ComboMenu["wSlider"].Cast<Slider>().CurrentValue;
            var wTarget = TargetSelector.GetTarget(
                Player.Instance.AttackRange + wSlider,
                DamageType.Magical,
                Player.Instance.ServerPosition);
            var useCard = Essentials.ComboMenu["useCard"].Cast<CheckBox>().CurrentValue;
            var chooser = Essentials.ComboMenu["chooser"].Cast<Slider>().DisplayName;

            if (useCard && wTarget != null)
            {
                switch (chooser)
                {
                    case "Smart":
                        var selectedCard = Essentials.HeroCardSelection(wTarget);
                        SelectCard(wTarget, selectedCard);
                        break;
                    default:
                        SelectCard(wTarget, chooser);
                        break;
                }
            }

            var qTarget = TargetSelector.GetTarget(
                Program.Q.Range,
                DamageType.Magical,
                Player.Instance.ServerPosition);
            var useQ = Essentials.ComboMenu["useQ"].Cast<CheckBox>().CurrentValue;

            if (!useQ || qTarget == null)
            {
                return;
            }
            var useQStun = Essentials.ComboMenu["useQStun"].Cast<CheckBox>().CurrentValue;
            var qPred = Essentials.ComboMenu["qPred"].Cast<Slider>().CurrentValue;
            var manaManagerQ = Essentials.ComboMenu["manaManagerQ"].Cast<Slider>().CurrentValue;

            if (useQStun)
            {
                if (!Program.Q.IsInRange(qTarget) || !Program.Q.IsReady() || !(Essentials.ManaPercent() >= manaManagerQ)
                    || !qTarget.IsStunned)
                {
                    return;
                }
                var pred = Program.Q.GetPrediction(qTarget);

                if (pred.HitChancePercent >= qPred)
                {
                    Program.Q.Cast(pred.CastPosition);
                }
            }
            else
            {
                if (Program.Q.IsInRange(qTarget) && Program.Q.IsReady() && Essentials.ManaPercent() >= manaManagerQ)
                {
                    var pred = Program.Q.GetPrediction(qTarget);

                    if (pred.HitChancePercent >= qPred)
                    {
                        Program.Q.Cast(pred.CastPosition);
                    }
                }
            }
        }

        /// <summary>
        /// Does KillSteal
        /// </summary>
        public static void KillSteal()
        {
            var useQ = Essentials.KillStealMenu["useQ"].Cast<CheckBox>().CurrentValue;
            var qPred = Essentials.KillStealMenu["qPred"].Cast<Slider>().CurrentValue;
            var manaManagerQ = Essentials.KillStealMenu["manaManagerQ"].Cast<Slider>().CurrentValue;

            if (useQ)
            {
                var t = TargetSelector.GetTarget(Program.Q.Range, DamageType.Magical, Player.Instance.ServerPosition);

                if (t != null && Program.Q.IsReady())
                {
                    if (t.Health < DamageLibrary.CalculateDamage(t, true, false, false, false)
                        && Essentials.ManaPercent() >= manaManagerQ)
                    {
                        var pred = Program.Q.GetPrediction(t);

                        if (pred != null && pred.HitChancePercent >= qPred)
                        {
                            Program.Q.Cast(pred.CastPosition);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Does Auto Q
        /// </summary>
        public static void AutoQ()
        {
            var target = EntityManager.Heroes.Enemies.FirstOrDefault(t => Program.Q.IsInRange(t));

            if (target == null)
            {
                return;
            }

            if (!target.IsStunned || !target.IsRooted || !target.IsTaunted)
            {
                return;
            }

            var pred = Program.Q.GetPrediction(target);

            if (pred.HitChancePercent >= 75)
            {
                Program.Q.Cast(pred.CastPosition);
            }
        }
    }
}
