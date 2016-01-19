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
        public static void SelectCard(Obj_AI_Base t, Cards selectedCard)
        {
            if (t == null)
            {
                return;
            }

            if (selectedCard == Cards.Red)
            {
                CardSelector.StartSelecting(Cards.Red);
            }
            else if (selectedCard == Cards.Yellow)
            {
                CardSelector.StartSelecting(Cards.Yellow);
            }
            else if (selectedCard == Cards.Blue)
            {
                CardSelector.StartSelecting(Cards.Blue);
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

            if (useQ && (Program.Q.IsReady() && Player.Instance.ManaPercent >= manaManagerQ))
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
                if (chooser == "Smart")
                {
                    var selectedCard = Essentials.MinionCardSelection(minion);

                    if (selectedCard != Cards.None)
                    {
                        SelectCard(minion, selectedCard);
                    }
                }
                else if (chooser == "Yellow")
                {
                    SelectCard(minion, Cards.Yellow);
                }
                else if (chooser == "Red")
                {
                    SelectCard(minion, Cards.Red);
                }
                else if (chooser == "Blue")
                {
                    SelectCard(minion, Cards.Blue);
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
                if (Program.Q.IsReady() && Player.Instance.ManaPercent >= manaManagerQ)
                {
                    var minionPrediction = Program.Q.GetPrediction(qMinion);

                    if (minionPrediction.HitChancePercent >= qPred)
                    {
                        Program.Q.Cast(minionPrediction.CastPosition);
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

                if (chooser == "Smart")
                {
                    var selectedCard = Essentials.MinionCardSelection(minion);

                    if (selectedCard != Cards.None)
                    {
                        SelectCard(minion, selectedCard);
                    }
                }
                else if (chooser == "Yellow")
                {
                    SelectCard(minion, Cards.Yellow);
                }
                else if (chooser == "Red")
                {
                    SelectCard(minion, Cards.Red);
                }
                else if (chooser == "Blue")
                {
                    SelectCard(minion, Cards.Blue);
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
                DamageType.Mixed);
            var m = EntityManager.MinionsAndMonsters.GetLaneMinions(
                EntityManager.UnitTeam.Enemy,
                Player.Instance.ServerPosition,
                Player.Instance.AttackRange + wSlider).FirstOrDefault();
            var useCard = Essentials.HarassMenu["useCard"].Cast<CheckBox>().CurrentValue;
            var chooser = Essentials.HarassMenu["chooser"].Cast<Slider>().DisplayName;

            if (useCard && m != null)
            {
                if (chooser == "Smart")
                {
                    var selectedCard = Essentials.MinionCardSelection(m);

                    if (selectedCard != Cards.None)
                    {
                        SelectCard(m, selectedCard);
                    }
                }
                else if (chooser == "Yellow")
                {
                    SelectCard(m, Cards.Yellow);
                }
                else if (chooser == "Red")
                {
                    SelectCard(m, Cards.Red);
                }
                else if (chooser == "Blue")
                {
                    SelectCard(m, Cards.Blue);
                }
            }

            if (useCard && t != null)
            {
                if (chooser == "Smart")
                {
                    var selectedCard = Essentials.HeroCardSelection(t);
                    SelectCard(t, selectedCard);
                }
                else if (chooser == "Yellow")
                {
                    SelectCard(t, Cards.Yellow);
                }
                else if (chooser == "Red")
                {
                    SelectCard(t, Cards.Red);
                }
                else if (chooser == "Blue")
                {
                    SelectCard(t, Cards.Blue);
                }
            }

            var qTarget = TargetSelector.GetTarget(Program.Q.Range, DamageType.Magical);
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
                || !(Player.Instance.ManaPercent >= manaManagerQ))
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
                DamageType.Magical);
            var useCard = Essentials.ComboMenu["useCard"].Cast<CheckBox>().CurrentValue;
            var chooser = Essentials.ComboMenu["chooser"].Cast<Slider>().DisplayName;

            if (useCard && wTarget != null)
            {
                if (chooser == "Smart")
                {
                    var selectedCard = Essentials.HeroCardSelection(wTarget);

                    if (selectedCard != Cards.None)
                    {
                        SelectCard(wTarget, selectedCard);
                    }
                }
                else if (chooser == "Yellow")
                {
                    SelectCard(wTarget, Cards.Yellow);
                }
                else if (chooser == "Red")
                {
                    SelectCard(wTarget, Cards.Red);
                }
                else if (chooser == "Blue")
                {
                    SelectCard(wTarget, Cards.Blue);
                }
            }

            var qTarget = TargetSelector.GetTarget(
                Program.Q.Range,
                DamageType.Magical);
            var useQ = Essentials.ComboMenu["useQ"].Cast<CheckBox>().CurrentValue;

            if (useQ && qTarget != null)
            {
                var useQStun = Essentials.ComboMenu["useQStun"].Cast<CheckBox>().CurrentValue;
                var qPred = Essentials.ComboMenu["qPred"].Cast<Slider>().CurrentValue;
                var manaManagerQ = Essentials.ComboMenu["manaManagerQ"].Cast<Slider>().CurrentValue;

                if (useQStun)
                {
                    if (Program.Q.IsInRange(qTarget) && Program.Q.IsReady() &&
                        Player.Instance.ManaPercent >= manaManagerQ &&
                        qTarget.IsStunned)
                    {
                        var pred = Program.Q.GetPrediction(qTarget);

                        if (pred.HitChancePercent >= qPred)
                        {
                            Program.Q.Cast(pred.CastPosition);
                        }
                    }
                }

                if (!useQStun)
                {
                    if (Program.Q.IsInRange(qTarget) && Program.Q.IsReady() &&
                        Player.Instance.ManaPercent >= manaManagerQ)
                    {
                        var pred = Program.Q.GetPrediction(qTarget);

                        if (pred.HitChancePercent >= qPred)
                        {
                            Program.Q.Cast(pred.CastPosition);
                        }
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
                var target =
                    EntityManager.Heroes.Enemies.Where(
                        t =>
                            Program.Q.IsInRange(t) && t.IsValidTarget() &&
                            t.Health <= DamageLibrary.CalculateDamage(t, true, false, false, false))
                        .OrderByDescending(t => t.Health)
                        .FirstOrDefault();

                if (target != null && Program.Q.IsReady() && Player.Instance.ManaPercent >= manaManagerQ)
                {
                    var pred = Program.Q.GetPrediction(target);

                    if (pred != null && pred.HitChancePercent >= qPred)
                    {
                        Program.Q.Cast(pred.CastPosition);
                    }
                }
            }
        }

        /// <summary>
        /// Does Auto Q
        /// </summary>
        public static void AutoQ()
        {
            var target = EntityManager.Heroes.Enemies.FirstOrDefault(t => t.IsValidTarget() && Program.Q.IsInRange(t) && t.IsStunned);

            if (target == null)
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
