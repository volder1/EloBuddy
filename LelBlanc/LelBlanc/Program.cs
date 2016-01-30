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
        /// Contains the Color Picker
        /// </summary>
        public static ColorPicker Picker;

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
            if (Player.Instance.Hero != Champion.Leblanc)
            {
                return;
            }

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

            // Methods
            Config.Initialize();

            // Constructors
            Picker = new ColorPicker(Config.DrawingMenu, "draw_", System.Drawing.Color.FromArgb(255, 255, 0, 0), "Color Settings for Damage Indicator");
            Indicator = new DamageIndicator();

            // Events
            Game.OnUpdate += Game_OnUpdate;
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
            if (Config.MiscMenu["pet"].Cast<CheckBox>().CurrentValue)
            {
                Pet.MovePet();
            }
            if (Config.KillStealMenu["toggle"].Cast<CheckBox>().CurrentValue)
            {
                Modes.KillSteal.Execute();
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
