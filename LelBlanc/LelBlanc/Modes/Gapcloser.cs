using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace LelBlanc.Modes
{
    internal class Gapcloser
    {
        public static bool UseQ = Config.MiscMenu["useQ"].Cast<CheckBox>().CurrentValue;
        public static bool UseW = Config.MiscMenu["useW"].Cast<CheckBox>().CurrentValue;
        public static bool UseE = Config.MiscMenu["useE"].Cast<CheckBox>().CurrentValue;
        public static bool UseQR = Config.MiscMenu["useQR"].Cast<CheckBox>().CurrentValue;
        public static bool UseWR = Config.MiscMenu["useWR"].Cast<CheckBox>().CurrentValue;
        public static bool UseER = Config.MiscMenu["useER"].Cast<CheckBox>().CurrentValue;

        public static void Execute(AIHeroClient target)
        {
            if (Player.Instance.Spellbook.GetSpell(SpellSlot.R).Level < 1 || (!UseQR && !UseWR && !UseER))
            {
                Pre6Combo(target);
            }
            else
            {
                Post6Combo(target);
            }
        }

        public static void Pre6Combo(AIHeroClient target)
        {
            if (!UseW && !UseQ && !UseE)
            {
                Program.ComboGapCloser = false;
            }

            if (UseW && Program.W.IsReady() && Program.W.IsInRange(target) &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslide")
            {
                Program.W.Cast(target);

                if (!UseQ && !UseE)
                {
                    Program.ComboGapCloser = false;
                }
            }

            if (UseQ && (Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslide" &&
                         !Program.W.IsReady() ||
                         Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslidereturn") &&
                Program.Q.IsReady() && Program.Q.IsInRange(target))
            {
                Program.Q.Cast(target);

                if (!UseW && !UseE)
                {
                    Program.ComboGapCloser = false;
                }
            }

            if (UseE && Program.E.IsReady() && Program.E.IsInRange(target) && Extension.IsMarked(target))
            {
                Program.E.Cast(target);

                if (!UseQ && !UseW)
                {
                    Program.ComboGapCloser = false;
                }
            }

            if (UseW && Program.WReturn.IsReady() && !Program.Q.IsReady() && !Program.E.IsReady() &&
                Program.E.Range <= Program.LastWPosition.Distance(target) &&
                Extension.DamageLibrary.CalculateDamage(target, true, true, true, true) < target.Health &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslidereturn")
            {
                Program.WReturn.Cast();
                Program.ComboGapCloser = false;
                return;
            }

            if (!Program.Q.IsReady() && !Program.W.IsReady() && !Program.E.IsReady())
            {
                Program.ComboGapCloser = false;
            }
        }

        public static void Post6Combo(AIHeroClient target)
        {
            if (UseW && Program.W.IsReady() && Program.W.IsInRange(target) &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslide")
            {
                Program.W.Cast(target);
            }

            if (UseQ && (Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslide" &&
                         !Program.W.IsReady() ||
                         Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslidereturn") &&
                Program.Q.IsReady() && Program.Q.IsInRange(target))
            {
                Program.Q.Cast(target);
            }

            if (UseQR && Program.QUltimate.IsReady() && Program.QUltimate.IsInRange(target) &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancchaosorbm")
            {
                Program.QUltimate.Cast(target);
            }

            if (UseE && Program.E.IsReady() && Program.E.IsInRange(target) && Extension.IsMarked(target))
            {
                Program.E.Cast(target);
            }

            if (UseER && Program.EUltimate.IsReady() && Program.EUltimate.IsInRange(target) &&
                Extension.IsMarked(target) &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancsoulshacklem")
            {
                Program.EUltimate.Cast(target);
            }

            if (UseW && Program.WReturn.IsReady() && !Program.Q.IsReady() && !Program.E.IsReady() &&
                Program.E.Range <= Program.LastWPosition.Distance(target) &&
                Extension.DamageLibrary.CalculateDamage(target, true, true, true, true) < target.Health &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslidereturn")
            {
                Program.WReturn.Cast();
                Program.ComboGapCloser = false;
                return;
            }

            if (!Program.Q.IsReady() && !Program.W.IsReady() && !Program.E.IsReady() && !Program.RReturn.IsReady())
            {
                Program.ComboGapCloser = false;
            }
        }
    }
}