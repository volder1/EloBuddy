namespace Nasus
{
    using System;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;

    /// <summary>
    /// Main Class
    /// </summary>
    class Program
    {
        /// <summary>
        /// Spell Q
        /// </summary>
        public static Spell.Active Q;

        /// <summary>
        /// Spell W
        /// </summary>
        public static Spell.Targeted W;

        /// <summary>
        /// Spell E
        /// </summary>
        public static Spell.Skillshot E;

        /// <summary>
        /// Spell R
        /// </summary>
        public static Spell.Active R;

        /// <summary>
        /// Called when the Program Starts
        /// </summary>
        private static void Main()
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        /// <summary>
        /// Called when the Game finished Loading
        /// </summary>
        /// <param name="args">The Args</param>
        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Nasus)
            {
                return;
            }

            Q = new Spell.Active(SpellSlot.Q, 150);
            W = new Spell.Targeted(SpellSlot.W, 600);
            E = new Spell.Skillshot(SpellSlot.E, 650, SkillShotType.Circular, 500, 20, 380)
            {
                AllowedCollisionCount = int.MaxValue
            };
            R = new Spell.Active(SpellSlot.R);

            // Methods
            Config.Initialize();
            Chat.Print("KA Nasus: Loaded", System.Drawing.Color.Green);

            // Events
            Game.OnTick += Game_OnTick;
            Game.OnTick += StateHandler.KillSteal;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
        }

        private static void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                var t = target as AIHeroClient;
                var useQ = Config.ComboMenu["useQ"].Cast<CheckBox>().CurrentValue;
                var useQh = Config.FarmMenu["useQH"].Cast<CheckBox>().CurrentValue;

                if (t != null && (useQ || useQh))
                {
                    if (t.IsValidTarget() && Q.IsReady())
                    {
                        Q.Cast();
                        Orbwalker.ForcedTarget = target;
                    }
                }
            }
        }

        /// <summary>
        /// Called whenever a target is about to be attacked by Orbwalker.
        /// </summary>
        /// <param name="target">The Target</param>
        /// <param name="args">The Args</param>
        private static void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                var aaDisable = Config.FarmMenu["disableAA"].Cast<CheckBox>().CurrentValue;

                if (aaDisable && !Player.Instance.HasBuff("SiphoningStrike"))
                {
                    args.Process = false;
                }
            }
        }

        /// <summary>
        /// Called whenever the Game Draws Itself
        /// </summary>
        /// <param name="args">The Args</param>
        private static void Drawing_OnDraw(EventArgs args)
        {
            var drawW = Config.DrawingMenu["drawW"].Cast<CheckBox>().CurrentValue;
            var drawE = Config.DrawingMenu["drawE"].Cast<CheckBox>().CurrentValue;

            if (drawW)
            {
                Circle.Draw(W.IsReady() ? SharpDX.Color.Green : SharpDX.Color.Red, W.Range, Player.Instance.Position);
            }

            if (drawE)
            {
                Circle.Draw(E.IsReady() ? SharpDX.Color.Green : SharpDX.Color.Red, E.Range, Player.Instance.Position);
            }
        }

        /// <summary>
        /// Called whenever the Game Updates Itself
        /// </summary>
        /// <param name="args">The Args</param>
        private static void Game_OnTick(EventArgs args)
        {
            if (Orbwalker.ForcedTarget != null)
            {
                if (!Player.Instance.IsInAutoAttackRange(Orbwalker.ForcedTarget) ||
                    !Orbwalker.ForcedTarget.IsValidTarget() ||
                    Orbwalker.ForcedTarget.IsInvulnerable)
                {
                    Orbwalker.ForcedTarget = null;
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                StateHandler.Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                StateHandler.Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                StateHandler.LaneClear();
            }

            if ((Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) ||
                 Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) ||
                 Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear)) &&
                Config.FarmMenu["useQ"].Cast<CheckBox>().CurrentValue)
            {
                StateHandler.LastHit();
            }
        }
    }
}