namespace TwistedBuddy
{
    using System;
    using System.Drawing;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;

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
        private static Menu mainMenu, cardSelectorMenu, comboMenu, laneClearMenu, jungleClearMenu, harassMenu, killStealMenu, drawingMenu, miscMenu;

        /// <summary>
        /// Gets the player.
        /// </summary>
        private static AIHeroClient PlayerInstance
        {
            get { return ObjectManager.Player; }
        }

        /// <summary>
        /// Called when program starts
        /// </summary>
        static void Main()
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        /// <summary>
        /// Called when the game finishes loading.
        /// </summary>
        /// <param name="args">The Args.</param>
        static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (PlayerInstance.BaseSkinName != ChampionName)
            {
                return;
            }

            Bootstrap.Init(null);

            Q = new Spell.Skillshot(SpellSlot.Q, 1450, SkillShotType.Linear, 250, 1000, 40);
            R = new Spell.Active(SpellSlot.R, 5500);

            // Menu
            mainMenu = MainMenu.AddMenu("Twisted Fate", "TwistedFate");

            // Card Selector Menu
            cardSelectorMenu = mainMenu.AddSubMenu("Card Selector Menu", "csMenu");
            cardSelectorMenu.AddGroupLabel("Card Selector Settings");
            cardSelectorMenu.Add("useY", new KeyBind("Use Yellow Card", false, KeyBind.BindTypes.HoldActive, "W".ToCharArray()[0]));
            cardSelectorMenu.Add("useB", new KeyBind("Use Blue Card", false, KeyBind.BindTypes.HoldActive, "E".ToCharArray()[0]));
            cardSelectorMenu.Add("useR", new KeyBind("Use Red Card", false, KeyBind.BindTypes.HoldActive, "T".ToCharArray()[0]));

            // Combo
            comboMenu = mainMenu.AddSubMenu("Combo Menu", "comboMenu");
            comboMenu.AddGroupLabel("Combo Settings");
            comboMenu.Add("useQ", new CheckBox("Use Q in Combo"));
            comboMenu.Add("useCard", new CheckBox("Use W in Combo"));
            comboMenu.Add("useQStun", new CheckBox("Use Q only if Stunned"));
            comboMenu.Add("manaManagerQ", new Slider("How much mana before using Q", 25));
            comboMenu.AddSeparator();

            var comboCardChooserSlider = comboMenu.Add("chooser", new Slider("mode", 0, 0, 3));
            var comboCardArray = new[] { "Smart", "Blue", "Red", "Yellow" };
            comboCardChooserSlider.DisplayName = comboCardArray[comboCardChooserSlider.CurrentValue];
            comboCardChooserSlider.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
            {
                sender.DisplayName = comboCardArray[changeArgs.NewValue];
            };

            // Harass Menu
            harassMenu = mainMenu.AddSubMenu("Harass Menu", "harassMenu");
            harassMenu.AddGroupLabel("Harass Settings");
            harassMenu.Add("useQ", new CheckBox("Use Q in Harass"));
            harassMenu.Add("useCard", new CheckBox("Use W in Harass"));
            harassMenu.Add("manaManagerQ", new Slider("How much mana before using Q", 25));
            harassMenu.AddSeparator();

            var harassCardChooserSlider = harassMenu.Add("chooser", new Slider("mode", 0, 0, 3));
            var harassCardArray = new[] { "Smart", "Blue", "Red", "Yellow" };
            harassCardChooserSlider.DisplayName = harassCardArray[harassCardChooserSlider.CurrentValue];
            harassCardChooserSlider.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
            {
                sender.DisplayName = harassCardArray[changeArgs.NewValue];
            };

            // Lane Clear Menu
            laneClearMenu = mainMenu.AddSubMenu("Lane Clear", "laneclearMenu");
            laneClearMenu.AddGroupLabel("LaneClear Settings");
            laneClearMenu.Add("useQ", new CheckBox("Use Q in LaneClear", false));
            laneClearMenu.Add("useCard", new CheckBox("Use W in LaneClear"));
            laneClearMenu.Add("manaManagerQ", new Slider("How much mana before using Q", 50));
            laneClearMenu.AddSeparator();

            var laneclearCardChooserSlider = laneClearMenu.Add("chooser", new Slider("mode", 0, 0, 3));
            var laneclearCardArray = new[] { "Smart", "Blue", "Red", "Yellow" };
            laneclearCardChooserSlider.DisplayName = laneclearCardArray[laneclearCardChooserSlider.CurrentValue];
            laneclearCardChooserSlider.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
            {
                sender.DisplayName = laneclearCardArray[changeArgs.NewValue];
            };
            
