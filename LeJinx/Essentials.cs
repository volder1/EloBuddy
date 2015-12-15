namespace Jinx
{
    using EloBuddy;
    using EloBuddy.SDK;

    using SharpDX;

    using Color = System.Drawing.Color;

    /// <summary>
    /// A Class Containing Methods that are needed for the Main Program.
    /// </summary>
    internal class Essentials
    {
        /// <summary>
        /// Jungle Mob List 
        /// </summary>
        public static readonly string[] JungleMobsList = { "SRU_Red", "SRU_Blue", "SRU_Dragon", "SRU_Baron", "SRU_Gromp", "SRU_Murkwolf", "SRU_Razorbeak", "SRU_Krug", "Sru_Crab" };

        /// <summary>
        /// Jungle Mob List for Twisted Treeline
        /// </summary>
        public static readonly string[] JungleMobsListTwistedTreeline = { "TT_NWraith1.1", "TT_NWraith4.1", "TT_NGolem2.1", "TT_NGolem5.1", "TT_NWolf3.1", "TT_NWolf6.1", "TT_Spiderboss8.1" };

        /// <summary>
        /// Thank you ScienceARK for this method
        /// </summary>
        /// <returns>If Jinx is using FishBones.</returns>
        public static bool FishBones()
        {
            return Player.Instance.HasBuff("JinxQ");
        }

        /// <summary>
        /// Taken from OKTW. Spells that useE can be used on.
        /// </summary>
        /// <param name="spellName">The name of the Spell</param>
        /// <returns>If useE should be used or not.</returns>
        public static bool ShouldUseE(string spellName)
        {
            switch (spellName)
            {
                case "ThreshQ":
                    return true;
                case "KatarinaR":
                    return true;
                case "AlZaharNetherGrasp":
                    return true;
                case "GalioIdolOfDurand":
                    return true;
                case "LuxMaliceCannon":
                    return true;
                case "MissFortuneBulletTime":
                    return true;
                case "RocketGrabMissile":
                    return true;
                case "CaitlynPiltoverPeacemaker":
                    return true;
                case "EzrealTrueshotBarrage":
                    return true;
                case "InfiniteDuress":
                    return true;
                case "VelkozR":
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Contains the range of Minigun.
        /// </summary>
        public const float MinigunRange = 525f;

        /// <summary>
        /// Gets the Range of FishBones
        /// </summary>
        /// <returns>Returns the range of FishBones</returns>
        public static float FishBonesRange()
        {
            return 670f + Player.Instance.BoundingRadius + 25 * Program.Q.Level;
        }

        /// <summary>
        /// Taken from AdEvade which was taken from OKTW
        /// </summary>
        /// <param name="start">Start Position of Line</param>
        /// <param name="end">End Position of Line</param>
        /// <param name="radius">Radius of Line</param>
        /// <param name="width">Width of Line</param>
        /// <param name="color">Color of Line</param>
        public static void DrawLineRectangle(Vector2 start, Vector2 end, int radius, int width, Color color)
        {
            var dir = (end - start).Normalized();
            var pDir = dir.Perpendicular();

            var rightStartPos = start + pDir * radius;
            var leftStartPos = start - pDir * radius;
            var rightEndPos = end + pDir * radius;
            var leftEndPos = end - pDir * radius;

            var rStartPos = Drawing.WorldToScreen(new Vector3(rightStartPos.X, rightStartPos.Y, Player.Instance.Position.Z));
            var lStartPos = Drawing.WorldToScreen(new Vector3(leftStartPos.X, leftStartPos.Y, Player.Instance.Position.Z));
            var rEndPos = Drawing.WorldToScreen(new Vector3(rightEndPos.X, rightEndPos.Y, Player.Instance.Position.Z));
            var lEndPos = Drawing.WorldToScreen(new Vector3(leftEndPos.X, leftEndPos.Y, Player.Instance.Position.Z));

            Drawing.DrawLine(rStartPos, rEndPos, width, color);
            Drawing.DrawLine(lStartPos, lEndPos, width, color);
            Drawing.DrawLine(rStartPos, lStartPos, width, color);
            Drawing.DrawLine(lEndPos, rEndPos, width, color);
        }

        /// <summary>
        /// DamageLibrary Class for Jinx Spells.
        /// </summary>
        public static class DamageLibrary
        {
            /// <summary>
            /// Calculates and returns damage totally done to the target
            /// </summary>
            /// <param name="target">The Target</param>
            /// <param name="useQ">Include useQ in Calculations?</param>
            /// <param name="useW">Include useW in Calculations?</param>
            /// <param name="useE">Include useE in Calculations?</param>
            /// <param name="useR">Include useR in Calculations?</param>
            /// <returns>The total damage done to target.</returns>
            public static float CalculateDamage(Obj_AI_Base target, bool useQ, bool useW, bool useE, bool useR)
            {
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
            /// Calculates the Damage done with useQ
            /// </summary>
            /// <param name="target">The Target</param>
            /// <returns>Returns the Damage done with useQ</returns>
            private static float QDamage(Obj_AI_Base target)
            {
                return Player.Instance.GetAutoAttackDamage(target);
            }

            /// <summary>
            /// Calculates the Damage done with useW
            /// </summary>
            /// <param name="target">The Target</param>
            /// <returns>Returns the Damage done with useW</returns>
            private static float WDamage(Obj_AI_Base target)
            {
                return Player.Instance.CalculateDamageOnUnit(
                    target,
                    DamageType.Physical,
                    new[] { 0, 10, 60, 110, 160, 210 }[Program.W.Level])
                       + (Player.Instance.TotalAttackDamage * 1.4f);
            }

            /// <summary>
            /// Calculates the Damage done with useE
            /// </summary>
            /// <param name="target">The Target</param>
            /// <returns>Returns the Damage done with useE</returns>
            private static float EDamage(Obj_AI_Base target)
            {
                return Player.Instance.CalculateDamageOnUnit(
                    target,
                    DamageType.Magical,
                    new[] { 0, 80, 135, 190, 245, 300 }[Program.E.Level] + (Player.Instance.TotalMagicalDamage));
            }

            /// <summary>
            /// Calculates the Damage done with useR (Fluxy's Method)
            /// </summary>
            /// <param name="target">The Target</param>
            /// <returns>Returns the Damage done with useR</returns>
            private static float RDamage(Obj_AI_Base target)
            {
                if (!Program.R.IsLearned) return 0;
                var level = Program.R.Level - 1;

                if (target.Distance(Player.Instance) < 1350)
                {
                    return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                        (float)
                            (new double[] { 25, 35, 45 }[level] +
                             new double[] { 25, 30, 35 }[level] / 100 * (target.MaxHealth - target.Health) +
                             0.1 * Player.Instance.TotalAttackDamage));
                }

                return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                    (float)
                        (new double[] { 250, 350, 450 }[level] +
                         new double[] { 25, 30, 35 }[level] / 100 * (target.MaxHealth - target.Health) +
                         1 * Player.Instance.TotalAttackDamage));
            }
        }
    }
}