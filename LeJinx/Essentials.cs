namespace LeJinx
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    using SharpDX;

    /// <summary>
    /// A Class Containing Methods that are needed for the Main Program.
    /// </summary>
    internal class Essentials
    {
        /// <summary>
        /// Thank you ScienceARK for this method
        /// </summary>
        /// <returns>If Jinx is using FishBones.</returns>
        public static bool FishBones()
        {
            return Player.Instance.AttackRange > 530f;
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
        /// Calculates if Q Should be Minigun or FishBones
        /// </summary>
        /// <param name="t">The Target being attacked (Or will be)</param>
        /// <param name="menu">Combo, LaneClear, Harass, etc</param>
        /// <returns>Returns the string "Minigun" or "FishBones"</returns>
        public static string QModeSelector(AIHeroClient t, Menu menu)
        {
            string qMode;
            var championsAroundTarget = EntityManager.Heroes.Enemies.Count(target => target.IsValidTarget() && target.Distance(t) <= 100);
            var minionsAroundTarget = EntityManager.MinionsAndMonsters.EnemyMinions.Count(target => target.IsValidTarget() && target.Distance(t) <= 100);

            if (championsAroundTarget >= menu["qCountC"].Cast<Slider>().CurrentValue)
            {
                qMode = "FishBones";
            }
            else if (minionsAroundTarget >= menu["qCountM"].Cast<Slider>().CurrentValue)
            {
                qMode = "FishBones";
            }
            else
            {
                qMode = "Minigun";
            }

            return qMode;
        }

        /// <summary>
        /// Calculates if Q Should be Minigun or FishBones
        /// </summary>
        /// <param name="m">The Minion being attacked (Or will be)</param>
        /// <param name="menu">Combo, LaneClear, Harass, etc</param>
        /// <returns>Returns the string "Minigun" or "FishBones"</returns>
        public static string QModeSelector(Obj_AI_Base m, Menu menu)
        {
            var minionsAroundTarget = EntityManager.MinionsAndMonsters.EnemyMinions.Where(target => target.IsValidTarget() && target.Distance(m) <= 100);
            var qMode = minionsAroundTarget.Count() >= menu["qCountM"].Cast<Slider>().CurrentValue ? "FishBones" : "Minigun";

            return qMode;
        }

        /// <summary>
        /// Uses Q after logic. For Fishbones.
        /// </summary>
        public static void FishbonesQLogic()
        {
            if (!FishBones())
            {
                return;
            }

            var target = TargetSelector.GetTarget(FishBonesRange(), DamageType.Physical);

            if (target == null)
            {
                return;
            }

            if (!(Player.Instance.Distance(target) <= MinigunRange)
                || QModeSelector(target, JinXxxMenu.ComboMenu) != "Minigun")
            {
                return;
            }

            Program.Q.Cast();
            Orbwalker.ForcedTarget = target;
        }

        /// <summary>
        /// Uses Q after logic. For Minigun.
        /// </summary>
        public static void MinigunQLogic()
        {
            if (FishBones())
            {
                return;
            }

            var target = TargetSelector.GetTarget(FishBonesRange(), DamageType.Physical);

            if (target == null)
            {
                return;
            }

            if (Player.Instance.Distance(target) <= FishBonesRange() && Player.Instance.Distance(target) > MinigunRange)
            {
                Program.Q.Cast();
                Orbwalker.ForcedTarget = target;
            }

            else if (Player.Instance.Distance(target) <= FishBonesRange()
                     && QModeSelector(target, JinXxxMenu.ComboMenu) == "FishBones")
            {
                Program.Q.Cast();
                Orbwalker.ForcedTarget = target;
            }
        }

        /// <summary>
        /// Taken from AdEvade which was taken from OKTW
        /// </summary>
        /// <param name="start">Start Position of Line</param>
        /// <param name="end">End Position of Line</param>
        /// <param name="radius">Radius of Line</param>
        /// <param name="width">Width of Line</param>
        /// <param name="color">Color of Line</param>
        public static void DrawLineRectangle(Vector2 start, Vector2 end, int radius, int width, System.Drawing.Color color)
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
            /// <param name="Q">Include Q in Calculations?</param>
            /// <param name="W">Include W in Calculations?</param>
            /// <param name="E">Include E in Calculations?</param>
            /// <param name="R">Include R in Calculations?</param>
            /// <returns>The total damage done to target.</returns>
            public static float CalculateDamage(Obj_AI_Base target, bool Q, bool W, bool E, bool R)
            {
                var totaldamage = 0f;

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

                return totaldamage;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="target"></param>
            /// <returns></returns>
            private static float QDamage(Obj_AI_Base target)
            {
                return Player.Instance.GetAutoAttackDamage(target);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="target"></param>
            /// <returns></returns>
            private static float WDamage(Obj_AI_Base target)
            {
                return Player.Instance.CalculateDamageOnUnit(
                    target,
                    DamageType.Physical,
                    new[] { 0, 10, 60, 110, 160, 210 }[Program.W.Level])
                       + (Player.Instance.FlatPhysicalDamageMod * 1.4f);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="target"></param>
            /// <returns></returns>
            private static float EDamage(Obj_AI_Base target)
            {
                return Player.Instance.CalculateDamageOnUnit(
                    target,
                    DamageType.Magical,
                    new[] { 0, 80, 135, 190, 245, 300 }[Program.E.Level] + (Player.Instance.FlatMagicDamageMod));
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="target"></param>
            /// <returns></returns>
            private static float RDamage(Obj_AI_Base target)
            {
                return Player.Instance.CalculateDamageOnUnit(
                    target,
                    DamageType.Physical,
                    (target.MaxHealth - target.Health * new[] { 0, 0.25f, 0.30f, 0.45f }[Program.R.Level])
                    - target.FlatPhysicalReduction);
            }
        }
    }
}