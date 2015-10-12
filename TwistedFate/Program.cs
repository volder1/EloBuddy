namespace TwistedBuddy
{
    using System;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;

    using SharpDX;

    internal class Program
    {
        /// <summary>
        /// Q
        /// </summary>
        public static Spell.Skillshot Q;

        /// <summary>
        /// W
        /// </summary>
        public static Spell.Targeted W;

        /// <summary>
        /// E
        /// </summary>
        public static Spell.Targeted E;

        /// <summary>
        /// R
        /// </summary>
        public static Spell.Active R;

        /// <summary>
        /// Twisted Fate's Name
        /// </summary>
        public const string ChampionName = "TwistedFate";

        /// <summary>
        /// Called when program starts
        /// </summary>
        private static void Main()
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        /// <summary>
        /// Called when the game finishes loading.
        /// </summary>
        /// <param name="args">The Args.</param>
        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.BaseSkinName != ChampionName)
            {
                return;
            }

            Bootstrap.Init(null);

            Q = new Spell.Skillshot(SpellSlot.Q, 1450, SkillShotType.Linear, 250, 1000, 40);
            R = new Spell.Active(SpellSlot.R, 5500);

            // Menu
            Essentials.MainMenu = MainMenu.AddMenu("Twisted Fate", "TwistedFate");

            // Card Selector Menu
            Essentials.CardSelectorMenu = Essentials.MainMenu.AddSubMenu("Card Selector Menu", "csMenu");
            Essentials.CardSelectorMenu.AddGroupLabel("Card Selector Settings");
            Essentials.CardSelectorMenu.Add("useY", new KeyBind("Use Yellow Card", false, KeyBind.BindTypes.HoldActive, "W".ToCharArray()[0]));
            Essentials.CardSelectorMenu.Add("useB", new KeyBind("Use Blue Card", false, KeyBind.BindTypes.HoldActive, "E".ToCharArray()[0]));
            Essentials.CardSelectorMenu.Add("useR", new KeyBind("Use Red Card", false, KeyBind.BindTypes.HoldActive, "T".ToCharArray()[0]));

            // Combo
            Essentials.ComboMenu = Essentials.MainMenu.AddSubMenu("Combo Menu", "comboMenu");
            Essentials.ComboMenu.AddGroupLabel("Combo Settings");
            Essentials.ComboMenu.Add("useQ", new CheckBox("Use Q in Combo"));
            Essentials.ComboMenu.Add("useCard", new CheckBox("Use W in Combo"));
            Essentials.ComboMenu.Add("useQStun", new CheckBox("Use Q only if Stunned"));
            Essentials.ComboMenu.Add("manaManagerQ", new Slider("How much mana before using Q", 25));
            Essentials.ComboMenu.AddSeparator();

            var comboCardChooserSlider = Essentials.ComboMenu.Add("chooser", new Slider("mode", 0, 0, 3));
            var comboCardArray = new[] { "Smart", "Blue", "Red", "Yellow" };
            comboCardChooserSlider.DisplayName = comboCardArray[comboCardChooserSlider.CurrentValue];
            comboCardChooserSlider.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
            {
                sender.DisplayName = comboCardArray[changeArgs.NewValue];
            };

            // Harass Menu
            Essentials.HarassMenu = Essentials.MainMenu.AddSubMenu("Harass Menu", "harassMenu");
            Essentials.HarassMenu.AddGroupLabel("Harass Settings");
            Essentials.HarassMenu.Add("useQ", new CheckBox("Use Q in Harass"));
            Essentials.HarassMenu.Add("useCard", new CheckBox("Use W in Harass"));
            Essentials.HarassMenu.Add("manaManagerQ", new Slider("How much mana before using Q", 25));
            Essentials.HarassMenu.AddSeparator();

            var harassCardChooserSlider = Essentials.HarassMenu.Add("chooser", new Slider("mode", 0, 0, 3));
            var harassCardArray = new[] { "Smart", "Blue", "Red", "Yellow" };
            harassCardChooserSlider.DisplayName = harassCardArray[harassCardChooserSlider.CurrentValue];
            harassCardChooserSlider.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
            {
                sender.DisplayName = harassCardArray[changeArgs.NewValue];
            };

            // Lane Clear Menu
            Essentials.LaneClearMenu = Essentials.MainMenu.AddSubMenu("Lane Clear", "laneclearMenu");
            Essentials.LaneClearMenu.AddGroupLabel("LaneClear Settings");
            Essentials.LaneClearMenu.Add("useQ", new CheckBox("Use Q in LaneClear", false));
            Essentials.LaneClearMenu.Add("useCard", new CheckBox("Use W in LaneClear"));
            Essentials.LaneClearMenu.Add("manaManagerQ", new Slider("How much mana before using Q", 50));
            Essentials.LaneClearMenu.AddSeparator();

            var laneclearCardChooserSlider = Essentials.LaneClearMenu.Add("chooser", new Slider("mode", 0, 0, 3));
            var laneclearCardArray = new[] { "Smart", "Blue", "Red", "Yellow" };
            laneclearCardChooserSlider.DisplayName = laneclearCardArray[laneclearCardChooserSlider.CurrentValue];
            laneclearCardChooserSlider.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
            {
                sender.DisplayName = laneclearCardArray[changeArgs.NewValue];
            };
            
            // Jungle Clear Menu
            Essentials.JungleClearMenu = Essentials.MainMenu.AddSubMenu("Jungle Clear Menu", "jgMenu");
            Essentials.JungleClearMenu.AddGroupLabel("JungleClear Settings");
            Essentials.JungleClearMenu.Add("useQ", new CheckBox("Use Q in JungleClear", false));
            Essentials.JungleClearMenu.Add("useCard", new CheckBox("Use W in JungleClear"));
            Essentials.JungleClearMenu.Add("manaManagerQ", new Slider("How much mana before using Q", 50));
            Essentials.JungleClearMenu.AddSeparator();

            var jungleclearCardChooserSlider = Essentials.JungleClearMenu.Add("chooser", new Slider("mode", 0, 0, 3));
            var jungleclearCardArray = new[] { "Smart", "Blue", "Red", "Yellow" };
            jungleclearCardChooserSlider.DisplayName = jungleclearCardArray[jungleclearCardChooserSlider.CurrentValue];
            jungleclearCardChooserSlider.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
            {
                sender.DisplayName = jungleclearCardArray[changeArgs.NewValue];
            };

            // Kill Steal Menu
            Essentials.KillStealMenu = Essentials.MainMenu.AddSubMenu("Kill Steal Menu", "ksMenu");
            Essentials.KillStealMenu.AddGroupLabel("KillSteal Settings");
            Essentials.KillStealMenu.Add("useQ", new CheckBox("Use Q to KS"));
            Essentials.KillStealMenu.Add("manaManagerQ", new Slider("How much mana before using Q", 15));
            Essentials.KillStealMenu.AddSeparator();

            // Drawing Menu
            Essentials.DrawingMenu = Essentials.MainMenu.AddSubMenu("Drawing Menu", "drawMenu");
            Essentials.DrawingMenu.AddGroupLabel("Drawing Settings");
            Essentials.DrawingMenu.Add("drawQ", new CheckBox("Draw Q Range"));
            Essentials.DrawingMenu.Add("drawR", new CheckBox("Draw R Range"));
            Essentials.DrawingMenu.AddSeparator();

            // Misc Menu
            Essentials.MiscMenu = Essentials.MainMenu.AddSubMenu("Misc Menu", "miscMenu");
            Essentials.MiscMenu.AddGroupLabel("Misc Settings");
            Essentials.MiscMenu.Add("autoQ", new CheckBox("Automatically Q's a CCed Target"));
            Essentials.MiscMenu.Add("autoY", new CheckBox("Automatically select Yellow Card when R"));
            Essentials.MiscMenu.Add("manaW", new Slider("How much mana before selecting Blue Card", 25));
            Essentials.MiscMenu.Add("delay", new Slider("Delay Card Choosing", 800, 175, 1000));

            Chat.Print("TwistedBuddy 2.0.0.0 - By KarmaPanda", Color.Green);

            // Events
            Game.OnTick += Game_OnTick;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        /// <summary>
        /// Called on Spell Cast
        /// </summary>
        /// <param name="sender">The Person who casted a spell</param>
        /// <param name="args">The Args</param>
        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "gate" && Essentials.MiscMenu["autoY"].Cast<CheckBox>().CurrentValue)
            {
                CardSelector.StartSelecting(Cards.Yellow);
            }
        }

        /// <summary>
        /// Called when game draws.
        /// </summary>
        /// <param name="args">The Args.</param>
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Essentials.DrawingMenu["drawQ"].Cast<CheckBox>().CurrentValue)
            {
                if (Player.Instance != null)
                {
                    Circle.Draw(Q.IsReady() ? Color.Green : Color.Red, Q.Range, Player.Instance.Position);
                }
            }

            if (!Essentials.DrawingMenu["drawR"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            if (Player.Instance != null)
            {
                Circle.Draw(R.IsReady() ? Color.Green : Color.Red, R.Range, Player.Instance.Position);
            }
        }

        /// <summary>
        /// Called when game updates.
        /// </summary>
        /// <param name="args">The Args.</param>
        private static void Game_OnTick(EventArgs args)
        {
            var useY = Essentials.CardSelectorMenu["useY"].Cast<KeyBind>().CurrentValue;
            var useB = Essentials.CardSelectorMenu["useB"].Cast<KeyBind>().CurrentValue;
            var useR = Essentials.CardSelectorMenu["useR"].Cast<KeyBind>().CurrentValue;

            if (useY)
            {
                CardSelector.StartSelecting(Cards.Yellow);
            }
            if (useB)
            {
                CardSelector.StartSelecting(Cards.Blue);
            }
            if (useR)
            {
                CardSelector.StartSelecting(Cards.Red);
            }

            if (Essentials.MiscMenu["autoQ"].Cast<CheckBox>().CurrentValue)
            {
                StateManager.AutoQ();
            }

            if (Essentials.KillStealMenu["useQ"].Cast<CheckBox>().CurrentValue)
            {
                StateManager.KillSteal();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                StateManager.Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                StateManager.LaneClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                StateManager.JungleClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                StateManager.Harass();
            }
        }
    }
}
