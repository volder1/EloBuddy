using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace LelBlanc.Modes
{
    internal class Combo
    {
        #region Properties

        private static bool UseQ => Config.ComboMenu["useQ"].Cast<CheckBox>().CurrentValue;

        private static bool UseW => Config.ComboMenu["useW"].Cast<CheckBox>().CurrentValue;

        private static bool UseReturn => Config.ComboMenu["useReturn"].Cast<CheckBox>().CurrentValue;

        private static bool UseE => Config.ComboMenu["useE"].Cast<CheckBox>().CurrentValue;

        private static bool UseQr => Config.ComboMenu["useQR"].Cast<CheckBox>().CurrentValue;

        private static bool UseWr => Config.ComboMenu["useWR"].Cast<CheckBox>().CurrentValue;

        private static bool UseReturn2 => Config.ComboMenu["useReturn2"].Cast<CheckBox>().CurrentValue;

        private static bool UseEr => Config.ComboMenu["useER"].Cast<CheckBox>().CurrentValue;

        private static bool MinimumRange => Config.ComboMenu["minRange"].Cast<CheckBox>().CurrentValue;

        #endregion

        #region Methods

        /// <summary>
        /// Executes the Combo
        /// </summary>
        public static void Execute()
        {
            if (UseReturn)
            {
                Extension.LogicReturn();
            }
            if (UseReturn2)
            {
                Extension.LogicReturn(true);
            }
            if (Player.Instance.Spellbook.GetSpell(SpellSlot.R).Level < 1 || (!UseQr && !UseWr && !UseEr))
            {
                Pre6Combo();
            }
            else
            {
                Post6Combo();
            }
        }

        /// <summary>
        /// Pre Level 6 Combo
        /// </summary>
        private static void Pre6Combo()
        {
            var enemiesBeingE =
                EntityManager.Heroes.Enemies.Where(t => t.IsValidTarget(Program.E.Range) && Extension.IsBeingE(t))
                    .ToArray();

            if (!Program.Q.IsLearned)
            {
                if (!enemiesBeingE.Any() && Program.WReturn.IsReady() && UseReturn &&
                    Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslidereturn")
                {
                    Program.WReturn.Cast();
                }

                var wTarget = TargetSelector.SelectedTarget ??
                              TargetSelector.GetTarget(Program.W.Range, DamageType.Magical);

                if (wTarget != null && UseW && !Program.Q.IsLearned && Program.W.IsReady() &&
                    Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslide")
                {
                    Program.W.Cast(wTarget);
                }

                var eTarget = TargetSelector.SelectedTarget ??
                              TargetSelector.GetTarget(Program.E.Range, DamageType.Magical);

                if (eTarget != null && UseE && !Program.Q.IsLearned && Program.E.IsReady())
                {
                    Program.E.Cast(eTarget);
                }
            }

            var target = TargetSelector.SelectedTarget ?? TargetSelector.GetTarget(Program.Q.Range, DamageType.Magical);

            if (target == null)
            {
                return;
            }

            if (UseQ && Program.Q.IsReady() && Program.Q.IsInRange(target))
            {
                Program.Q.Cast(target);
            }

            if (UseW && !Program.Q.IsReady() && Program.W.IsReady() &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslide" &&
                Extension.IsMarked(target))
            {
                Program.W.Cast(target);
            }

            if (UseE &&
                (Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslidereturn" ||
                 !Program.W.IsReady()) && Program.E.IsReady() && Program.E.IsInRange(target))
            {
                Program.E.Cast(target);
            }
        }

        private static void Post6Combo()
        {
            switch (Config.ComboMenu["mode"].Cast<ComboBox>().SelectedIndex)
            {
                // Default Logic    
                case 0:
                    DoubleQLogic();
                    break;
                // Double E Logic
                case 1:
                    DoubleELogic();
                    break;
                // W -> Q -> R -> E
                case 2:
                    ChaseBurst();
                    break;
            }
        }

        private static void DoubleQLogic()
        {
            var range = MinimumRange ? Program.W.Range : Program.Q.Range;
            var target = TargetSelector.SelectedTarget ?? TargetSelector.GetTarget(range, DamageType.Magical);

            if (Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslidereturn" &&
                !Program.W.IsReady())
            {
                target = TargetSelector.SelectedTarget ??
                         TargetSelector.GetTarget(Program.E.Range, DamageType.Magical);

                if (target == null)
                {
                    return;
                }

                if (UseE && Program.E.IsReady() && Program.E.IsInRange(target))
                {
                    Program.E.Cast(target);
                }
            }
            else if (!Program.Q.IsReady() && !Program.QUltimate.IsReady())
            {
                if (target == null)
                {
                    return;
                }

                if (UseW && Program.W.IsReady() &&
                    Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslide" &&
                    Extension.IsMarked(target))
                {
                    Program.W.Cast(target);
                }
            }
            else
            {
                if (target == null)
                {
                    return;
                }

                if (UseQ && Program.Q.IsReady() && Program.Q.IsInRange(target))
                {
                    Program.Q.Cast(target);
                }

                if (UseQr && Program.QUltimate.IsReady() && Program.QUltimate.IsInRange(target) &&
                    Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancchaosorbm")
                {
                    Program.QUltimate.Cast(target);
                }
            }
        }

        private static void ChaseBurst()
        {
            var target = TargetSelector.SelectedTarget ?? TargetSelector.GetTarget(Program.W.Range, DamageType.Magical);

            if (!Program.Q.IsReady() && !Program.QUltimate.IsReady() &&
                (Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslidereturn" ||
                 !Program.W.IsReady()))
            {
                target = TargetSelector.SelectedTarget ??
                         TargetSelector.GetTarget(Program.E.Range, DamageType.Magical);

                if (target == null)
                {
                    return;
                }

                if (UseE && Program.E.IsReady() && Program.E.IsInRange(target))
                {
                    Program.E.Cast(target);
                }
            }
            else if (Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslidereturn" ||
                     !Program.W.IsReady())
            {
                if (target == null)
                {
                    target = TargetSelector.SelectedTarget ??
                             TargetSelector.GetTarget(Program.Q.Range, DamageType.Magical);
                }

                if (target == null)
                {
                    return;
                }

                if (UseQ && Program.Q.IsReady() && Program.Q.IsInRange(target))
                {
                    Program.Q.Cast(target);
                }

                if (UseQr && !Program.Q.IsReady() && Program.QUltimate.IsReady() &&
                    Program.QUltimate.IsInRange(target) &&
                    Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancchaosorbm")
                {
                    Program.QUltimate.Cast(target);
                }
            }
            else
            {
                if (target == null)
                {
                    return;
                }

                if (UseW && Program.W.IsReady() && target.IsValidTarget(Program.W.Range) &&
                    Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslide")
                {
                    Program.W.Cast(target);
                }
            }
        }

        private static void DoubleELogic()
        {
            var target = TargetSelector.SelectedTarget ?? TargetSelector.GetTarget(Program.E.Range, DamageType.Magical);

            if (target == null)
            {
                return;
            }

            if (UseQ && Program.Q.IsReady() && Program.Q.IsInRange(target))
            {
                Program.Q.Cast(target);
            }

            if (UseW && !Program.Q.IsReady() && Program.W.IsReady() &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslide" &&
                Extension.IsMarked(target))
            {
                Program.W.Cast(target);
            }

            if (UseW && !Program.Q.IsLearned && !Program.E.IsLearned && Program.W.IsReady() &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslide")
            {
                Program.W.Cast(target);
            }

            if (UseE && !Program.W.IsReady() && Program.E.IsReady() && Program.E.IsInRange(target))
            {
                Program.E.Cast(target);
            }

            if (UseEr && !Program.E.IsReady() && Program.EUltimate.IsReady() && Extension.IsBeingE(target) &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancsoulshacklem")
            {
                Program.EUltimate.Cast(target);
            }
        }

        #endregion
    }
}