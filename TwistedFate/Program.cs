namespace TwistedBuddy
{
    using System;
    using System.Drawing;
    using System.Collections.Generic;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using SharpDX;

    class Program
    {
        /// <summary>
        /// Twisted Fate's Name
        /// </summary>
        public const string ChampionName = "TwistedFate";

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
        /// Menu
        /// </summary>
        private static Menu TwistedFate, CardSelectorMenu, ComboMenu, LaneClearMenu, JungleClearMenu, HarassMenu, KillStealMenu, DrawingMenu, MiscMenu;

        /// <summary>
        /// Gets the player.
        /// </summary>
        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        /// <summary>
        /// Called when program starts
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.BaseSkinName != ChampionName)
            {
                return;
            }

            Bootstrap.Init(null);

            Q = new Spell.Skillshot(SpellSlot.Q, 1450, SkillShotType.Linear, 250, 1000, 40);
            R = new Spell.Active(SpellSlot.R, 5500);

            // Menu
            TwistedFate = MainMenu.AddMenu("Twisted Fate", "TwistedFate");

            // Card Selector Menu
            CardSelectorMenu = TwistedFate.AddSubMenu("Card Selector Menu", "csMenu");
            CardSelectorMenu.AddGroupLabel("Card Selector Settings");
            CardSelectorMenu.Add("useY", new KeyBind("Use Yellow Card", false, KeyBind.BindTypes.HoldActive, "W".ToCharArray()[0]));
            CardSelectorMenu.Add("useB", new KeyBind("Use Blue Card", false, KeyBind.BindTypes.HoldActive, "E".ToCharArray()[0]));
            CardSelectorMenu.Add("useR", new KeyBind("Use Red Card", false, KeyBind.BindTypes.HoldActive, "T".ToCharArray()[0]));

            // Combo
            ComboMenu = TwistedFate.AddSubMenu("Combo Menu", "comboMenu");
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.Add("useQ", new CheckBox("Use Q in Combo", true));
            ComboMenu.Add("useCard", new CheckBox("Use W in Combo", true));
            ComboMenu.Add("manaManagerQ", new Slider("How much mana before using Q", 25, 0, 100));
            ComboMenu.AddSeparator();

            var comboCardChooserSlider = ComboMenu.Add("chooser", new Slider("mode", 0, 0, 3));
            var comboCardArray = new[] { "Smart", "Blue", "Red", "Yellow" };
            comboCardChooserSlider.DisplayName = comboCardArray[comboCardChooserSlider.CurrentValue];
            comboCardChooserSlider.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
            {
                sender.DisplayName = comboCardArray[changeArgs.NewValue];
            };

            // Harass Menu
            HarassMenu = TwistedFate.AddSubMenu("Harass Menu", "harassMenu");
            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.Add("useQ", new CheckBox("Use Q in Harass", true));
            HarassMenu.Add("useCard", new CheckBox("Use W in Harass", true));
            HarassMenu.Add("manaManagerQ", new Slider("How much mana before using Q", 25, 0, 100));
            HarassMenu.AddSeparator();

            var harassCardChooserSlider = HarassMenu.Add("chooser", new Slider("mode", 0, 0, 3));
            var harassCardArray = new[] { "Smart", "Blue", "Red", "Yellow" };
            harassCardChooserSlider.DisplayName = harassCardArray[harassCardChooserSlider.CurrentValue];
            harassCardChooserSlider.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
            {
                sender.DisplayName = harassCardArray[changeArgs.NewValue];
            };

            // Lane Clear Menu
            LaneClearMenu = TwistedFate.AddSubMenu("Lane Clear", "laneclearMenu");
            LaneClearMenu.AddGroupLabel("LaneClear Settings");
            LaneClearMenu.Add("useQ", new CheckBox("Use Q in LaneClear", false));
            LaneClearMenu.Add("useCard", new CheckBox("Use W in LaneClear", true));
            LaneClearMenu.Add("manaManagerQ", new Slider("How much mana before using Q", 50, 0, 100));
            LaneClearMenu.AddSeparator();

            var laneclearCardChooserSlider = LaneClearMenu.Add("chooser", new Slider("mode", 0, 0, 3));
            var laneclearCardArray = new[] { "Smart", "Blue", "Red", "Yellow" };
            laneclearCardChooserSlider.DisplayName = laneclearCardArray[laneclearCardChooserSlider.CurrentValue];
            laneclearCardChooserSlider.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
            {
                sender.DisplayName = laneclearCardArray[changeArgs.NewValue];
            };
            
            // Jungle Clear Menu
            JungleClearMenu = TwistedFate.AddSubMenu("Jungle Clear Menu", "jgMenu");
            JungleClearMenu.AddGroupLabel("JungleClear Settings");
            JungleClearMenu.Add("useQ", new CheckBox("Use Q in JungleClear", false));
            JungleClearMenu.Add("useCard", new CheckBox("Use W in LaneClear", true));
            JungleClearMenu.Add("manaManagerQ", new Slider("How much mana before using Q", 50, 0, 100));
            JungleClearMenu.AddSeparator();

            var jungleclearCardChooserSlider = JungleClearMenu.Add("chooser", new Slider("mode", 0, 0, 3));
            var jungleclearCardArray = new[] { "Smart", "Blue", "Red", "Yellow" };
            jungleclearCardChooserSlider.DisplayName = jungleclearCardArray[jungleclearCardChooserSlider.CurrentValue];
            jungleclearCardChooserSlider.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
            {
                sender.DisplayName = jungleclearCardArray[changeArgs.NewValue];
            };

            // Kill Steal Menu
            KillStealMenu = TwistedFate.AddSubMenu("Kill Steal Menu", "ksMenu");
            KillStealMenu.AddGroupLabel("KillSteal Settings");
            KillStealMenu.Add("useQ", new CheckBox("Use Q to KS", true));
            KillStealMenu.Add("manaManagerQ", new Slider("How much mana before using Q", 15, 0, 100));
            KillStealMenu.AddSeparator();

            // Drawing Menu
            DrawingMenu = TwistedFate.AddSubMenu("Drawing Menu", "drawMenu");
            DrawingMenu.AddGroupLabel("Drawing Settings");
            DrawingMenu.Add("drawQ", new CheckBox("Draw Q Range", true));
            DrawingMenu.Add("drawR", new CheckBox("Draw R Range", true));
            DrawingMenu.AddSeparator();

            MiscMenu = TwistedFate.AddSubMenu("Misc Menu", "miscMenu");
            MiscMenu.AddGroupLabel("Misc Settings");
            MiscMenu.Add("autoQ", new CheckBox("Automatically Q's a CCed Target", true));
            MiscMenu.Add("autoY", new CheckBox("Automatically select Yellow Card when R", true));
            MiscMenu.Add("manaW", new Slider("How much mana before selecting Blue Card", 25, 0, 100));

            Chat.Print("Advanced Twisted Fate - By KarmaPanda", System.Drawing.Color.Green);

            // Events
            Game.OnTick += Game_OnTick;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "gate" && MiscMenu["autoY"].Cast<CheckBox>().CurrentValue)
            {
                CardSelector.StartSelecting(Cards.Yellow);
            }
        }

        /// <summary>
        /// Post Attack
        /// </summary>
        /// <param name="target">The Target Orbwalker is Aiming For</param>
        /// <param name="args">The Attack Arg.</param>
        static void Orbwalker_OnPreAttack(AttackableUnit target, EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                var t = target as AIHeroClient;
                var useCard = ComboMenu["useCard"].Cast<CheckBox>().CurrentValue;
                var chooser = ComboMenu["chooser"].Cast<Slider>().DisplayName;

                if (useCard && t != null)
                {
                    switch (chooser)
                    {
                        case "Smart":
                            var selectedCard = heroCardSelection(t);
                            Combo(t, selectedCard);
                            break;
                        default:
                            Combo(t, chooser);
                            break;
                    }
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                var minion = target as Obj_AI_Base;
                var useCard = LaneClearMenu["useCard"].Cast<CheckBox>().CurrentValue;
                var chooser = LaneClearMenu["chooser"].Cast<Slider>().DisplayName;

                if (useCard && minion != null)
                {
                    switch (chooser)
                    {
                        case "Smart":
                            var selectedCard = minionCardSelection(minion);
                            LaneClear(minion, selectedCard);
                            break;
                        default:
                            LaneClear(minion, chooser);
                            break;
                    }
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                var minion = target as Obj_AI_Base;
                var useCard = JungleClearMenu["useCard"].Cast<CheckBox>().CurrentValue;
                var chooser = JungleClearMenu["chooser"].Cast<Slider>().DisplayName;

                if (useCard && minion != null)
                {
                    switch (chooser)
                    {
                        case "Smart":
                            var selectedCard = minionCardSelection(minion);
                            JungleClear(minion, selectedCard);
                            break;
                        default:
                            JungleClear(minion, chooser);
                            break;
                    }
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                var t = target as AIHeroClient;
                var m = target as Obj_AI_Base;
                var useCard = HarassMenu["useCard"].Cast<CheckBox>().CurrentValue;
                var chooser = ComboMenu["chooser"].Cast<Slider>().DisplayName;

                if (useCard && t != null)
                {
                    switch (chooser)
                    {
                        case "Smart":
                            var selectedCard = minionCardSelection(m);
                            Harass(m, selectedCard);
                            break;
                        default:
                            Harass(m, chooser);
                            break;
                    }
                }

                if (useCard && m != null)
                {
                    if (m.Health <= ObjectManager.Player.GetAutoAttackDamage(m) + Player.GetSpellDamage(m, SpellSlot.W))
                    {
                        switch (chooser)
                        {
                            case "Smart":
                                var selectedCard = heroCardSelection(t);
                                Harass(t, selectedCard);
                                break;
                            default:
                                Harass(t, chooser);
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Does Auto Q
        /// </summary>
        static void AutoQ()
        {
            var heroes = HeroManager.Enemies.Where(t => t.IsFeared || t.IsCharmed || t.IsTaunted || t.IsRecalling);

            if (heroes != null)
            {
                foreach (var t in heroes)
                {
                    var pred = Q.GetPrediction(t);

                    if (pred.HitChance == HitChance.High)
                    {
                        Q.Cast(pred.CastPosition);
                    }
                }
            }
        }

        /// <summary>
        /// Does Combo
        /// </summary>
        /// <param name="t">The Target</param>
        static void Combo(AIHeroClient t, string selectedCard)
        {
            if (t == null)
            {
                return;
            }

            switch (selectedCard)
            {
                case "Red":
                    CardSelector.StartSelecting(Cards.Red);
                    break;
                case "Yellow":
                    CardSelector.StartSelecting(Cards.Yellow);
                    break;
                case "Blue":
                    CardSelector.StartSelecting(Cards.Blue);
                    break;
                default:
                    return;
            }
        }

        /// <summary>
        /// Does Harass with Minion
        /// </summary>
        /// <param name="t">The Minion</param>
        static void Harass(Obj_AI_Base t, string selectedCard)
        {
            if (t == null)
            {
                return;
            }

            switch(selectedCard)
            {
                case "Red":
                    CardSelector.StartSelecting(Cards.Red);
                    break;
                case "Yellow":
                    CardSelector.StartSelecting(Cards.Yellow);
                    break;
                case "Blue":
                    CardSelector.StartSelecting(Cards.Blue);
                    break;
                default:
                    return;
            }
        }

        /// <summary>
        /// Does Harass with Target
        /// </summary>
        /// <param name="t">The Target</param>
        static void Harass(AIHeroClient t, string selectedCard)
        {
            if (t == null)
            {
                return;
            }

            switch (selectedCard)
            {
                case "Red":
                    CardSelector.StartSelecting(Cards.Red);
                    break;
                case "Yellow":
                    CardSelector.StartSelecting(Cards.Yellow);
                    break;
                case "Blue":
                    CardSelector.StartSelecting(Cards.Blue);
                    break;
                default:
                    return;
            }
        }

        /// <summary>
        /// Does LaneClear
        /// </summary>
        /// <param name="t">The Target</param>
        static void LaneClear(Obj_AI_Base t, string selectedCard)
        {
            if (t == null)
            {
                return;
            }

            switch (selectedCard)
            {
                case "Red":
                    CardSelector.StartSelecting(Cards.Red);
                    break;
                case "Yellow":
                    CardSelector.StartSelecting(Cards.Yellow);
                    break;
                case "Blue":
                    CardSelector.StartSelecting(Cards.Blue);
                    break;
                default:
                    return;
            }
        }

        /// <summary>
        /// Does LaneClear
        /// </summary>
        /// <param name="t">The Target</param>
        static void JungleClear(Obj_AI_Base t, string selectedCard)
        {
            if (t == null)
            {
                return;
            }

            switch (selectedCard)
            {
                case "Red":
                    CardSelector.StartSelecting(Cards.Red);
                    break;
                case "Yellow":
                    CardSelector.StartSelecting(Cards.Yellow);
                    break;
                case "Blue":
                    CardSelector.StartSelecting(Cards.Blue);
                    break;
                default:
                    return;
            }
        }

        /// <summary>
        /// Returns the card that should be picked in that scenario
        /// </summary>
        /// <param name="t">The Target</param>
        /// <returns>The Card that should be used.</returns>
        static string minionCardSelection(Obj_AI_Base t)
        {
            string card = null;
            var minionsaroundTarget = ObjectManager.Get<Obj_AI_Base>().Where(target => target.IsMinion && target.Distance(t) <= 200).Count();
            var manaW = MiscMenu["manaW"].Cast<Slider>().CurrentValue;

            if (t.IsAlly)
            {
                return card;
            }

            if (Player.ManaPercent <= manaW || t.IsStructure())
            {
                card = "Blue";
                return card;
            }
            else if (Player.ManaPercent > 25 
                && t.Team == GameObjectTeam.Neutral 
                && (t.Name == "SRU_Blue" 
                || t.Name == "SRU_Gromp" 
                || t.Name == "SRU_Murkwolf" 
                || t.Name == "SRU_Razorbeak" 
                || t.Name == "SRU_Red") 
                && minionsaroundTarget <= 2)
            {
                card = "Yellow";
                return card;
            }
            else if (Player.ManaPercent > 25 && minionsaroundTarget > 2)
            {
                card = "Red";
                return card;
            }
            else
            {
                return card;
            }
        }

        /// <summary>
        /// Returns the card that should be picked in that scenario
        /// </summary>
        /// <param name="t">The Target</param>
        /// <returns>The Card that should be used.</returns>
        static string heroCardSelection(Obj_AI_Base t)
        {
            string card = null;
            var alliesaroundTarget = HeroManager.Enemies.Where(target => target.Distance(t) <= 200).Count();
            var manaW = MiscMenu["manaW"].Cast<Slider>().CurrentValue;

            if (t.IsAlly)
            {
                return card;
            }

            if (Player.ManaPercent <= manaW)
            {
                card = "Blue";
                return card;
            }
            else if (Player.ManaPercent > 25 && alliesaroundTarget >= 2)
            {
                card = "Red";
                return card;
            }
            else if (Player.ManaPercent > 25 && alliesaroundTarget == 1)
            {
                card = "Yellow";
                return card;
            }
            else
            {
                return card;
            }
        }

        /// <summary>
        /// Called when game draws.
        /// </summary>
        /// <param name="args">The Args.</param>
        static void Drawing_OnDraw(EventArgs args)
        {
            if (DrawingMenu["drawQ"].Cast<CheckBox>().CurrentValue)
            {
                if (Player != null)
                {
                    EloBuddy.SDK.Rendering.Circle.Draw(Q.IsReady() ? SharpDX.Color.Green : SharpDX.Color.Red, Q.Range, Player.Position);
                }
            }

            if (DrawingMenu["drawR"].Cast<CheckBox>().CurrentValue)
            {
                if (Player != null)
                {
                    EloBuddy.SDK.Rendering.Circle.Draw(R.IsReady() ? SharpDX.Color.Green : SharpDX.Color.Red, R.Range, Player.Position);
                }
            }
        }

        /// <summary>
        /// Called when game updates.
        /// </summary>
        /// <param name="args">The Args.</param>
        static void Game_OnTick(EventArgs args)
        {
            var useY = CardSelectorMenu["useY"].Cast<KeyBind>();
            var useB = CardSelectorMenu["useB"].Cast<KeyBind>();
            var useR = CardSelectorMenu["useR"].Cast<KeyBind>();

            if (useY.CurrentValue)
            {
                CardSelector.StartSelecting(Cards.Yellow);
                useY.CurrentValue = false;
            }
            if (useB.CurrentValue)
            {
                CardSelector.StartSelecting(Cards.Blue);
                useB.CurrentValue = false;
            }
            if (useR.CurrentValue)
            {
                CardSelector.StartSelecting(Cards.Red);
                useR.CurrentValue = false;
            }

            var autoQ = MiscMenu["autoQ"].Cast<CheckBox>().CurrentValue;

            if (autoQ)
            {
                AutoQ();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                var qTarget = TargetSelector.GetTarget(Q.Range, DamageType.Mixed);
                var useQ = ComboMenu["useQ"].Cast<CheckBox>().CurrentValue;
                var manaManagerQ = ComboMenu["manaManagerQ"].Cast<Slider>().CurrentValue;

                // Cast Q if possible.
                if (useQ && qTarget != null)
                {
                    if (Q.IsInRange(qTarget) && Q.IsReady() && Player.ManaPercent >= manaManagerQ)
                    {
                        var pred = Q.GetPrediction(qTarget);

                        if (pred.HitChance == HitChance.High)
                        {
                            Q.Cast(pred.CastPosition);
                        }
                    }
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                var qMinion = EntityManager.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Position.To2D(), Q.Range, false).OrderBy(t => t.Health).FirstOrDefault();//ObjectManager.Get<Obj_AI_Base>().Where(t => t.IsEnemy && t.IsMinion && Q.IsInRange(t)).OrderBy(t => t.Health).FirstOrDefault();
                var useQ = LaneClearMenu["useQ"].Cast<CheckBox>().CurrentValue;
                var manaManagerQ = LaneClearMenu["manaManagerQ"].Cast<Slider>().CurrentValue;

                // Cast Q if possible.
                if (useQ && qMinion != null)
                {
                    if (Q.IsReady() && Player.ManaPercent >= manaManagerQ)
                    {
                        var minionPrediction = Prediction.Position.PredictUnitPosition(qMinion, Q.Speed);

                        if (minionPrediction != null && Player.IsFacing(qMinion))
                        {
                            Q.Cast(minionPrediction.To3D());
                        }
                        /*var pred = Q.GetPrediction(qMinion);

                        if (pred.HitChance == HitChance.High)
                        {
                            Q.Cast(qMinion);
                        }*/
                    }
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                var qMinion = EntityManager.GetJungleMonsters(Player.Position.To2D(), Q.Range, false).OrderByDescending(t => t.Health).FirstOrDefault();//ObjectManager.Get<Obj_AI_Base>().Where(t => t.Team == GameObjectTeam.Neutral && Q.IsInRange(t)).OrderBy(t => t.MaxHealth).FirstOrDefault();
                var useQ = JungleClearMenu["useQ"].Cast<CheckBox>().CurrentValue;
                var manaManagerQ = JungleClearMenu["manaManagerQ"].Cast<Slider>().CurrentValue;


                // Cast Q if possible.
                if (useQ && qMinion != null)
                {
                    if (Q.IsReady() && Player.ManaPercent >= manaManagerQ)
                    {
                        var minionPrediction = Prediction.Position.PredictUnitPosition(qMinion, Q.Speed);

                        if (minionPrediction != null && Player.IsFacing(qMinion))
                        {
                            Q.Cast(minionPrediction.To3D());
                        }
                        /*var pred = Q.GetPrediction(qMinion);

                        if (pred.HitChance == HitChance.High)
                        {
                            Q.Cast(qMinion);
                        }*/
                    }
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                var qTarget = TargetSelector.GetTarget(Q.Range, DamageType.Mixed);
                var useQ = HarassMenu["useQ"].Cast<CheckBox>().CurrentValue;
                var manaManagerQ = HarassMenu["manaManagerQ"].Cast<Slider>().CurrentValue;

                if (useQ)
                {
                    if (Q.IsInRange(qTarget) && Q.IsReady() && Player.ManaPercent >= manaManagerQ)
                    {
                        var pred = Q.GetPrediction(qTarget);
                        Q.Cast(pred.CastPosition);
                    }
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None))
            {
                var useQ = KillStealMenu["useQ"].Cast<CheckBox>().CurrentValue;
                var manaManagerQ = KillStealMenu["manaManagerQ"].Cast<Slider>().CurrentValue;

                if (useQ)
                {
                    var t = TargetSelector.GetTarget(Q.Range, DamageType.Mixed);

                    if (t != null && Q.IsReady())
                    {
                        if (t.Health < Player.GetSpellDamage(t, SpellSlot.Q) && Player.ManaPercent >= manaManagerQ)
                        {
                            var pred = Q.GetPrediction(t);
                            Q.Cast(pred.CastPosition);
                        }
                    }
                }
            }
        }
    }
}
