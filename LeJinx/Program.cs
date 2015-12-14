using System.ComponentModel;
using System.Media;
using System.Net;
using EloBuddy.Sandbox;

namespace Jinx
{
    using System;
    using System.IO;
    using System.Linq;

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
        /// Stores Allah Akbar File
        /// </summary>
        public static SoundPlayer AllahAkbar;

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
            R = new Spell.Skillshot(SpellSlot.R, 3000, SkillShotType.Linear, 500, 1500, 140);

            JinXxxMenu.Initialize();
            Indicator = new DamageIndicator.DamageIndicator();

            Chat.Print("Jin-XXX: Loaded", System.Drawing.Color.AliceBlue);
            Chat.Print("Jin-XXX: Check out the Menu and adjust to your preference.", System.Drawing.Color.Aqua);
            Chat.Print("Jin-XXX: Please be sure to upvote if you enjoy!", System.Drawing.Color.OrangeRed);

            Game.OnUpdate += Game_OnUpdate;
            Game.OnUpdate += ActiveStates.Game_OnUpdate;
            Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Drawing.OnDraw += Drawing_OnDraw;

            try
            {
                var sandBox = SandboxConfig.DataDirectory + @"\JinXXX\";

                if (!Directory.Exists(sandBox))
                {
                    Directory.CreateDirectory(sandBox);
                }

                // Credits to iRaxe for the Original Idea
                if (!File.Exists(sandBox + "Allahu_Akbar_Sound_Effect_Download_Link.wav"))
                {
                    var client = new WebClient();
                    client.DownloadFile("http://italianbuffet.it/Allahu_Akbar_Sound_Effect_Download_Link.wav",
                        sandBox + "Allahu_Akbar_Sound_Effect_Download_Link.wav");
                    client.DownloadFileCompleted += Client_DownloadFileCompleted;
                }

                if (File.Exists(sandBox + "Allahu_Akbar_Sound_Effect_Download_Link.wav"))
                {
                    AllahAkbar = new SoundPlayer
                    {
                        SoundLocation = SandboxConfig.DataDirectory + @"\JinXXX\" + "Allahu_Akbar_Sound_Effect_Download_Link.wav"
                    };
                    AllahAkbar.Load();
                }
            }
            catch (Exception e)
            {
                Chat.Print("Failed to load Allah Akbar: " + e.ToString());
            }
        }

        /// <summary>
        /// Download Finished
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The Args</param>
        private static void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Chat.Print("Failed Downloading: " + e.Error.ToString());
            AllahAkbar = new SoundPlayer
            {
                SoundLocation = SandboxConfig.DataDirectory + @"\JinXXX\" + "Allahu_Akbar_Sound_Effect_Download_Link.wav"
            };
            AllahAkbar.Load();
        }

        /// <summary>
        /// Called Before Attack
        /// </summary>
        /// <param name="target">The Target</param>
        /// <param name="args">The Args</param>
        private static void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (Essentials.FishBones() && target.IsStructure())
            {
                Q.Cast();
            }
        }

        /// <summary>
        /// Called After Attack
        /// </summary>
        /// <param name="target">The Target</param>
        /// <param name="args">The Args</param>
        private static void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            Orbwalker.ForcedTarget = null;
        }

        /// <summary>
        /// Called when a Spell gets Casted
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="args">The Spell</param>
        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var allahAkbarT = JinXxxMenu.MiscMenu["allahAkbarT"].Cast<CheckBox>().CurrentValue;
            var autoE = JinXxxMenu.MiscMenu["autoE"].Cast<CheckBox>().CurrentValue;
            var eSlider = JinXxxMenu.MiscMenu["eSlider"].Cast<Slider>().CurrentValue;

            if (AllahAkbar != null)
            {
                if (allahAkbarT && sender.IsMe && args.SData.Name.Equals("JinxR"))
                {
                    AllahAkbar.Play();
                }
            }

            if (!autoE || !E.IsReady())
            {
                return;
            }

            if (!sender.IsEnemy || !sender.IsValidTarget(E.Range) || !Essentials.ShouldUseE(args.SData.Name))
            {
                return;
            }
            var prediction = E.GetPrediction(sender);

            if (prediction.HitChancePercent >= eSlider)
            {
                E.Cast(prediction.CastPosition);
            }
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
                Circle.Draw(Q.IsReady() ? Color.Green : Color.Red, !Essentials.FishBones() ? Essentials.FishBonesRange() : Essentials.MinigunRange, Player.Instance.Position);
            }

            if (drawW)
            {
                Circle.Draw(W.IsReady() ? Color.Red : Color.Green, W.Range, Player.Instance.Position);
            }

            if (drawE)
            {
                Circle.Draw(E.IsReady() ? Color.Red : Color.Green, E.Range, Player.Instance.Position);
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
                        && t.Distance(Player.Instance) >= JinXxxMenu.MiscMenu["rRange"].Cast<Slider>().CurrentValue)
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
        private static void Game_OnUpdate(EventArgs args)
        {
            if (Orbwalker.ForcedTarget != null)
            {
                if (!Player.Instance.IsInAutoAttackRange(Orbwalker.ForcedTarget) || !Orbwalker.ForcedTarget.IsValidTarget())
                {
                    Orbwalker.ForcedTarget = null;
                }
                else if (Orbwalker.ForcedTarget !=
                         TargetSelector.GetTarget(Player.Instance.GetAutoAttackRange(), DamageType.Physical) &&
                         Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
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

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                StateManager.LastHit();
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