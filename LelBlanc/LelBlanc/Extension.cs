using EloBuddy;
using EloBuddy.SDK;

namespace LelBlanc
{
    internal class Extension
    {
        public static bool IsMarked(Obj_AI_Base target)
        {
            return target.HasBuff("LeBlancMarkOfSilence") || target.HasBuff("LeBlancMarkOfSilenceM");
        }

        internal class DamageLibrary
        {
            /// <summary>
            /// Calculates Damage for LeBlanc
            /// </summary>
            /// <param name="target">The Target</param>
            /// <param name="q">The Q</param>
            /// <param name="w">The W</param>
            /// <param name="e">The E</param>
            /// <param name="r">The R</param>
            /// <returns></returns>
            public static float CalculateDamage(Obj_AI_Base target, bool q, bool w, bool e, bool r)
            {
                var totaldamage = 0f;

                if (target == null) return totaldamage;

                if (q && Program.Q.IsReady())
                {
                    totaldamage += QDamage(target);
                }

                if (w && Program.W.IsReady() &&
                    Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslide")
                {
                    totaldamage = WDamage(target);

                    if (q && Program.Q.IsReady() || IsMarked(target))
                    {
                        totaldamage += QDamage(target);
                    }
                }

                if (e && Program.E.IsReady())
                {
                    totaldamage += EDamage(target);

                    if (q && Program.Q.IsReady() || IsMarked(target))
                    {
                        totaldamage += QDamage(target);
                    }
                }

                if (r && Program.QUltimate.IsReady() &&
                    Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancchaosorbm")
                {
                    totaldamage += QDamage(target);
                }

                if (r && Program.WUltimate.IsReady() &&
                    Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancslidem")
                {
                    totaldamage += WrDamage(target);

                    if (q && Program.Q.IsReady() || IsMarked(target))
                    {
                        totaldamage += QDamage(target);
                    }
                }

                if (r && Program.EUltimate.IsReady() &&
                    Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancsoulshacklem")
                {
                    totaldamage += ErDamage(target);

                    if (q && Program.Q.IsReady() || IsMarked(target))
                    {
                        totaldamage += QDamage(target);
                    }
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
                return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical,
                    new[] {0, 55, 80, 105, 130, 155}[Program.Q.Level] + (Player.Instance.TotalMagicalDamage*0.4f));
            }

            /// <summary>
            /// Calculates the Damage done with W
            /// </summary>
            /// <param name="target">The Target</param>
            /// <returns>Returns the Damage done with W</returns>
            private static float WDamage(Obj_AI_Base target)
            {
                return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical,
                    new[] {0, 85, 125, 165, 205, 245}[Program.W.Level] + (Player.Instance.TotalMagicalDamage*0.6f));
            }

            /// <summary>
            /// Calculates the Damage done with E
            /// </summary>
            /// <param name="target">The Target</param>
            /// <returns>Returns the Damage done with E</returns>
            private static float EDamage(Obj_AI_Base target)
            {
                return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical,
                    new[] {0, 40, 65, 90, 115, 140}[Program.E.Level] + (Player.Instance.TotalMagicalDamage*0.5f));
            }

            /// <summary>
            /// Calculates the Damage done with R
            /// </summary>
            /// <returns>Returns the Damage done with R</returns>
            private static float WrDamage(Obj_AI_Base target)
            {
                return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical,
                    new[] {0, 150, 300, 450}[Program.WUltimate.Level] + (Player.Instance.TotalMagicalDamage*0.9f));
            }

            /// <summary>
            /// Calculates the Damage done with R
            /// </summary>
            /// <returns>Returns the Damage done with R</returns>
            private static float ErDamage(Obj_AI_Base target)
            {
                return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical,
                    new[] {0, 100, 200, 300}[Program.EUltimate.Level] + (Player.Instance.TotalMagicalDamage*0.6f));
            }
        }
    }
}