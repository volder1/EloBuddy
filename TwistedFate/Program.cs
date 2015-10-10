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

    internal class Program
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
        public static Menu MainMenu, CardSelectorMenu, ComboMenu, LaneClearMenu, JungleClearMenu, HarassMenu, KillStealMenu, DrawingMenu, MiscMenu;

        /// <summary>
        /// Gets the player.
        /// </summary>
        private static AIHeroClient PlayerInstance
        {
            get { return Player.Instance; }
        }

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
            if (PlayerInstance.BaseSkinName != ChampionName)
            {
                return;
            }

            Bootstrap.Init(null);

            Q = new Spell.Skillshot(SpellSlot.Q, 1450, SkillShotType.Linear, 250, 1000, 40);
            R = new Spell.Active(SpellSlot.R, 5500);

            // Menu
            MainMenu = EloBuddy.SDK.Menu.MainMenu.AddMenu("Twisted Fate", "TwistedFate");

            // Card Selector Menu
            CardSelectorMenu = MainMenu.AddSubMenu("Card Selector Menu", "csMenu");
            CardSelectorMenu.AddGroupLabel("Card Selector Settings");
            CardSelectorMenu.Add("useY", new KeyBind("Use Yellow Card", false, KeyBind.BindTypes.HoldActive, "W".ToCharArray()[0]));
            CardSelectorMenu.Add("useB", new KeyBind("Use Blue Card", false, KeyBind.BindTypes.HoldActive, "E".ToCharArray()[0]));
            CardSelectorMenu.Add("useR", new KeyBind("Use Red Card", false, KeyBind.BindTypes.HoldActive, "T".ToCharArray()[0]));

            // Combo
            ComboMenu = MainMenu.AddSubMenu("Combo Menu", "comboMenu");
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.Add("useQ", new CheckBox("Use Q in Combo"));
            ComboMenu.Add("useCard", new CheckBox("Use W in Combo"));
            ComboMenu.Add("useQStun", new CheckBox("Use Q only if Stunned"));
            ComboMenu.Add("manaManagerQ", new Slider("How much mana before using Q", 25));
            ComboMenu.AddSeparator();

            var comboCardChooserSlider = ComboMenu.Add("chooser", new Slider("mode", 0, 0, 3));
            var comboCardArray = new[] { "Smart", "Blue", "Red", "Yellow" };
            comboCardChooserSlider.DisplayName = comboCardArray[comboCardChooserSlider.CurrentValue];
            comboCardChooserSlider.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
            {
                sender.DisplayName = comboCardArray[changeArgs.NewValue];
            };

            // Harass Menu
            HarassMenu = MainMenu.AddSubMenu("Harass Menu", "harassMenu");
            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.Add("useQ", new CheckBox("Use Q in Harass"));
            HarassMenu.Add("useCard", new CheckBox("Use W in Harass"));
            HarassMenu.Add("manaManagerQ", new Slider("How much mana before using Q", 25));
            HarassMenu.AddSeparator();

            var harassCardChooserSlider = HarassMenu.Add("chooser", new Slider("mode", 0, 0, 3));
            var harassCardArray = new[] { "Smart", "Blue", "Red", "Yellow" };
            harassCardChooserSlider.DisplayName = harassCardArray[harassCardChooserSlider.CurrentValue];
            harassCardChooserSlider.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
            {
                sender.DisplayName = harassCardArray[changeArgs.NewValue];
            };

            // Lane Clear Menu
            LaneClearMenu = MainMenu.AddSubMenu("Lane Clear", "laneclearMenu");
            LaneClearMenu.AddGroupLabel("LaneClear Settings");
            LaneClearMenu.Add("useQ", new CheckBox("Use Q in LaneClear", false));
            LaneClearMenu.Add("useCard", new CheckBox("Use W in LaneClear"));
            LaneClearMenu.Add("manaManagerQ", new Slider("How much mana before using Q", 50));
            LaneClearMenu.AddSeparator();

            var laneclearCardChooserSlider = LaneClearMenu.Add("chooser", new Slider("mode", 0, 0, 3));
            var laneclearCardArray = new[] { "Smart", "Blue", "Red", "Yellow" };
            laneclearCardChooserSlider.DisplayName = laneclearCardArray[laneclearCardChooserSlider.CurrentValue];
            laneclearCardChooserSlider.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
            {
                sender.DisplayName = laneclearCardArray[changeArgs.NewValue];
            };
            
            // Jungle Clear Menu
            JungleClearMenu = MainMenu.AddSubMenu("Jungle Clear Menu", "jgMenu");
            JungleClearMenu.AddGroupLabel("JungleClear Settings");
            JungleClearMenu.Add("useQ", new CheckBox("Use Q in JungleClear", false));
            JungleClearMenu.Add("useCard", new CheckBox("Use W in JungleClear"));
            JungleClearMenu.Add("manaManagerQ", new Slider("How much mana before using Q", 50));
            JungleClearMenu.AddSeparator();

            var jungleclearCardChooserSlider = JungleClearMenu.Add("chooser", new Slider("mode", 0, 0, 3));
            var jungleclearCardArray = new[] { "Smart", "Blue", "Red", "Yellow" };
            jungleclearCardChooserSlider.DisplayName = jungleclearCardArray[jungleclearCardChooserSlider.CurrentValue];
            jungleclearCardChooserSlider.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
            {
                sender.DisplayName = jungleclearCardArray[changeArgs.NewValue];
            };

            // Kill Steal Menu
            KillStealMenu = MainMenu.AddSubMenu("Kill Steal Menu", "ksMenu");
            KillStealMenu.AddGroupLabel("KillSteal Settings");
            KillStealMenu.Add("useQ", new CheckBox("Use Q to KS"));
            KillStealMenu.Add("manaManagerQ", new Slider("How much mana before using Q", 15));
            KillStealMenu.AddSeparator();

            // Drawing Menu
            DrawingMenu = MainMenu.AddSubMenu("Drawing Menu", "drawMenu");
            DrawingMenu.AddGroupLabel("Drawing Settings");
            DrawingMenu.Add("drawQ", new CheckBox("Draw Q Range"));
            DrawingMenu.Add("drawR", new CheckBox("Draw R Range"));
            DrawingMenu.AddSeparator();

            MiscMenu = MainMenu.AddSubMenu("Misc Menu", "miscMenu");
            MiscMenu.AddGroupLabel("Misc Settings");
            MiscMenu.Add("autoQ", new CheckBox("Automatically Q's a CCed Target"));
            MiscMenu.Add("autoY", new CheckBox("Automatically select Yellow Card when R"));
            MiscMenu.Add("manaW", new Slider("How much mana before selecting Blue Card", 25));
            MiscMenu.Add("delay", new CheckBox("Delay Card Choosing", false));

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
        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
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
        private static void Orbwalker_OnPreAttack(AttackableUnit target, EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                var minion = target as Obj_AI_Minion;
                var useCard = LaneClearMenu["useCard"].Cast<CheckBox>().CurrentValue;
                var chooser = LaneClearMenu["chooser"].Cast<Slider>().DisplayName;

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
                var minion = target as Obj_AI_Minion;
                var useCard = JungleClearMenu["useCard"].Cast<CheckBox>().CurrentValue;
                var chooser = JungleClearMenu["chooser"].Cast<Slider>().DisplayName;

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
                var m = target as Obj_AI_Minion;
                var useCard = HarassMenu["useCard"].Cast<CheckBox>().CurrentValue;
                var chooser = ComboMenu["chooser"].Cast<Slider>().DisplayName;

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
        private static void AutoQ()
        {
            var heroes = EntityManager.Heroes.Enemies.Where(t => t.HasBuff("Stun"));

            foreach (var t in heroes)
            {
                if (Q.IsReady())
                {
                    Q.Cast(t.ServerPosition);
                }
            }
        }

        /// <summary>
        /// Does Combo
        /// </summary>
        /// <param name="t">The Target</param>
        /// <param name="selectedCard">The Card that is selected.</param>
        private static void Combo(AIHeroClient t, string selectedCard)
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
        private static void Harass(Obj_AI_Base t, string selectedCard)
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
        private static void Harass(AIHeroClient t, string selectedCard)
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
        private static void LaneClear(Obj_AI_Base t, string selectedCard)
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
        private static void JungleClear(Obj_AI_Base t, string selectedCard)
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
        private static string MinionCardSelection(Obj_AI_Base t)
        {
            string card;
            var minionsaroundTarget = ObjectManager.Get<Obj_AI_Base>().Count(target => target.IsMinion && target.Distance(t) <= 200);
            var manaW = MiscMenu["manaW"].Cast<Slider>().CurrentValue;

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
        private static string HeroCardSelection(Obj_AI_Base t)
        {
            string card;
            var alliesaroundTarget = EntityManager.Heroes.Enemies.Count(target => target.Distance(t) <= 200);
            var manaW = MiscMenu["manaW"].Cast<Slider>().CurrentValue;

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
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (DrawingMenu["drawQ"].Cast<CheckBox>().CurrentValue)
            {
                if (PlayerInstance != null)
                {
                    Circle.Draw(Q.IsReady() ? SharpDX.Color.Green : SharpDX.Color.Red, Q.Range, PlayerInstance.Position);
                }
            }

            if (DrawingMenu["drawR"].Cast<CheckBox>().CurrentValue)
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
        private static void Game_OnTick(EventArgs args)
        {          
            var useY = CardSelectorMenu["useY"].Cast<KeyBind>();
            var useB = CardSelectorMenu["useB"].Cast<KeyBind>();
            var useR = CardSelectorMenu["useR"].Cast<KeyBind>();

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

            if (MiscMenu["autoQ"].Cast<CheckBox>().CurrentValue)
            {
                AutoQ();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                var qTarget = TargetSelector.GetTarget(Q.Range, DamageType.Mixed);
                var useQ = ComboMenu["useQ"].Cast<CheckBox>().CurrentValue;
                var useQStun = ComboMenu["useQStun"].Cast<CheckBox>().CurrentValue;
                var manaManagerQ = ComboMenu["manaManagerQ"].Cast<Slider>().CurrentValue;

                var wTarget = TargetSelector.GetTarget(
                    PlayerInstance.AttackRange + 150,
                    DamageType.Mixed,
                    PlayerInstance.ServerPosition);
                var useCard = ComboMenu["useCard"].Cast<CheckBox>().CurrentValue;
                var chooser = ComboMenu["chooser"].Cast<Slider>().DisplayName;

                if (useCard && wTarget != null)
                {
                    switch (chooser)
                    {
                        case "Smart":
                            var selectedCard = HeroCardSelection(wTarget);
                            Combo(wTarget, selectedCard);
                            //Orbwalker.ForcedTarget = wTarget;
                            break;
                        default:
                            Combo(wTarget, chooser);
                            //Orbwalker.ForcedTarget = wTarget;
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
                var qMinion = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, PlayerInstance.ServerPosition, Q.Range, false).OrderBy(t => t.Health).FirstOrDefault();//ObjectManager.Get<Obj_AI_Base>().Where(t => t.IsEnemy && t.IsMinion && Q.IsInRange(t)).OrderBy(t => t.Health).FirstOrDefault();
                var useQ = LaneClearMenu["useQ"].Cast<CheckBox>().CurrentValue;
                var manaManagerQ = LaneClearMenu["manaManagerQ"].Cast<Slider>().CurrentValue;

                // Cast Q if possible.
                if (useQ && qMinion != null)
                {
                    if (Q.IsReady() && PlayerInstance.ManaPercent >= manaManagerQ)
                    {
                        var minionPrediction = Prediction.Position.PredictLinearMissile(qMinion, Q.Range, Q.Width, Q.CastDelay, Q.Speed, int.MaxValue, PlayerInstance.ServerPosition);//Prediction.Position.PredictUnitPosition(qMinion, Q.Speed);

                        if (minionPrediction != null)
                        {
                            if (minionPrediction.HitChance == HitChance.High)
                            {
                                 Q.Cast(minionPrediction.CastPosition);
                            }
                        }
                    }
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                var qMinion = EntityManager.MinionsAndMonsters.GetJungleMonsters(PlayerInstance.ServerPosition, Q.Range, false).OrderByDescending(t => t.Health).FirstOrDefault();//ObjectManager.Get<Obj_AI_Base>().Where(t => t.Team == GameObjectTeam.Neutral && Q.IsInRange(t)).OrderBy(t => t.MaxHealth).FirstOrDefault();
                var useQ = JungleClearMenu["useQ"].Cast<CheckBox>().CurrentValue;
                var manaManagerQ = JungleClearMenu["manaManagerQ"].Cast<Slider>().CurrentValue;

                // Cast Q if possible.
                if (useQ && qMinion != null)
                {
                    if (Q.IsReady() && PlayerInstance.ManaPercent >= manaManagerQ)
                    {
                        var minionPrediction = Prediction.Position.PredictLinearMissile(qMinion, Q.Range, Q.Width, Q.CastDelay, Q.Speed, int.MaxValue, PlayerInstance.ServerPosition); //Prediction.Position.PredictUnitPosition(qMinion, Q.Speed);

                        if (minionPrediction != null)
                        {
                            if (minionPrediction.HitChance == HitChance.High)
                            {
                                Q.Cast(minionPrediction.CastPosition);
                            }
                        }
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
                    if (qTarget != null)
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

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None))
            {
                var useQ = KillStealMenu["useQ"].Cast<CheckBox>().CurrentValue;
                var manaManagerQ = KillStealMenu["manaManagerQ"].Cast<Slider>().CurrentValue;

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
