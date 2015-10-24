namespace TwistedBuddy
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
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
                    Program.Q.Range).OrderBy(t => t.Health).FirstOrDefault();
            var useQ = Essentials.LaneClearMenu["useQ"].Cast<CheckBox>().CurrentValue;
            var manaManagerQ = Essentials.LaneClearMenu["manaManagerQ"].Cast<Slider>().CurrentValue;

            if (useQ && qMinion != null)
            {
                if (Program.Q.IsReady() && Essentials.ManaPercent() >= manaManagerQ)
                {
                    var minionPrediction = Prediction.Position.PredictLinearMissile(
                        qMinion,
                        Program.Q.Range,
                        Program.Q.Width,
                        Program.Q.CastDelay,
                        Program.Q.Speed,
                        int.MaxValue,
                        Player.Instance.ServerPosition);

                    if (minionPrediction != null)
                    {
                        if (minionPrediction.HitChance == HitChance.High)
                        {
                            Program.Q.Cast(minionPrediction.CastPosition);
                        }
                    }
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
            var manaManagerQ = Essentials.JungleClearMenu["manaManagerQ"].Cast<Slider>().CurrentValue;

            if (useQ && qMinion != null)
            {
                if (Program.Q.IsReady() && Essentials.ManaPercent() >= manaManagerQ)
                {
                    var minionPrediction = Prediction.Position.PredictLinearMissile(
                        qMinion,
                        Program.Q.Range,
                        Program.Q.Width,
                        Program.Q.CastDelay,
                        Program.Q.Speed,
                        int.MaxValue,
                        Player.Instance.ServerPosition);

                    if (minionPrediction != null)
                    {
                        if (minionPrediction.HitChance == HitChance.High)
                        {
                            Program.Q.Cast(minionPrediction.CastPosition);
                        }
                    }
                }
            }

            var minion =
                EntityManager.MinionsAndMonsters.GetJungleMonsters(
                    Player.Instance.ServerPosition,
                    Program.Q.Range).OrderByDescending(t => t.Health).FirstOrDefault();
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
            var t = TargetSelector.GetTarget(
                Player.Instance.AttackRange + 100,
                DamageType.Mixed,
                Player.Instance.ServerPosition);
            var m = EntityManager.MinionsAndMonsters.GetLaneMinions(
                EntityManager.UnitTeam.Enemy,
                Player.Instance.ServerPosition,
                Player.Instance.AttackRange + 100).FirstOrDefault();
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

            var qTarget = TargetSelector.GetTarget(Program.Q.Range, DamageType.Mixed, Player.Instance.ServerPosition);
            var useQ = Essentials.HarassMenu["useQ"].Cast<CheckBox>().CurrentValue;
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

            if (pred.HitChance == HitChance.High)
            {
                Program.Q.Cast(pred.CastPosition);
            }
        }

        /// <summary>
        /// Does Combo
        /// </summary>
        public static void Combo()
        {
            var wTarget = TargetSelector.GetTarget(
                Player.Instance.AttackRange + 150,
                DamageType.Mixed,
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
                DamageType.Mixed,
                Player.Instance.ServerPosition);
            var useQ = Essentials.ComboMenu["useQ"].Cast<CheckBox>().CurrentValue;

            if (!useQ || qTarget == null)
            {
                return;
            }
            var useQStun = Essentials.ComboMenu["useQStun"].Cast<CheckBox>().CurrentValue;
            var manaManagerQ = Essentials.ComboMenu["manaManagerQ"].Cast<Slider>().CurrentValue;

            if (useQStun)
            {
                if (!Program.Q.IsInRange(qTarget) || !Program.Q.IsReady()
                    || !(Essentials.ManaPercent() >= manaManagerQ) || !qTarget.HasBuff("Stun"))
                {
                    return;
                }
                var pred = Program.Q.GetPrediction(qTarget);

                if (pred.HitChance == HitChance.High)
                {
                    Program.Q.Cast(pred.CastPosition);
                }
            }
            else
            {
                if (!Program.Q.IsInRange(qTarget) || !Program.Q.IsReady()
                    || !(Essentials.ManaPercent() >= manaManagerQ))
                {
                    return;
                }
                var pred = Program.Q.GetPrediction(qTarget);

                if (pred.HitChance == HitChance.High)
                {
                    Program.Q.Cast(pred.CastPosition);
                }
            }
        }

        /// <summary>
        /// Does KillSteal
        /// </summary>
        public static void KillSteal()
        {
            var useQ = Essentials.KillStealMenu["useQ"].Cast<CheckBox>().CurrentValue;
            var manaManagerQ = Essentials.KillStealMenu["manaManagerQ"].Cast<Slider>().CurrentValue;

            if (!useQ)
            {
                return;
            }
            var t = TargetSelector.GetTarget(
                Program.Q.Range,
                DamageType.Mixed,
                Player.Instance.ServerPosition);

            if (t == null || !Program.Q.IsReady())
            {
                return;
            }
            if (!(t.Health < Player.Instance.GetSpellDamage(t, SpellSlot.Q))
                || !(Essentials.ManaPercent() >= manaManagerQ))
            {
                return;
            }
            var pred = Program.Q.GetPrediction(t);

            if (pred != null && pred.HitChance == HitChance.High)
            {
                Program.Q.Cast(pred.CastPosition);
            }
        }

        /// <summary>
        /// Does Auto Q
        /// </summary>
        public static void AutoQ()
        {
            var heroes = EntityManager.Heroes.Enemies.Where(t => t.HasBuff("Stun"));

            foreach (var pred in heroes.Select(t => Program.Q.GetPrediction(t)).Where(pred => pred != null).Where(pred => pred.HitChance == HitChance.High && Program.Q.IsReady()))
            {
                Program.Q.Cast(pred.CastPosition);
            }
        }
    }
}
