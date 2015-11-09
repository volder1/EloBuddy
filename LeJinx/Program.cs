namespace LeJinx
{
    using System;
    using System.Linq;
    using System.Security.Permissions;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;

    using SharpDX;

    /// <summary>
    /// Made by KarmaPanda
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Spell Q
        /// </summary>
        public static Spell.Active Q;

        /// <summary>
        /// The Spells W, E, and R
        /// </summary>
        public static Spell.Skillshot W, E, R;

        /// <summary>
        /// Champion's Name
        /// </summary>
        public const string ChampionName = "Jinx";

        /// <summary>
        /// Stores Damage Indicator
        /// </summary>
        public static DamageIndicator.DamageIndicator Indicator;

        /// <summary>
        /// Called when the Program is run
        /// </summary>
        private static void Main()
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        /// <summary>
        /// Called when the Game finishes loading
        /// </summary>
        /// <param name="args">The Args</param>
        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.BaseSkinName != ChampionName)
            {
                return;
            }

            Q = new Spell.Active(SpellSlot.Q);
            W = new Spell.Skillshot(SpellSlot.W, 1450, SkillShotType.Linear, 600, 3300, 60);
            E = new Spell.Skillshot(SpellSlot.E, 900, SkillShotType.Circular, 1200, 1750, 150);
            R = new Spell.Skillshot(SpellSlot.R, 3250, SkillShotType.Linear, 500, 1700, 140);

            JinXxxMenu.Initialize();
            Indicator = new DamageIndicator.DamageIndicator();

            Chat.Print("Jin-XXX by KarmaPanda", Color.Green);

            Game.OnTick += Game_OnTick;
            Game.OnTick += ActiveStates.Game_OnTick;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        /// <summary>
        /// Called when it is possible to Interrupt
        /// </summary>
        /// <param name="sender">The Interruptable Target</param>
        /// <param name="e">The Information</param>
        private static void Interrupter_OnInterruptableSpell(
            Obj_AI_Base sender,
            Interrupter.InterruptableSpellEventArgs e)
        {
            if (sender != null && !sender.IsAlly && JinXxxMenu.MiscMenu["interruptE"].Cast<CheckBox>().CurrentValue
                && Player.Instance.ManaPercent >= JinXxxMenu.MiscMenu["interruptmanaE"].Cast<Slider>().CurrentValue
                && (E.IsInRange(sender) && E.IsReady() && sender.IsValidTarget() && e.DangerLevel == DangerLevel.High))
            {
                E.Cast(sender);
            }
        }

        /// <summary>
        /// Called when it is possible to Gapclose
        /// </summary>
        /// <param name="sender">The Gapclosable Target</param>
        /// <param name="e">The Information</param>
        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (sender != null && !sender.IsAlly && JinXxxMenu.MiscMenu["gapcloserE"].Cast<CheckBox>().CurrentValue
                && Player.Instance.ManaPercent >= JinXxxMenu.MiscMenu["gapclosermanaE"].Cast<Slider>().CurrentValue
                && (E.IsInRange(sender) && E.IsReady() && sender.IsValidTarget()))
            {
                E.Cast(sender);
            }
        }

        /// <summary>
        /// Called every time the Game Draws
        /// </summary>
        /// <param name="args">The Args</param>
        private static void Drawing_OnDraw(EventArgs args)
        {
            var drawQ = JinXxxMenu.DrawingMenu["drawQ"].Cast<CheckBox>().CurrentValue;
            var drawW = JinXxxMenu.DrawingMenu["drawW"].Cast<CheckBox>().CurrentValue;
            var drawE = JinXxxMenu.DrawingMenu["drawE"].Cast<CheckBox>().CurrentValue;
            var predW = JinXxxMenu.DrawingMenu["predW"].Cast<CheckBox>().CurrentValue;
            var predR = JinXxxMenu.DrawingMenu["predR"].Cast<CheckBox>().CurrentValue;

            if (drawQ)
            {
                Circle.Draw(Q.IsReady() ? Color.Red : Color.Green, Essentials.FishBones() ? Essentials.FishBonesRange() : Essentials.MinigunRange);
            }

            if (drawW)
            {
                Circle.Draw(W.IsReady() ? Color.Red : Color.Green, W.Radius);
            }

            if (drawE)
            {
                Circle.Draw(E.IsReady() ? Color.Red : Color.Green, E.Radius);
            }

            if (predW)
            {
                var enemy =
                    EntityManager.Heroes.Enemies.Where(t => t.IsValidTarget() && W.IsInRange(t))
                        .OrderBy(t => t.Distance(Player.Instance))
                        .FirstOrDefault();
                if (enemy == null)
                {
                    return;
                }
                var wPred = W.GetPrediction(enemy).CastPosition;
                Essentials.DrawLineRectangle(wPred.To2D(), Player.Instance.Position.To2D(), W.Width, 1, W.IsReady() ? System.Drawing.Color.Yellow : System.Drawing.Color.Red);
            }

            if (predR)
            {
                var enemy =
                    EntityManager.Heroes.Enemies.Where(
                        t =>
                        t.IsValidTarget()
                        && t.Distance(Player.Instance) <= JinXxxMenu.MiscMenu["rRange"].Cast<Slider>().CurrentValue)
                        .OrderBy(t => t.Distance(Player.Instance))
                        .FirstOrDefault();
                if (enemy == null)
                {
                    return;
                }
                var rPred = R.GetPrediction(enemy).CastPosition;
                Essentials.DrawLineRectangle(rPred.To2D(), Player.Instance.Position.To2D(), R.Width, 1, R.IsReady() ? System.Drawing.Color.Yellow : System.Drawing.Color.Red);
            }
        }

        /// <summary>
        /// Called every time the Game Ticks
        /// </summary>
        /// <param name="args">The Args</param>
        private static void Game_OnTick(EventArgs args)
        {
            if (Orbwalker.ForcedTarget != null)
            {
                if (!Player.Instance.IsInAutoAttackRange(Orbwalker.ForcedTarget) || !Orbwalker.ForcedTarget.IsValid)
                {
                    Orbwalker.ForcedTarget = null;
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                StateManager.Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                StateManager.Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                StateManager.LaneClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                StateManager.JungleClear();
            }
        }
    }
}