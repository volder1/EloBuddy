namespace TwistedBuddy
{
    using EloBuddy;
    using EloBuddy.SDK;

    class DamageLibrary
    {
        /// <summary>
        /// Calculates and returns damage totally done to the target
        /// </summary>
        /// <param name="target">The Target</param>
        /// <param name="Q">Include Q in Calculations?</param>
        /// <param name="W">Include W in Calculations?</param>
        /// <param name="E">Include E in Calculations?</param>
        /// <param name="R">Include R in Calculations?</param>
        /// <returns>The total damage done to target.</returns>
        public static float CalculateDamage(Obj_AI_Base target, bool Q, bool W, bool E, bool R)
        {
            var totaldamage = 0f;

            if (target != null)
            {
                if (Q && Program.Q.IsReady())
                {
                    totaldamage = totaldamage + QDamage(target);
                }

                if (W && Program.W.IsReady())
                {
                    totaldamage = totaldamage + WDamage(target);
                }

                if (E && Program.E.IsReady())
                {
                    totaldamage = totaldamage + EDamage(target);
                }

                if (R && Program.R.IsReady())
                {
                    totaldamage = totaldamage + RDamage(target);
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
            return (new[] { 0, 60, 100, 160, 210, 260 }[Program.Q.Level]) + (Player.Instance.FlatMagicDamageMod * 0.65f);
        }

        /// <summary>
        /// Calculates the Damage done with W
        /// </summary>
        /// <param name="target">The Target</param>
        /// <returns>Returns the Damage done with W</returns>
        private static float WDamage(Obj_AI_Base target)
        {
            if (Player.Instance.HasBuff("bluecardpreattack"))
            {
                return Player.Instance.CalculateDamageOnUnit(
                    target,
                    DamageType.Mixed,
                    new[] { 0, 40, 60, 80, 100, 120 }[Program.W.Level]) + (Player.Instance.FlatMagicDamageMod * 0.5f)
                       + (Player.Instance.FlatPhysicalDamageMod);
            }
            if (Player.Instance.HasBuff("redcardpreattack"))
            {
                return Player.Instance.CalculateDamageOnUnit(
                    target,
                    DamageType.Mixed,
                    new[] { 0, 30, 45, 60, 75, 90 }[Program.W.Level] + (Player.Instance.FlatMagicDamageMod * 0.5f)
                    + Player.Instance.FlatPhysicalDamageMod);
            }
            if (Player.Instance.HasBuff("goldcardpreattack"))
            {
                return Player.Instance.CalculateDamageOnUnit(
                    target,
                    DamageType.Mixed,
                    new[] { 0, 15, 22.5f, 30, 37.5f, 45 }[Program.W.Level] + (Player.Instance.FlatMagicDamageMod * 0.5f)
                    + Player.Instance.FlatPhysicalDamageMod);
            }
            return 0;
        }

        /// <summary>
        /// Calculates the Damage done with E
        /// </summary>
        /// <param name="target">The Target</param>
        /// <returns>Returns the Damage done with E</returns>
        private static float EDamage(Obj_AI_Base target)
        {
            if (Player.Instance.HasBuff("cardmasterstackparticle"))
            {
                return Player.Instance.CalculateDamageOnUnit(
                    target,
                    DamageType.Magical,
                    new[] { 0, 55, 80, 105, 130, 155 }[Program.E.Level]) + (Player.Instance.FlatMagicDamageMod * 0.5f);
            }
            return 0;
        }

        /// <summary>
        /// Calculates the Damage done with R (Fluxy's Method)
        /// </summary>
        /// <param name="target">The Target</param>
        /// <returns>Returns the Damage done with R</returns>
        private static float RDamage(Obj_AI_Base target)
        {
            return 0;
        }
    }
}
