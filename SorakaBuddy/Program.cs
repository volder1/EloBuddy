namespace SorakaBuddy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Drawing;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using SharpDX;

    internal class Program
    {
        /// <summary>
        /// Soraka's Name
        /// </summary>
        public const string ChampionName = "Soraka";

        /// <summary>
        /// Starcall
        /// </summary>
        public static Spell.Skillshot Q;

        /// <summary>
        /// Astral Infusion
        /// </summary>
        public static Spell.Targeted W;

        /// <summary>
        /// Equinox
        /// </summary>
        public static Spell.Skillshot E;

        /// <summary>
        /// Wish
        /// </summary>
        public static Spell.Active R;

        /// <summary>
        /// Initializes the Menu
        /// </summary>
        public static Menu SorakaBuddy, ComboMenu, HarassMenu, HealMenu, InterruptMenu, GapcloserMenu, DrawingMenu, MiscMenu;

        /// <summary>
        /// Gets the Player
        /// </summary>
        public static AIHeroClient PlayerInstance
        {
            get { return Player.Instance; }
        }

        /// <summary>
        /// Runs when the Program Starts
        /// </summary>
        /// <param name="args">The run arguments</param>
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        /// <summary>
        /// Called when Loading is Completed
        /// </summary>
        /// <param name="args">The loading arguments</param>
        static void Loading_OnLoadingComplete(EventArgs args)
        {
            try
            {
                if (ChampionName != PlayerInstance.BaseSkinName)
                {
                    return;
                }

                Q = new Spell.Skillshot(SpellSlot.Q, 950, SkillShotType.Circular, 283, 1100, 210);
                W = new Spell.Targeted(SpellSlot.W, 550);
                E = new Spell.Skillshot(SpellSlot.E, 925, SkillShotType.Circular, 500, 1750, 70);
                R = new Spell.Active(SpellSlot.R, int.MaxValue);

                SorakaBuddy = MainMenu.AddMenu("SorakaBuddy", "SorakaBuddy");

                // Combo Menu
                ComboMenu = SorakaBuddy.AddSubMenu("Combo", "Combo");
                ComboMenu.AddGroupLabel("Combo Setting");
                ComboMenu.Add("useQ", new CheckBox("Use Q", true));
                ComboMenu.Add("useE", new CheckBox("Use E", true));
                ComboMenu.AddSeparator();
                ComboMenu.AddGroupLabel("ManaManager");
                ComboMenu.Add("manaQ", new Slider("Min Mana % before Q", 25, 0, 100));
                ComboMenu.Add("manaE", new Slider("Min Mana % before E", 25, 0, 100));

                // Lane Clear Menu
                /*LaneClearMenu = SorakaBuddy.AddSubMenu("Lane Clear", "LaneClear");
                LaneClearMenu.Add("useQ", new CheckBox("Use Q", true));*/

                // Harass Menu
                HarassMenu = SorakaBuddy.AddSubMenu("Harass", "Harass");
                HarassMenu.AddGroupLabel("Harass Setting");
                HarassMenu.Add("useQ", new CheckBox("Use Q", true));
                HarassMenu.Add("useE", new CheckBox("Use E", true));
                HarassMenu.AddSeparator();
                HarassMenu.AddGroupLabel("ManaManager");
                HarassMenu.Add("manaQ", new Slider("Min Mana % before Q", 25, 0, 100));
                HarassMenu.Add("manaE", new Slider("Min Mana % before E", 25, 0, 100));

                // Heal Menu
                var Allies = HeroManager.Allies.Where(a => !a.IsMe);
                HealMenu = SorakaBuddy.AddSubMenu("Auto Heal", "Heal");
                HealMenu.AddGroupLabel("Auto W Setting");
                HealMenu.Add("autoW", new CheckBox("Auto W Allies and Me", true));
                HealMenu.Add("autoWHP_self", new Slider("Own HP % before using W", 50, 0, 100));
                HealMenu.Add("autoWHP_other", new Slider("Ally HP % before W", 50, 0, 100));
                HealMenu.AddSeparator();
                HealMenu.AddGroupLabel("Auto R Setting");
                HealMenu.Add("useR", new CheckBox("Auto R on HP %", true));
                HealMenu.AddSeparator();
                HealMenu.Add("hpR", new Slider("HP % before using R", 25, 0, 100));
                HealMenu.AddSeparator();
                HealMenu.AddLabel("Which Champion to Heal? Using W?");
                if (Allies != null)
                {
                    foreach (var a in Allies)
                    {
                        HealMenu.Add("autoHeal_" + a.BaseSkinName, new CheckBox("Auto Heal with W " + a.BaseSkinName, true));
                    }
                }
                HealMenu.AddSeparator();
                HealMenu.AddLabel("Which Champion to Heal? Using R?");
                if (Allies != null)
                {
                    foreach (var a in Allies)
                    {
                        HealMenu.Add("autoHealR_" + a.BaseSkinName, new CheckBox("Auto Heal with R " + a.BaseSkinName, true));
                    }
                }
                HealMenu.Add("autoHealR_" + PlayerInstance.BaseSkinName, new CheckBox("Auto Heal Self with R", true));
                HealMenu.AddSeparator();
                HealMenu.AddGroupLabel("Heal Priority");
                var healPrioritySlider = HealMenu.Add("Slider", new Slider("mode", 0, 0, 2));
                var healPriorityArray = new[] { "MostAD", "MostAP", "LowestHealth" };
                healPrioritySlider.DisplayName = healPriorityArray[healPrioritySlider.CurrentValue];
                healPrioritySlider.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
                {
                    sender.DisplayName = healPriorityArray[changeArgs.NewValue];
                };

                // Interrupt Menu
                InterruptMenu = SorakaBuddy.AddSubMenu("Interrupter", "Interrupter");
                InterruptMenu.AddGroupLabel("Interrupter Setting");
                InterruptMenu.Add("useE", new CheckBox("Use E on Interrupt", true));

                // Gapcloser Menu
                GapcloserMenu = SorakaBuddy.AddSubMenu("Gapcloser", "Gapcloser");
                GapcloserMenu.AddGroupLabel("Gapcloser Setting");
                GapcloserMenu.Add("useQ", new CheckBox("Use Q on Gapcloser", true));
                GapcloserMenu.Add("useE", new CheckBox("Use E on Gapcloser", true));

                // Drawing Menu
                DrawingMenu = SorakaBuddy.AddSubMenu("Drawing", "Drawing");
                DrawingMenu.AddGroupLabel("Drawing Setting");
                DrawingMenu.Add("drawQ", new CheckBox("Draw Q Range", true));
                DrawingMenu.Add("drawW", new CheckBox("Draw W Range", true));
                DrawingMenu.Add("drawE", new CheckBox("Draw E Range", true));

                // Misc Menu
                MiscMenu = SorakaBuddy.AddSubMenu("Misc", "Misc");
                MiscMenu.AddGroupLabel("Miscellaneous Setting");
                MiscMenu.Add("disableMAA", new CheckBox("Disable Minion AA", true));
                MiscMenu.Add("disableCAA", new CheckBox("Disable Champion AA", true));

                Chat.Print("SorakaBuddy: Initialized", System.Drawing.Color.LightGreen);

                Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
                Game.OnTick += Game_OnTick;
                Drawing.OnDraw += Drawing_OnDraw;

                Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
                Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            }
            catch (Exception e)
            {
                Chat.Print("SorakaBuddy: Exception occured while Initializing Addon. Error: " + e.Message);
            }
        }

        /// <summary>
        /// Does Combo
        /// </summary>
        static void Combo()
        {
            if (ComboMenu["useQ"].Cast<CheckBox>().CurrentValue)
            {
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

                if (target != null)
                {
                    if (PlayerInstance.ManaPercent >= ComboMenu["manaQ"].Cast<Slider>().CurrentValue)
                    {
                        if (target.IsValidTarget(Q.Range) && Q.IsReady())
                        {
                            var pred = Prediction.Position.PredictCircularMissile(target, Q.Range, Q.Radius, Q.CastDelay, Q.Speed, PlayerInstance.Position); //Q.GetPrediction(target);

                            if (pred.HitChance == HitChance.High)
                            {
                                Q.Cast(pred.CastPosition);
                            }
                        }
                    }
                }
            }
            if (ComboMenu["useE"].Cast<CheckBox>().CurrentValue)
            {
                var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);

                if (target != null)
                {
                    if (PlayerInstance.ManaPercent >= ComboMenu["manaE"].Cast<Slider>().CurrentValue)
                    {
                        if (target.IsValidTarget(E.Range) && E.IsReady())
                        {
                            var pred = Prediction.Position.PredictCircularMissile(target, E.Range, E.Radius, E.CastDelay, E.Speed, PlayerInstance.Position); //E.GetPrediction(target);

                            if (pred.HitChance == HitChance.High)
                            {
                                E.Cast(pred.CastPosition);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Does Harass
        /// </summary>
        static void Harass()
        {
            if (HarassMenu["useQ"].Cast<CheckBox>().CurrentValue)
            {
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

                if (target != null)
                {
                    if (PlayerInstance.ManaPercent >= HarassMenu["manaQ"].Cast<Slider>().CurrentValue)
                    {
                        if (target.IsValidTarget(Q.Range) && Q.IsReady())
                        {
                            var pred = Prediction.Position.PredictCircularMissile(target, Q.Range, Q.Radius, Q.CastDelay, Q.Speed, PlayerInstance.Position); //Q.GetPrediction(target);

                            if (pred.HitChance == HitChance.High)
                            {
                                Q.Cast(pred.CastPosition);
                            }
                        }
                    }
                }
            }
            if (HarassMenu["useE"].Cast<CheckBox>().CurrentValue)
            {
                var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);

                if (target != null)
                {
                    if (PlayerInstance.ManaPercent >= HarassMenu["manaE"].Cast<Slider>().CurrentValue)
                    {
                        if (target.IsValidTarget(E.Range) && E.IsReady())
                        {
                            var pred = Prediction.Position.PredictCircularMissile(target, E.Range, E.Radius, E.CastDelay, E.Speed, PlayerInstance.Position); //E.GetPrediction(target);

                            if (pred.HitChance == HitChance.High)
                            {
                                E.Cast(pred.CastPosition);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Does Auto Heal
        /// </summary>
        static void AutoHeal()
        {
            var healPrioritySlider = HealMenu["Slider"].Cast<Slider>().DisplayName;
            var autoWHP_other = HealMenu["autoWHP_other"].Cast<Slider>().CurrentValue;
            var autoWHP_self = HealMenu["autoWHP_self"].Cast<Slider>().CurrentValue;
            var Recall = PlayerInstance.HasBuff("Recall");

            switch (healPrioritySlider)
            {
                case "MostAD":
                    if (HealMenu["autoW"].Cast<CheckBox>().CurrentValue)
                    {
                        var MostADAlly = HeroManager.Allies.Where(a => W.IsInRange(a) && !a.IsMe).OrderBy(a => a.TotalAttackDamage).OrderBy(a => a.Health).FirstOrDefault();

                        if (MostADAlly != null)
                        {
                            if (MostADAlly.HealthPercent <= autoWHP_other
                                && PlayerInstance.HealthPercent >= autoWHP_self)
                            {
                                if (HealMenu["autoHeal_" + MostADAlly.BaseSkinName].Cast<CheckBox>().CurrentValue && !Recall)
                                {
                                    W.Cast(MostADAlly);
                                }
                            }
                        }
                    }
                    if (HealMenu["useR"].Cast<CheckBox>().CurrentValue)
                    {
                        var MostADAllyOOR = HeroManager.Allies.OrderByDescending(a => a.TotalAttackDamage).OrderBy(a => a.Health).FirstOrDefault();

                        if (MostADAllyOOR != null)
                        {
                            if (MostADAllyOOR.HealthPercent <= HealMenu["hpR"].Cast<Slider>().CurrentValue)
                            {
                                if (HealMenu["autoHealR_" + MostADAllyOOR.BaseSkinName].Cast<CheckBox>().CurrentValue && !Recall)
                                {
                                    R.Cast();
                                }
                            }
                        }
                    }
                    break;
                case "MostAP":
                    if (HealMenu["autoW"].Cast<CheckBox>().CurrentValue)
                    {
                        var MostAPAlly = HeroManager.Allies.Where(a => W.IsInRange(a) && !a.IsMe).OrderBy(a => a.TotalMagicalDamage).OrderBy(a => a.Health).FirstOrDefault();

                        if (MostAPAlly != null)
                        {
                            if (MostAPAlly.HealthPercent <= autoWHP_other
                                && PlayerInstance.HealthPercent >= autoWHP_self)
                            {
                                if (HealMenu["autoHeal_" + MostAPAlly.BaseSkinName].Cast<CheckBox>().CurrentValue && !Recall)
                                {
                                    W.Cast(MostAPAlly);
                                }
                            }
                        }
                    }
                    if (HealMenu["useR"].Cast<CheckBox>().CurrentValue)
                    {
                        var MostAPAllyOOR = HeroManager.Allies.OrderByDescending(a => a.TotalMagicalDamage).OrderBy(a => a.Health).FirstOrDefault();

                        if (MostAPAllyOOR != null)
                        {
                            if (MostAPAllyOOR.HealthPercent <= HealMenu["hpR"].Cast<Slider>().CurrentValue)
                            {
                                if (HealMenu["autoHealR_" + MostAPAllyOOR.BaseSkinName].Cast<CheckBox>().CurrentValue && !Recall)
                                {
                                    R.Cast();
                                }
                            }
                        }
                    }
                    break;
                case "LowestHealth":
                    if (HealMenu["autoW"].Cast<CheckBox>().CurrentValue)
                    {
                        var LowestHealthAlly = HeroManager.Allies.Where(a => W.IsInRange(a) && !a.IsMe).OrderBy(a => a.Health).FirstOrDefault();

                        if (LowestHealthAlly != null)
                        {
                            if (LowestHealthAlly.HealthPercent <= autoWHP_other
                                && PlayerInstance.HealthPercent >= autoWHP_self)
                            {
                                if (HealMenu["autoHeal_" + LowestHealthAlly.BaseSkinName].Cast<CheckBox>().CurrentValue && !Recall)
                                {
                                    W.Cast(LowestHealthAlly);
                                }
                            }
                        }
                    }
                    if (HealMenu["useR"].Cast<CheckBox>().CurrentValue)
                    {
                        var LowestHealthAllyOOR = HeroManager.Allies.OrderByDescending(a => a.Health).FirstOrDefault();

                        if (LowestHealthAllyOOR != null)
                        {
                            if (LowestHealthAllyOOR.HealthPercent <= HealMenu["hpR"].Cast<Slider>().CurrentValue)
                            {
                                if (HealMenu["autoHealR_" + LowestHealthAllyOOR.BaseSkinName].Cast<CheckBox>().CurrentValue && !Recall)
                                {
                                    R.Cast();
                                }
                            }
                        }
                    }
                    break;
                default:
                    return;
            }
        }

        /// <summary>
        /// Called when Sender is Interruptable
        /// </summary>
        /// <param name="sender">The Attacker</param>
        /// <param name="e">The Spell Information</param>
        static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (InterruptMenu["useE"].Cast<CheckBox>().CurrentValue || e.DangerLevel == DangerLevel.High)
            {
                if (sender.IsValidTarget(E.Range))
                {
                    if (E.IsReady())
                    {
                        var pred = E.GetPrediction(sender);

                        if (pred.HitChance == HitChance.High)
                        {
                            E.Cast(pred.CastPosition);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when Sender can be Gapclosed
        /// </summary>
        /// <param name="sender">The Victim</param>
        /// <param name="e">The Spell Information</param>
        static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (GapcloserMenu["useQ"].Cast<CheckBox>().CurrentValue)
            {
                if (sender.IsValidTarget(Q.Range) && Q.IsReady())
                {
                    var pred = Q.GetPrediction(sender);

                    if (pred.HitChance == HitChance.High)
                    {
                        Q.Cast(pred.CastPosition);
                    }
                }
            }

            if (GapcloserMenu["useE"].Cast<CheckBox>().CurrentValue)
            {
                if (sender.IsValidTarget(E.Range) && E.IsReady())
                {
                    var pred = E.GetPrediction(sender);

                    if (pred.HitChance == HitChance.High)
                    {
                        E.Cast(pred.CastPosition);
                    }
                }
            }
        }

        /// <summary>
        /// Before Attack
        /// </summary>
        /// <param name="target">The Target that will be attacked</param>
        /// <param name="args">The Args</param>
        static void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            var t = target as AIHeroClient;
            var m = target as Obj_AI_Base;

            if (t != null)
            {
                var alliesNearPlayer = HeroManager.Allies.Where(a => PlayerInstance.Distance(a) <= 1200).Count();
                
                if (alliesNearPlayer > 1)
                {
                    if (MiscMenu["disableCAA"].Cast<CheckBox>().CurrentValue)
                    {
                        args.Process = false;
                    }
                }

            }
            else if (m != null)
            {
                var alliesNearPlayer = HeroManager.Allies.Where(a => PlayerInstance.Distance(a) <= 1200).Count();
                if (alliesNearPlayer > 1)
                {
                    if (MiscMenu["disableMAA"].Cast<CheckBox>().CurrentValue)
                    {
                        args.Process = false;
                    }
                }
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// Called when the Game Updates
        /// </summary>
        /// <param name="args">The Args</param>
        static void Game_OnTick(EventArgs args)
        {
            if (HealMenu["autoW"].Cast<CheckBox>().CurrentValue
                || HealMenu["autoR"].Cast<CheckBox>().CurrentValue)
            {
                AutoHeal();
            }
            
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
        }
        
        /// <summary>
        /// Called when the Game Draws
        /// </summary>
        /// <param name="args">The Args</param>
        static void Drawing_OnDraw(EventArgs args)
        {
            var PlayerPosition = PlayerInstance.Position;

            if (DrawingMenu["drawQ"].Cast<CheckBox>().CurrentValue)
            {
                EloBuddy.SDK.Rendering.Circle.Draw(Q.IsReady() ? SharpDX.Color.Green : SharpDX.Color.Red, Q.Range, PlayerPosition);
            }
            if (DrawingMenu["drawW"].Cast<CheckBox>().CurrentValue)
            {
                EloBuddy.SDK.Rendering.Circle.Draw(W.IsReady() ? SharpDX.Color.Green : SharpDX.Color.Red, W.Range, PlayerPosition);
            }
            if (DrawingMenu["drawE"].Cast<CheckBox>().CurrentValue)
            {
                EloBuddy.SDK.Rendering.Circle.Draw(E.IsReady() ? SharpDX.Color.Green : SharpDX.Color.Red, E.Range, PlayerPosition);
            }
        }
    }
}
