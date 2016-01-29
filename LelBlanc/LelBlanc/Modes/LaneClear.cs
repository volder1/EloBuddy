using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace LelBlanc.Modes
{
    internal class LaneClear
    {
        public static bool UseQ = Config.LaneClearMenu["useQ"].Cast<CheckBox>().CurrentValue;
        public static bool UseW = Config.LaneClearMenu["useW"].Cast<CheckBox>().CurrentValue;
        public static int SliderW = Config.LaneClearMenu["sliderW"].Cast<Slider>().CurrentValue;
        public static bool UseQR = Config.LaneClearMenu["useQR"].Cast<CheckBox>().CurrentValue;
        public static bool UseWR = Config.LaneClearMenu["useWR"].Cast<CheckBox>().CurrentValue;
        public static int SliderWR = Config.LaneClearMenu["sliderWR"].Cast<Slider>().CurrentValue;

        public static void Execute()
        {
            if (Player.Instance.Spellbook.GetSpell(SpellSlot.R).Level < 1)
            {
                Pre6LaneClear();
            }
            else
            {
                Post6LaneClear();
            }
        }

        public static void Pre6LaneClear()
        {
            if (UseQ && Program.Q.IsReady())
            {
                var minion = EntityManager.MinionsAndMonsters.Get(EntityManager.MinionsAndMonsters.EntityType.Minion,
                    EntityManager.UnitTeam.Enemy,
                    Player.Instance.ServerPosition, Program.Q.Range)
                    .FirstOrDefault(
                        t => Extension.DamageLibrary.CalculateDamage(t, true, false, false, false) >= t.Health);

                if (minion != null)
                {
                    Program.Q.Cast(minion);
                }
            }
            if (UseW && Program.W.IsReady() &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslide")
            {
                var minion = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,
                    Player.Instance.ServerPosition, Program.W.Range)
                    .Where(t => Extension.DamageLibrary.CalculateDamage(t, false, true, false, false) >= t.Health);
                var wAOE = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(minion, Program.W.Width,
                    (int) Program.W.Range);

                if (wAOE.HitNumber > SliderW)
                {
                    Program.W.Cast(wAOE.CastPosition);
                }
            }
            if (UseW && Program.WReturn.IsReady() &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslidereturn")
            {
                Program.WReturn.Cast();
            }
        }

        public static void Post6LaneClear()
        {
            if (UseQ && Program.Q.IsReady())
            {
                var minion = EntityManager.MinionsAndMonsters.Get(EntityManager.MinionsAndMonsters.EntityType.Minion,
                    EntityManager.UnitTeam.Enemy,
                    Player.Instance.ServerPosition, Program.Q.Range)
                    .FirstOrDefault(
                        t => Extension.DamageLibrary.CalculateDamage(t, true, false, false, false) >= t.Health);

                if (minion != null)
                {
                    Program.Q.Cast(minion);
                }
            }
            if (UseQR && Program.QUltimate.IsReady() &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancchaosorbm")
            {
                var minion = EntityManager.MinionsAndMonsters.Get(EntityManager.MinionsAndMonsters.EntityType.Minion,
                    EntityManager.UnitTeam.Enemy,
                    Player.Instance.ServerPosition, Program.QUltimate.Range)
                    .FirstOrDefault(
                        t => Extension.DamageLibrary.CalculateDamage(t, false, false, false, true) >= t.Health);

                if (minion != null)
                {
                    Program.QUltimate.Cast(minion);
                }
            }
            if (UseW && Program.W.IsReady() &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslide")
            {
                var minion = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,
                    Player.Instance.ServerPosition, Program.W.Range)
                    .Where(t => Extension.DamageLibrary.CalculateDamage(t, false, true, false, false) >= t.Health);
                var wAOE = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(minion, Program.W.Width,
                    (int) Program.W.Range);

                if (wAOE.HitNumber >= SliderW)
                {
                    Program.W.Cast(wAOE.CastPosition);
                }
            }
            if (UseW && Program.WReturn.IsReady() &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslidereturn")
            {
                Program.WReturn.Cast();
            }

            if (UseWR && Program.WUltimate.IsReady() &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancslidem")
            {
                var minion = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,
                    Player.Instance.ServerPosition, Program.WUltimate.Range)
                    .Where(t => Extension.DamageLibrary.CalculateDamage(t, false, false, false, true) >= t.Health);
                var wAOE = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(minion, Program.WUltimate.Width,
                    (int) Program.WUltimate.Range);

                if (wAOE.HitNumber >= SliderWR)
                {
                    Program.WUltimate.Cast(wAOE.CastPosition);
                }
            }

            if (UseWR && Program.RReturn.IsReady() &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancslidereturnm")
            {
                Program.RReturn.Cast();
            }
        }
    }
}