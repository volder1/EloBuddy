using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace LelBlanc.Modes
{
    internal class Combo
    {
        public static bool UseQ = Config.ComboMenu["useQ"].Cast<CheckBox>().CurrentValue;
        public static bool UseW = Config.ComboMenu["useW"].Cast<CheckBox>().CurrentValue;
        public static bool UseReturn = Config.ComboMenu["useReturn"].Cast<CheckBox>().CurrentValue;
        public static bool UseE = Config.ComboMenu["useE"].Cast<CheckBox>().CurrentValue;
        public static bool UseQR = Config.ComboMenu["useQR"].Cast<CheckBox>().CurrentValue;
        public static bool UseWR = Config.ComboMenu["useWR"].Cast<CheckBox>().CurrentValue;
        public static bool UseReturn2 = Config.ComboMenu["useReturn2"].Cast<CheckBox>().CurrentValue;
        public static bool UseER = Config.ComboMenu["useER"].Cast<CheckBox>().CurrentValue;

        public static void Execute()
        {
            if (Player.Instance.Spellbook.GetSpell(SpellSlot.R).Level < 1 || (!UseQR && !UseWR && !UseER))
            {
                Pre6Combo();
            }
            else
            {
                Post6Combo();
            }
        }

        public static void Pre6Combo()
        {
            var target = TargetSelector.GetTarget(700, DamageType.Magical);
            if (target == null)
            {
                if (Program.WReturn.IsReady() && UseReturn &&
                    Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslidereturn")
                {
                    Program.WReturn.Cast();
                }
                return;
            }

            if (UseQ && Program.Q.IsReady() && Program.Q.IsInRange(target))
            {
                Program.Q.Cast(target);
            }

            if (UseW && !Program.Q.IsLearned && !Program.E.IsLearned && Program.W.IsReady() &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslide")
            {
                Program.W.Cast(target);
            }

            if (UseW && !Program.Q.IsReady() && Program.W.IsReady() &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslide" &&
                Extension.IsMarked(target))
            {
                Program.W.Cast(target);
            }

            if (UseE && !Program.W.IsReady() && Program.E.IsReady())
            {
                Program.E.Cast(target);
            }

            if (UseReturn && Program.WReturn.IsReady() && !Program.Q.IsReady() && !Program.E.IsReady() &&
                Program.E.Range <= Program.LastWPosition.Distance(target) &&
                Extension.DamageLibrary.CalculateDamage(target, true, true, true, true) < target.Health &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslidereturn")
            {
                Program.WReturn.Cast();
            }
        }

        public static void Post6Combo()
        {
            var target = TargetSelector.GetTarget(700, DamageType.Magical);

            if (target == null)
            {
                if (Program.WReturn.IsReady() && UseReturn &&
                    Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslidereturn")
                {
                    Program.WReturn.Cast();
                }

                if (Program.RReturn.IsReady() && UseReturn2 &&
                    Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancslidereturnm")
                {
                    Program.RReturn.Cast();
                }

                return;
            }

            if (UseQ && Program.Q.IsReady() && Program.Q.IsInRange(target))
            {
                Program.Q.Cast(target);
            }

            if (UseQR && Program.QUltimate.IsReady() && Program.QUltimate.IsInRange(target) &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancchaosorbm")
            {
                Program.QUltimate.Cast(target);
            }

            if (UseW && !Program.Q.IsLearned && !Program.E.IsLearned && Program.W.IsReady() &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslide")
            {
                Program.W.Cast(target);
            }

            if (UseW && !Program.Q.IsReady() && !Program.QUltimate.IsReady() && Program.W.IsReady() &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslide" &&
                Extension.IsMarked(target))
            {
                Program.W.Cast(target);
            }

            if (UseWR && !Program.Q.IsReady() && Program.WUltimate.IsReady() &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancslidem")
            {
                Program.WUltimate.Cast(target);
            }

            if (UseE && Program.E.IsReady() && Extension.IsMarked(target))
            {
                Program.E.Cast(target);
            }

            if (UseER && Program.EUltimate.IsReady() && Extension.IsMarked(target) &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancsoulshacklem")
            {
                Program.EUltimate.Cast(target);
            }

            if (UseReturn && Program.WReturn.IsReady() && !Program.Q.IsReady() && !Program.E.IsReady() &&
                Program.E.Range <= Program.LastWPosition.Distance(target) &&
                Extension.DamageLibrary.CalculateDamage(target, true, true, true, true) < target.Health &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslidereturn")
            {
                Program.WReturn.Cast();
            }
        }
    }
}