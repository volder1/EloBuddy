using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace LelBlanc
{
    class Program
    {
        /// <summary>
        /// Should Activate Gapcloser Combo
        /// </summary>
        public static bool ComboGapCloser;

        /// <summary>
        /// Gapcloser Target
        /// </summary>
        public static AIHeroClient GapCloserTarget;
        
        /// <summary>
        /// Position for Last W
        /// </summary>
        public static Vector3 LastWPosition;

        /// <summary>
        /// Position for Last W Ultimate
        /// </summary>
        public static Vector3 LastWUltimatePosition;

        /// <summary>
        /// Position for Last W End Position
        /// </summary>
        public static Vector3 LastWEndPosition;

        /// <summary>
        /// Position for Last W End Position
        /// </summary>
        public static Vector3 LastWUltimateEndPosition;

        /// <summary>
        /// Contains All Active Spells
        /// </summary>
        public static Spell.Active WReturn, RReturn;

        /// <summary>
        /// Contains All Targeted Spells
        /// </summary>
        public static Spell.Targeted Q, QUltimate;

        /// <summary>
        /// Contains All Skillshots
        /// </summary>
        public static Spell.Skillshot W, E, WUltimate, EUltimate;

        /// <summary>
        /// Contains the DamageIndicator
        /// </summary>
        public static DamageIndicator Indicator;

        /// <summary>
        /// Called when the Program is Initialized
        /// </summary>
        private static void Main()
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        /// <summary>
        /// Called when the Game is finished loading
        /// </summary>
        /// <param name="args"></param>
        private static void Loading_OnLoadingComplete(System.EventArgs args)
        {
            Q = new Spell.Targeted(SpellSlot.Q, 720)
            {
                CastDelay = 500
            };

            W = new Spell.Skillshot(SpellSlot.W, 700, SkillShotType.Circular, 600, 1450, 220);

            WReturn = new Spell.Active(SpellSlot.W);

            RReturn = new Spell.Active(SpellSlot.R);

            E = new Spell.Skillshot(SpellSlot.E, 900, SkillShotType.Linear, 300, 1650, 55)
            {
                AllowedCollisionCount = 0
            };

            QUltimate = new Spell.Targeted(SpellSlot.R, 720);

            WUltimate = new Spell.Skillshot(SpellSlot.R, 700, SkillShotType.Circular, 600, 1450, 220);

            EUltimate = new Spell.Skillshot(SpellSlot.R, 900, SkillShotType.Linear, 300, 1650, 55)
            {
                AllowedCollisionCount = 0
            };

            Chat.Print("LelBlanc Loaded", System.Drawing.Color.Blue);

            ComboGapCloser = false;

            // Methods
            Config.Initialize();

            // Constructors
            Indicator = new DamageIndicator();
            
            // Events
            Game.OnUpdate += Game_OnUpdate;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnSpellCast;
            Drawing.OnDraw += OnDraw.DrawRange;
        }
        
        /// <summary>
        /// Called when On Spell Cast
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="args">The Arguments</param>
        private static void Obj_AI_Base_OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;

            if (args.SData.Name == "leblancslide")
            {
                LastWPosition = args.Start;
                LastWEndPosition = args.End;
            }

            if (args.SData.Name == "leblancslidem")
            {
                LastWUltimatePosition = args.Start;
                LastWUltimateEndPosition = args.End;
            }
        }

        /// <summary>
        /// Called when Gapcloser is possible
        /// </summary>
        /// <param name="sender">The Sender of the Gapcloser</param>
        /// <param name="e">The Event</param>
        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (e.Target == null || ComboGapCloser) return;
            if (!W.IsInRange(e.Target))
            {
                ComboGapCloser = false;
                GapCloserTarget = null;
                return;
            }
            ComboGapCloser = true;
            GapCloserTarget = e.Target as AIHeroClient;
            Modes.Gapcloser.Execute(e.Target as AIHeroClient);
        }

        /// <summary>
        /// Called whenever the game is being runned.
        /// </summary>
        /// <param name="args"></param>
        private static void Game_OnUpdate(System.EventArgs args)
        {
            if (Modes.KillSteal.ResetW && Player.Instance.ServerPosition.IsInRange(LastWEndPosition, 100))
            {
                if (WReturn.IsReady() && Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslidereturn")
                {
                    WReturn.Cast();
                    LastWEndPosition = new Vector3(null);
                    Modes.KillSteal.ResetW = false;
                    return;
                }
            }
            if (Modes.KillSteal.ResetW && Player.Instance.ServerPosition.IsInRange(LastWUltimateEndPosition, 100))
            {
                if (RReturn.IsReady() && Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancslidereturnm")
                {
                    RReturn.Cast();
                    LastWUltimateEndPosition = new Vector3(null);
                    Modes.KillSteal.ResetW = false;
                    return;
                }
            }
            if (GapCloserTarget != null && !W.IsInRange(GapCloserTarget))
            {
                GapCloserTarget = null;
                ComboGapCloser = false;
            }
            if (Config.MiscMenu["pet"].Cast<CheckBox>().CurrentValue)
            {
                Pet.MovePet();
            }
            if (Config.KillStealMenu["toggle"].Cast<CheckBox>().CurrentValue)
            {
                Modes.KillSteal.Execute();
            }
            if (Config.MiscMenu["gapCloser"].Cast<CheckBox>().CurrentValue && ComboGapCloser && GapCloserTarget != null)
            {
                Modes.Gapcloser.Execute(GapCloserTarget);
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Modes.Combo.Execute();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Modes.Harass.Execute();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                Modes.LaneClear.Execute();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Modes.JungleClear.Execute();
            }
        }
    }
}