            // Jungle Clear Menu
            jungleClearMenu = mainMenu.AddSubMenu("Jungle Clear Menu", "jgMenu");
            jungleClearMenu.AddGroupLabel("JungleClear Settings");
            jungleClearMenu.Add("useQ", new CheckBox("Use Q in JungleClear", false));
            jungleClearMenu.Add("useCard", new CheckBox("Use W in LaneClear"));
            jungleClearMenu.Add("manaManagerQ", new Slider("How much mana before using Q", 50));
            jungleClearMenu.AddSeparator();

            var jungleclearCardChooserSlider = jungleClearMenu.Add("chooser", new Slider("mode", 0, 0, 3));
            var jungleclearCardArray = new[] { "Smart", "Blue", "Red", "Yellow" };
            jungleclearCardChooserSlider.DisplayName = jungleclearCardArray[jungleclearCardChooserSlider.CurrentValue];
            jungleclearCardChooserSlider.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
            {
                sender.DisplayName = jungleclearCardArray[changeArgs.NewValue];
            };

            // Kill Steal Menu
            killStealMenu = mainMenu.AddSubMenu("Kill Steal Menu", "ksMenu");
            killStealMenu.AddGroupLabel("KillSteal Settings");
            killStealMenu.Add("useQ", new CheckBox("Use Q to KS"));
            killStealMenu.Add("manaManagerQ", new Slider("How much mana before using Q", 15));
            killStealMenu.AddSeparator();

            // Drawing Menu
            drawingMenu = mainMenu.AddSubMenu("Drawing Menu", "drawMenu");
            drawingMenu.AddGroupLabel("Drawing Settings");
            drawingMenu.Add("drawQ", new CheckBox("Draw Q Range"));
            drawingMenu.Add("drawR", new CheckBox("Draw R Range"));
            drawingMenu.AddSeparator();

            miscMenu = mainMenu.AddSubMenu("Misc Menu", "miscMenu");
            miscMenu.AddGroupLabel("Misc Settings");
            miscMenu.Add("autoQ", new CheckBox("Automatically Q's a CCed Target"));
            miscMenu.Add("autoY", new CheckBox("Automatically select Yellow Card when R"));
            miscMenu.Add("manaW", new Slider("How much mana before selecting Blue Card", 25));

            Chat.Print("Advanced Twisted Fate - By KarmaPanda", Color.Green);

