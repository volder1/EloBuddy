using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace LelBlanc.Modes
{
    internal class Harass
    {
        private static bool UseQ
        {
            get { return Config.HarassMenu["useQ"].Cast<CheckBox>().CurrentValue; }
        }

        private static bool UseW
        {
            get { return Config.HarassMenu["useW"].Cast<CheckBox>().CurrentValue; }
        }

        private static bool UseReturn
        {
            get { return Config.HarassMenu["useReturn"].Cast<CheckBox>().CurrentValue; }
        }

        private static bool UseE
        {
            get { return Config.HarassMenu["useE"].Cast<CheckBox>().CurrentValue; }
        }

        private static bool UseQr
        {
            get { return Config.HarassMenu["useQR"].Cast<CheckBox>().CurrentValue; }
        }

        private static bool UseWr
        {
            get { return Config.HarassMenu["useWR"].Cast<CheckBox>().CurrentValue; }
        }

        private static bool UseReturn2
        {
            get { return Config.HarassMenu["useReturn2"].Cast<CheckBox>().CurrentValue; }
        }


        private static bool UseEr
        {
            get { return Config.HarassMenu["useER"].Cast<CheckBox>().CurrentValue; }
        }

        public static void Execute()
        {
            if (Player.Instance.Spellbook.GetSpell(SpellSlot.R).Level < 1 || (!UseQr && !UseWr && !UseEr))
            {
                Pre6Harass();
            }
            else
            {
                Post6Harass();
            }
        }

        private static void Pre6Harass()
        {
            if (!Program.Q.IsLearned)
            {
                var enemiesBeingE =
                    EntityManager.Heroes.Enemies.Where(t => t.IsValidTarget(Program.E.Range) && Extension.IsBeingE(t));

                if (!enemiesBeingE.Any() && Program.WReturn.IsReady() && UseReturn &&
                    !Program.LastWPosition.IsUnderTurret() &&
                    Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslidereturn")
                {
                    Program.WReturn.Cast();
                }

                var wTarget = TargetSelector.GetTarget(Program.W.Range, DamageType.Magical);

                if (wTarget != null && UseW && !Program.Q.IsLearned && Program.W.IsReady() &&
                    Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslide")
                {
                    Program.W.Cast(wTarget);
                }

                var eTarget = TargetSelector.GetTarget(Program.E.Range, DamageType.Magical);

                if (eTarget != null && UseE && !Program.Q.IsLearned && Program.E.IsReady())
                {
                    Program.E.Cast(eTarget);
                }
            }

            var target = TargetSelector.GetTarget(Program.Q.Range, DamageType.Magical);

            if (target == null)
            {
                var enemiesBeingE =
                    EntityManager.Heroes.Enemies.Where(t => t.IsValidTarget(Program.E.Range) && Extension.IsBeingE(t));

                if (UseReturn && !enemiesBeingE.Any() && Program.WReturn.IsReady() && !Program.LastWPosition.IsUnderTurret() &&
                    Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslidereturn")
                {
                    Program.WReturn.Cast();
                }
                /*if (Program.WReturn.IsReady() && UseReturn && !Program.LastWPosition.IsUnderTurret() && 
                    Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslidereturn")
                {
                    Program.WReturn.Cast();
                }*/
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

            if (UseE && !Program.W.IsReady() && Program.E.IsReady())
            {
                Program.E.Cast(target);
            }

            #region Old Logic

            /*var target = TargetSelector.GetTarget(Program.Q.Range, DamageType.Magical);
            if (target == null)
            {
                var enemiesBeingE =
    EntityManager.Heroes.Enemies.Where(t => t.IsValidTarget(Program.E.Range) && Extension.IsBeingE(t));

                if (UseReturn && !enemiesBeingE.Any() && Program.WReturn.IsReady() &&
                    !Program.LastWPosition.IsUnderTurret() &&
                    Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslidereturn")
                {
                    Program.WReturn.Cast();
                }
                /*if (Program.WReturn.IsReady() && UseReturn && !Program.LastWPosition.IsUnderTurret() &&
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

            if (UseReturn && Program.WReturn.IsReady() && !Program.Q.IsReady() && Extension.IsBeingE(target) &&
                Program.LastWPosition.Distance(target) <= Program.E.Range &&
                Extension.DamageLibrary.CalculateDamage(target, true, true, true, true, Program.Ignite != null) < target.Health &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslidereturn")
            {
                Program.WReturn.Cast();
            }

            if (UseReturn && Program.WReturn.IsReady() && !Extension.IsMarked(target) && !Program.Q.IsReady() && !Program.E.IsReady() &&
                !Extension.IsBeingE(target) &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslidereturn")
            {
                Program.WReturn.Cast();
            }*/

            #endregion
        }

        private static void Post6Harass()
        {
            var target = TargetSelector.GetTarget(Program.Q.Range, DamageType.Magical);

            if (target == null)
            {
                var enemiesBeingE =
                    EntityManager.Heroes.Enemies.Where(t => t.IsValidTarget(Program.E.Range) && Extension.IsBeingE(t));

                if (UseReturn && !enemiesBeingE.Any() && Program.WReturn.IsReady() && !Program.LastWPosition.IsUnderTurret() &&
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

            if (UseQr && Program.QUltimate.IsReady() && Program.QUltimate.IsInRange(target) &&
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

            if (UseWr && !Program.Q.IsReady() && Program.WUltimate.IsReady() &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancslidem")
            {
                Program.WUltimate.Cast(target);
            }

            if (UseE && Program.E.IsReady() && Extension.IsMarked(target))
            {
                Program.E.Cast(target);
            }

            if (UseEr && Program.EUltimate.IsReady() && Extension.IsMarked(target) &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancsoulshacklem")
            {
                Program.EUltimate.Cast(target);
            }

            #region Old Logic

            /*var target = TargetSelector.GetTarget(Program.Q.Range, DamageType.Magical);

            if (target == null)
            {
                if (Program.WReturn.IsReady() && UseReturn && !Program.LastWPosition.IsUnderTurret() &&
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

            if (UseQr && Program.QUltimate.IsReady() && Program.QUltimate.IsInRange(target) &&
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

            if (UseWr && !Program.Q.IsReady() && Program.WUltimate.IsReady() &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancslidem")
            {
                Program.WUltimate.Cast(target);
            }

            if (UseE && Program.E.IsReady() && Extension.IsMarked(target))
            {
                Program.E.Cast(target);
            }

            if (UseEr && Program.EUltimate.IsReady() && Extension.IsMarked(target) &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancsoulshacklem")
            {
                Program.EUltimate.Cast(target);
            }

            /*if (UseReturn && Program.WReturn.IsReady() && !Program.Q.IsReady() && Extension.IsBeingE(target) &&
                Program.LastWPosition.Distance(target) <= Program.E.Range &&
                Extension.DamageLibrary.CalculateDamage(target, true, true, true, true, Program.Ignite != null) < target.Health &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslidereturn")
            {
                Program.WReturn.Cast();
            }

            if (UseReturn && Program.WReturn.IsReady() && !Extension.IsMarked(target) && !Program.Q.IsReady() && !Program.E.IsReady() &&
                !Extension.IsBeingE(target) &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslidereturn")
            {
                Program.WReturn.Cast();
            }*/

            #endregion
        }
    }
}