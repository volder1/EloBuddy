using EloBuddy;
using EloBuddy.SDK;

namespace Nasus
{
    internal class Extensions
    {
        internal class DamageLibrary
        {
            /// <summary>
            /// Calculates Damage for Nasus
            /// </summary>
            /// <param name="target">The Target</param>
            /// <param name="q">The Q</param>
            /// <param name="e">The E</param>
            /// <returns></returns>
            public static float CalculateDamage(Obj_AI_Base target, bool q, bool e)
            {
                var totaldamage = 0f;

                if (target == null) return totaldamage;

                if (q && Program.Q.IsReady())
                {
                    totaldamage += QDamage(target);
                }

                if (e && Program.E.IsReady())
                {
                    totaldamage += EDamage(target);
                }

                return totaldamage;
            }

            /// <summary>
            /// Calculates the Damage done with Q
            /// </summary>
            /// <param name="target">The Target</param>
            /// <returns>Returns the Damage done with Q</returns>
            private static float QDamage(Obj_AI_Base target)
            {
                float dmgItem = 0;

                if (Item.HasItem(3057) && (Item.CanUseItem(3057) || Player.HasBuff("sheen"))
                    && Player.Instance.BaseAttackDamage > dmgItem)
                {
                    dmgItem = Player.Instance.BaseAttackDamage;
                }

                if (Item.HasItem(3025) && (Item.CanUseItem(3025) || Player.HasBuff("itemfrozenfist"))
                    && Player.Instance.BaseAttackDamage*1.25 > dmgItem)
                {
                    dmgItem = Player.Instance.BaseAttackDamage*1.25f;
                }

                return
                    Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                        (new float[] {0, 30, 50, 70, 90, 110}[Program.Q.Level] + Player.Instance.FlatPhysicalDamageMod +
                         Player.Instance.GetBuffCount("NasusQStacks")) + dmgItem) +
                    Player.Instance.GetAutoAttackDamage(target);
            }

            /// <summary>
            /// Calculates the Damage done with E
            /// </summary>
            /// <param name="target">The Target</param>
            /// <returns>Returns the Damage done with E</returns>
            private static float EDamage(Obj_AI_Base target)
            {
                return target.CalculateDamageOnUnit(target, DamageType.Magical,
                    new[] {0, 55, 95, 135, 175, 215}[Program.E.Level] + (Player.Instance.TotalMagicalDamage*0.6f));
            }
        }
    }
}