            // Events
            Game.OnTick += Game_OnTick;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        /// <summary>
        /// Called on Spell Cast
        /// </summary>
        /// <param name="sender">The Person who casted a spell</param>
        /// <param name="args">The Args</param>
        static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "gate" && miscMenu["autoY"].Cast<CheckBox>().CurrentValue)
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
                /*var t = target as AIHeroClient;
                var useCard = comboMenu["useCard"].Cast<CheckBox>().CurrentValue;
                var chooser = comboMenu["chooser"].Cast<Slider>().DisplayName;

                if (useCard && t != null)
                {
                    switch (chooser)
                    {
                        case "Smart":
                            var selectedCard = HeroCardSelection(t);
                            Combo(t, selectedCard);
                            break;
                        default:
                            Combo(t, chooser);
                            break;
                    }
                }*/
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                var minion = target as Obj_AI_Base;
                var useCard = laneClearMenu["useCard"].Cast<CheckBox>().CurrentValue;
                var chooser = laneClearMenu["chooser"].Cast<Slider>().DisplayName;

                if (useCard && minion != null)
                {
                    switch (chooser)
                    {
                        case "Smart":
                            var selectedCard = MinionCardSelection(minion);
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
                var useCard = jungleClearMenu["useCard"].Cast<CheckBox>().CurrentValue;
                var chooser = jungleClearMenu["chooser"].Cast<Slider>().DisplayName;

                if (useCard && minion != null)
                {
                    switch (chooser)
                    {
                        case "Smart":
                            var selectedCard = MinionCardSelection(minion);
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
                var useCard = harassMenu["useCard"].Cast<CheckBox>().CurrentValue;
                var chooser = comboMenu["chooser"].Cast<Slider>().DisplayName;

                if (useCard && t != null)
                {
                    switch (chooser)
                    {
                        case "Smart":
                            var selectedCard = MinionCardSelection(m);
                            Harass(m, selectedCard);
                            break;
                        default:
                            Harass(m, chooser);
                            break;
                    }
                }

                if (useCard && m != null)
                {
                    if (m.Health <= ObjectManager.Player.GetAutoAttackDamage(m) + PlayerInstance.GetSpellDamage(m, SpellSlot.W))
                    {
                        switch (chooser)
                        {
                            case "Smart":
                                var selectedCard = HeroCardSelection(t);
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
            //var heroes = HeroManager.Enemies.Where(t => t.IsFeared || t.IsCharmed || t.IsTaunted || t.IsRecalling || t.HasBuff("Stun"));
            var heroes = HeroManager.Enemies.Where(t => t.HasBuff("Stun"));

            if (heroes != null)
            {
                foreach (var t in heroes)
                {
                    if (Q.IsReady())
                    {
                        Q.Cast(t.ServerPosition);
                    }
                }
            }
        }

        /// <summary>
        /// Does Combo
        /// </summary>
        /// <param name="t">The Target</param>
        /// <param name="selectedCard">The Card that is selected.</param>
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
        /// <param name="selectedCard">The Card that is selected.</param>
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
        /// <param name="selectedCard">The Card that is selected.</param>
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
        /// <param name="selectedCard">The Card that is selected.</param>
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
        /// <param name="selectedCard">The Card that is selected.</param>
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
        static string MinionCardSelection(Obj_AI_Base t)
        {
            string card;
            var minionsaroundTarget = ObjectManager.Get<Obj_AI_Base>().Count(target => target.IsMinion && target.Distance(t) <= 200);
            var manaW = miscMenu["manaW"].Cast<Slider>().CurrentValue;

            if (t.IsAlly)
            {
                return null;
            }

            if (PlayerInstance.ManaPercent <= manaW || t.IsStructure())
            {
                card = "Blue";
                return card;
            }
            else if (PlayerInstance.ManaPercent > 25 
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
            else if (PlayerInstance.ManaPercent > 25 && minionsaroundTarget > 2)
            {
                card = "Red";
                return card;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the card that should be picked in that scenario
        /// </summary>
        /// <param name="t">The Target</param>
        /// <returns>The Card that should be used.</returns>
        static string HeroCardSelection(Obj_AI_Base t)
        {
            string card;
            var alliesaroundTarget = HeroManager.Enemies.Count(target => target.Distance(t) <= 200);
            var manaW = miscMenu["manaW"].Cast<Slider>().CurrentValue;

            if (t.IsAlly)
            {
                return null;
            }

            if (PlayerInstance.ManaPercent <= manaW)
            {
                card = "Blue";
                return card;
            }
            else if (PlayerInstance.ManaPercent > 25 && alliesaroundTarget >= 2)
            {
                card = "Red";
                return card;
            }
            else if (PlayerInstance.ManaPercent > 25 && alliesaroundTarget == 1)
            {
                card = "Yellow";
                return card;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Called when game draws.
        /// </summary>
        /// <param name="args">The Args.</param>
        static void Drawing_OnDraw(EventArgs args)
        {
            if (drawingMenu["drawQ"].Cast<CheckBox>().CurrentValue)
            {
                if (PlayerInstance != null)
                {
                    Circle.Draw(Q.IsReady() ? SharpDX.Color.Green : SharpDX.Color.Red, Q.Range, PlayerInstance.Position);
                }
            }

            if (drawingMenu["drawR"].Cast<CheckBox>().CurrentValue)
            {
                if (PlayerInstance != null)
                {
                    Circle.Draw(R.IsReady() ? SharpDX.Color.Green : SharpDX.Color.Red, R.Range, PlayerInstance.Position);
                }
            }
        }

        /// <summary>
        /// Called when game updates.
        /// </summary>
        /// <param name="args">The Args.</param>
        static void Game_OnTick(EventArgs args)
        {          
            var useY = cardSelectorMenu["useY"].Cast<KeyBind>();
            var useB = cardSelectorMenu["useB"].Cast<KeyBind>();
            var useR = cardSelectorMenu["useR"].Cast<KeyBind>();

            if (useY.CurrentValue)
            {
                CardSelector.StartSelecting(Cards.Yellow);
            }
            if (useB.CurrentValue)
            {
                CardSelector.StartSelecting(Cards.Blue);
            }
            if (useR.CurrentValue)
            {
                CardSelector.StartSelecting(Cards.Red);
            }

            if (miscMenu["autoQ"].Cast<CheckBox>().CurrentValue)
            {
                AutoQ();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                var qTarget = TargetSelector.GetTarget(Q.Range, DamageType.Mixed);
                var useQ = comboMenu["useQ"].Cast<CheckBox>().CurrentValue;
                var useQStun = comboMenu["useQStun"].Cast<CheckBox>().CurrentValue;
                var manaManagerQ = comboMenu["manaManagerQ"].Cast<Slider>().CurrentValue;

                var wTarget = TargetSelector.GetTarget(
                    PlayerInstance.AttackRange + 150,
                    DamageType.Mixed,
                    PlayerInstance.ServerPosition);
                var useCard = comboMenu["useCard"].Cast<CheckBox>().CurrentValue;
                var chooser = comboMenu["chooser"].Cast<Slider>().DisplayName;

                if (useCard && wTarget != null)
                {
                    switch (chooser)
                    {
                        case "Smart":
                            var selectedCard = HeroCardSelection(wTarget);
                            Combo(wTarget, selectedCard);
                            Orbwalker.ForcedTarget = wTarget;
                            break;
                        default:
                            Combo(wTarget, chooser);
                            Orbwalker.ForcedTarget = wTarget;
                            break;
                    }
                }

                // Cast Q if possible.
                if (useQ && qTarget != null)
                {
                    if (useQStun)
                    {
                        if (Q.IsInRange(qTarget) && Q.IsReady() && PlayerInstance.ManaPercent >= manaManagerQ && qTarget.HasBuff("Stun"))
                        {
                            var pred = Q.GetPrediction(qTarget);

                            if (pred.HitChance == HitChance.High)
                            {
                                Q.Cast(pred.CastPosition);
                            }
                        }                         
                    }
                    else
                    {
                        if (Q.IsInRange(qTarget) && Q.IsReady() && PlayerInstance.ManaPercent >= manaManagerQ)
                        {
                            var pred = Q.GetPrediction(qTarget);

                            if (pred.HitChance == HitChance.High)
                            {
                                Q.Cast(pred.CastPosition);
                            }
                        }
                    }
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                var qMinion = EntityManager.GetLaneMinions(EntityManager.UnitTeam.Enemy, PlayerInstance.Position.To2D(), Q.Range, false).OrderBy(t => t.Health).FirstOrDefault();//ObjectManager.Get<Obj_AI_Base>().Where(t => t.IsEnemy && t.IsMinion && Q.IsInRange(t)).OrderBy(t => t.Health).FirstOrDefault();
                var useQ = laneClearMenu["useQ"].Cast<CheckBox>().CurrentValue;
                var manaManagerQ = laneClearMenu["manaManagerQ"].Cast<Slider>().CurrentValue;

                // Cast Q if possible.
                if (useQ && qMinion != null)
                {
                    if (Q.IsReady() && PlayerInstance.ManaPercent >= manaManagerQ)
                    {
                        var minionPrediction = Prediction.Position.PredictUnitPosition(qMinion, Q.Speed);

                        if (minionPrediction != null && PlayerInstance.IsFacing(qMinion))
                        {
                            Q.Cast(minionPrediction.To3D());
                        }
                    }
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                var qMinion = EntityManager.GetJungleMonsters(PlayerInstance.Position.To2D(), Q.Range, false).OrderByDescending(t => t.Health).FirstOrDefault();//ObjectManager.Get<Obj_AI_Base>().Where(t => t.Team == GameObjectTeam.Neutral && Q.IsInRange(t)).OrderBy(t => t.MaxHealth).FirstOrDefault();
                var useQ = jungleClearMenu["useQ"].Cast<CheckBox>().CurrentValue;
                var manaManagerQ = jungleClearMenu["manaManagerQ"].Cast<Slider>().CurrentValue;

                // Cast Q if possible.
                if (useQ && qMinion != null)
                {
                    if (Q.IsReady() && PlayerInstance.ManaPercent >= manaManagerQ)
                    {
                        var minionPrediction = Prediction.Position.PredictUnitPosition(qMinion, Q.Speed);

                        if (minionPrediction != null && PlayerInstance.IsFacing(qMinion))
                        {
                            Q.Cast(minionPrediction.To3D());
                        }
                    }
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                var qTarget = TargetSelector.GetTarget(Q.Range, DamageType.Mixed);
                var useQ = harassMenu["useQ"].Cast<CheckBox>().CurrentValue;
                var manaManagerQ = harassMenu["manaManagerQ"].Cast<Slider>().CurrentValue;

                if (useQ)
                {
                    if (Q.IsInRange(qTarget) && Q.IsReady() && PlayerInstance.ManaPercent >= manaManagerQ)
                    {
                        var pred = Q.GetPrediction(qTarget);
                        Q.Cast(pred.CastPosition);
                    }
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None))
            {
                var useQ = killStealMenu["useQ"].Cast<CheckBox>().CurrentValue;
                var manaManagerQ = killStealMenu["manaManagerQ"].Cast<Slider>().CurrentValue;

                if (useQ)
                {
                    var t = TargetSelector.GetTarget(Q.Range, DamageType.Mixed);

                    if (t != null && Q.IsReady())
                    {
                        if (t.Health < PlayerInstance.GetSpellDamage(t, SpellSlot.Q) && PlayerInstance.ManaPercent >= manaManagerQ)
                        {
                            var pred = Q.GetPrediction(t);

                            if (pred != null && pred.HitChance == HitChance.High)
                            {
                                Q.Cast(pred.CastPosition);
                            }
                        }
                    }
                }
            }
        }
    }
}
