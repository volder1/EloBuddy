using EloBuddy;
using EloBuddy.SDK;

namespace EloBuddy_PandaTeemo
{
    /// <summary>
    /// DamageLibrary Class for Teemo Spells.
    /// </summary>
    internal class DamageLibrary
    {
        /// <summary>
        /// Calculates and returns damage totally done to the target
        /// </summary>
        /// <param name="target">The Target</param>
        /// <param name="useQ">Include Q in Calculations?</param>
        /// <param name="useW">Include W in Calculations?</param>
        /// <param name="useE">Include E in Calculations?</param>
        /// <param name="useR">Include R in Calculations?</param>
        /// <returns>The total damage done to target.</returns>
        public static float CalculateDamage(Obj_AI_Base target, bool useQ, bool useW, bool useE, bool useR)
        {
            if (target == null)
            {
                return 0;
            }

            var totaldamage = 0f;

            if (useQ && Program.Q.IsReady())
            {
                totaldamage = totaldamage + QDamage(target);
            }

            if (useW && Program.W.IsReady())
            {
                totaldamage = totaldamage + WDamage(target);
            }

            if (useE && Program.E.IsReady())
            {
                totaldamage = totaldamage + EDamage(target);
            }

            if (useR && Program.R.IsReady())
            {
                totaldamage = totaldamage + RDamage(target);
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
            return target.CalculateDamageOnUnit(target, DamageType.Magical,
                new[] {0, 80, 125, 170, 215, 260}[Program.Q.Level] + (Player.Instance.TotalMagicalDamage*0.8f));
        }

        /// <summary>
        /// Calculates the Damage done with W
        /// </summary>
        /// <param name="target">The Target</param>
        /// <returns>Returns the Damage done with W</returns>
        private static float WDamage(Obj_AI_Base target)
        {
            return 0;
        }

        /// <summary>
        /// Calculates the Damage done with E
        /// </summary>
        /// <param name="target">The Target</param>
        /// <returns>Returns the Damage done with E</returns>
        private static float EDamage(Obj_AI_Base target)
        {
            return target.CalculateDamageOnUnit(target, DamageType.Magical,
                new[] {0, 10, 20, 30, 40, 50}[Program.E.Level] + (Player.Instance.TotalMagicalDamage*0.3f));
        }

        /// <summary>
        /// Calculates the Damage Done with R
        /// </summary>
        /// <param name="target">The Target</param>
        /// <returns>Returns the Damage done with R</returns>
        private static float RDamage(Obj_AI_Base target)
        {
            return target.CalculateDamageOnUnit(target, DamageType.Magical,
                (float) (new[] {0, 50, 81.25, 112.5}[Program.R.Level] + (Player.Instance.TotalMagicalDamage*0.125f)));
        }
    }
}
