using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

namespace LelBlanc.Modes
{
    class KillSteal
    {
        public static bool ResetW = false;

        public static void Execute()
        {
            if (Player.Instance.IsUnderTurret()) return;

            var killableEnemies =
                EntityManager.Heroes.Enemies.Where(
                    t =>
                        t.IsValidTarget() && !t.HasUndyingBuff() &&
                        Extension.DamageLibrary.CalculateDamage(t, Program.Q.IsReady(), Program.W.IsReady(),
                            Program.E.IsReady(), Program.RReturn.IsReady()) >= t.Health);
            var target = TargetSelector.GetTarget(killableEnemies, DamageType.Magical);

            if (target == null) return;

            if (target.Health <= Extension.DamageLibrary.CalculateDamage(target, true, false, false, false))
            {
                CastQ(target);    
            }

            else if (target.Health <= Extension.DamageLibrary.CalculateDamage(target, false, true, false, false))
            {
                CastW(target, true);
            }

            else if (target.Health <= Extension.DamageLibrary.CalculateDamage(target, false, false, true, false))
            {
                CastE(target);
            }

            else if (target.Health <= Extension.DamageLibrary.CalculateDamage(target, false, false, false, true))
            {
                CastR(target, true);
            }

            else if (target.Health <= Extension.DamageLibrary.CalculateDamage(target, true, true, false, false))
            {
                if (!Program.Q.IsReady() || !Program.W.IsReady()) return;

                CastQ(target);
                Core.DelayAction(() =>
                {
                    if (!target.IsDead &&
                        Extension.DamageLibrary.CalculateDamage(target, false, true, false, false) >= target.Health)
                    {
                        CastW(target, true);
                    }
                }, Program.Q.CastDelay);
            }

            else if (target.Health <= Extension.DamageLibrary.CalculateDamage(target, true, false, false, true))
            {
                if (!Program.Q.IsReady() || !Program.RReturn.IsReady()) return;

                CastQ(target);
                Core.DelayAction(() =>
                {
                    if (!target.IsDead)
                        CastR(target, true);
                }, Program.Q.CastDelay);
            }

            else
            {
                if (!Program.Q.IsReady() || !Program.W.IsReady() || !Program.E.IsReady() || !Program.QUltimate.IsReady()) return;

                CastQ(target);
                Core.DelayAction(() =>
                {
                    if (!target.IsDead && Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancchaosorbm")
                        CastR(target, false);
                }, Program.Q.CastDelay);
                Core.DelayAction(() =>
                {
                    if (!target.IsDead && Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancslide")
                        CastW(target, false);
                }, Program.QUltimate.CastDelay);
                Core.DelayAction(() =>
                {
                    if (!target.IsDead)
                        CastE(target);
                }, Program.W.CastDelay);

            }
        }

        private static void CastQ(AIHeroClient target)
        {
            if (!Program.Q.IsReady()) return;

            if (Program.Q.IsInRange(target))
            {
                Program.Q.Cast(target);
            }
        }


        private static void CastW(AIHeroClient target, bool useWReturn)
        {
            if (!Program.W.IsReady()) return;

            if (Program.W.IsInRange(target) &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslide")
            {
                Program.W.Cast(target);
                ResetW = useWReturn;
            }
        }

        private static void CastE(AIHeroClient target)
        {
            if (!Program.E.IsReady()) return;

            if (Program.E.IsInRange(target))
            {
                Program.E.Cast(target);
            }
        }


        private static void CastR(AIHeroClient target, bool useWReturn)
        {
            if (!Program.RReturn.IsReady())
            {
                return;
            }

            // Q
            if (Program.QUltimate.IsInRange(target) && Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancchaosorbm")
            {
                Program.QUltimate.Cast(target);
            }

            // W
            if (Program.WUltimate.IsInRange(target) && Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancslidem")
            {
                Program.WUltimate.Cast(target);
                ResetW = useWReturn;
            }

            // E
            if (Program.EUltimate.IsInRange(target) && Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancsoulshacklem")
            {
                Program.EUltimate.Cast(target);
            }
        }
    }
}
