namespace Lux
{
    using System;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;

    using SharpDX;

    class Program
    {
        /// <summary>
        /// Lux's Name
        /// </summary>
        private const string ChampionName = "Lux";

        /// <summary>
        /// Spell Q
        /// </summary>
        public static Spell.Skillshot Q;

        /// <summary>
        /// Spell W
        /// </summary>
        public static Spell.Skillshot W;

        /// <summary>
        /// Spell E
        /// </summary>
        public static Spell.Skillshot E;

        /// <summary>
        /// Spell E2
        /// </summary>
        public static Spell.Active E2;

        /// <summary>
        /// Spell R
        /// </summary>
        public static Spell.Skillshot R;

        /// <summary>
        /// Lux E Object
        /// </summary>
        public static GameObject LuxEObject;

        /// <summary>
        /// Initializes the Menu
        /// </summary>
        public static Menu LuxMenu, ComboMenu, HarassMenu, LaneClearMenu, KillStealMenu, MiscMenu, DrawingMenu;

        /// <summary>
        /// Gets Lux Passive Damage
        /// </summary>
        /// <param name="target">The Target</param>
        /// <returns>The damage that the passive will do.</returns>
        private static float LuxPassiveDamage(Obj_AI_Base target)
        {
            if (target.HasBuff("luxilluminatingfraulein"))
            {
                return target.CalculateDamageOnUnit(target, DamageType.Magical, (float)(10 + (8 * Player.Instance.Level) * (Player.Instance.FlatMagicDamageMod * 0.2)));
            }
            return 0;
        }

        /// <summary>
        /// Gets if the Spell should be casted by calculating current mana to ManaManager Limit
        /// </summary>
        /// <param name="spellSlot">The Spell Being calculated</param>
        /// <returns>If the spell should be casted.</returns>
        private static bool ManaManager(SpellSlot spellSlot)
        {
            if (MiscMenu["disableC"].Cast<CheckBox>().CurrentValue
                && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                return true;
            }

            var playerManaPercent = (Player.Instance.Mana / Player.Instance.MaxMana) * 100;

            if (spellSlot == SpellSlot.Q)
            {
                return MiscMenu["manaQ"].Cast<Slider>().CurrentValue <= playerManaPercent;
            }

            if (spellSlot == SpellSlot.W)
            {
                return MiscMenu["manaW"].Cast<Slider>().CurrentValue <= playerManaPercent;
            }

            if (spellSlot == SpellSlot.E)
            {
                return MiscMenu["manaE"].Cast<Slider>().CurrentValue <= playerManaPercent;
            }

            if (spellSlot == SpellSlot.R)
            {
                return MiscMenu["manaR"].Cast<Slider>().CurrentValue <= playerManaPercent;
            }

            return false;
        }

        /// <summary>
        /// Called when program starts.
        /// </summary>
        private static void Main()
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        /// <summary>
        /// Called when the game starts.
        /// </summary>
        /// <param name="args">The Args</param>
        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (ChampionName != Player.Instance.BaseSkinName)
            {
                return;
            }

            Q = new Spell.Skillshot(SpellSlot.Q, 1175, SkillShotType.Linear, 250, 85, 1150);
            W = new Spell.Skillshot(SpellSlot.W, 1075, SkillShotType.Linear, 250, 110, 1200);
            E = new Spell.Skillshot(SpellSlot.E, 1200, SkillShotType.Circular, 250, 280, 950);
            E2 = new Spell.Active(SpellSlot.E);
            R = new Spell.Skillshot(SpellSlot.R, 3300, SkillShotType.Linear, 1000, 190, int.MaxValue);

            LuxMenu = MainMenu.AddMenu("Lux", "Lux");
            LuxMenu.AddGroupLabel("This addon is made by KarmaPanda and should not be redistributed in any way.");
            LuxMenu.AddGroupLabel("Any unauthorized redistribution without credits will result in severe consequences.");
            LuxMenu.AddGroupLabel("Thank you for using this addon and have a fun time!");

            ComboMenu = LuxMenu.AddSubMenu("Combo", "Combo");
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.Add("useQ", new CheckBox("Combo using Q"));
            ComboMenu.Add("useE", new CheckBox("Combo using E"));
            ComboMenu.Add("useR", new CheckBox("Combo using R"));
            ComboMenu.Add("sliderR", new Slider("Amount of Enemies before casting R", 3, 1, 5));
            ComboMenu.AddLabel("Prediction Settings");
            ComboMenu.Add("qSlider", new Slider("Cast Q if % HitChance", 75));
            ComboMenu.Add("eSlider", new Slider("Cast E if % HitChance", 75));
            ComboMenu.Add("rSlider", new Slider("Cast R if % HitChance", 75));

            HarassMenu = LuxMenu.AddSubMenu("Harass", "Harass");
            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.Add("useQ", new CheckBox("Harass using Q"));
            HarassMenu.Add("useE", new CheckBox("Harass using E"));
            HarassMenu.AddLabel("Prediction Settings");
            HarassMenu.Add("qSlider", new Slider("Cast Q if % HitChance", 75));
            HarassMenu.Add("eSlider", new Slider("Cast E if % HitChance", 75));
            HarassMenu.Add("rSlider", new Slider("Cast R if % HitChance", 75));

            LaneClearMenu = LuxMenu.AddSubMenu("LaneClear", "LaneClear");
            LaneClearMenu.AddGroupLabel("Lane Clear Settings");
            LaneClearMenu.Add("useQ", new CheckBox("Lane Clear using Q", false));
            LaneClearMenu.Add("useE", new CheckBox("Lane Clear using E"));
            LaneClearMenu.Add("useR", new CheckBox("Mentally Retarded Mode (Use R)", false));
            LaneClearMenu.Add("useQs", new Slider("Minions before Q", 1, 1, 2));
            LaneClearMenu.Add("useEs", new Slider("Minions before E", 3, 1, 6));
            LaneClearMenu.Add("useRs", new Slider("Minions before R", 4, 1, 10));

            KillStealMenu = LuxMenu.AddSubMenu("KillSteal", "KillSteal");
            KillStealMenu.AddGroupLabel("Kill Steal Settings");
            KillStealMenu.Add("useQ", new CheckBox("Kill Steal using Q"));
            KillStealMenu.Add("useE", new CheckBox("Kill Steal using E"));
            KillStealMenu.Add("useR", new CheckBox("Kill Steal using R"));
            KillStealMenu.AddLabel("Prediction Settings");
            KillStealMenu.Add("qSlider", new Slider("Cast Q if % HitChance", 75));
            KillStealMenu.Add("eSlider", new Slider("Cast E if % HitChance", 75));
            KillStealMenu.Add("rSlider", new Slider("Cast R if % HitChance", 75));


            MiscMenu = LuxMenu.AddSubMenu("Misc", "Misc");
            MiscMenu.AddGroupLabel("Mana Manager Settings");
            MiscMenu.Add("manaQ", new Slider("Mana Manager Q", 25));
            MiscMenu.Add("manaW", new Slider("Mana Manager W", 25));
            MiscMenu.Add("manaE", new Slider("Mana Manager E", 25));
            MiscMenu.Add("manaR", new Slider("Mana Manager R", 25));
            MiscMenu.Add("disableC", new CheckBox("Disable Mana Manager in Combo"));
            MiscMenu.AddSeparator();
            MiscMenu.AddGroupLabel("Misc Settings");
            MiscMenu.Add("useW", new CheckBox("Automatically Cast W"));
            MiscMenu.Add("useM", new CheckBox("Use W only on Modes"));
            MiscMenu.Add("hpW", new Slider("HP % before W", 50));
            MiscMenu.AddLabel("Who to use W on?");
            var allies = EntityManager.Heroes.Allies.Where(a => !a.IsMe).OrderBy(a => a.BaseSkinName);
            foreach (var a in allies)
            {
                MiscMenu.Add("autoW_" + a.BaseSkinName, new CheckBox("Auto W " + a.BaseSkinName));
            }
            MiscMenu.Add("autoW_" + Player.Instance.BaseSkinName, new CheckBox("Auto W Self"));

            DrawingMenu = LuxMenu.AddSubMenu("Drawing", "Drawing");
            DrawingMenu.AddGroupLabel("Drawing Settings");
            DrawingMenu.Add("drawQ", new CheckBox("Draw Q Range"));
            DrawingMenu.Add("drawW", new CheckBox("Draw W Range"));
            DrawingMenu.Add("drawE", new CheckBox("Draw E Range"));
            DrawingMenu.Add("drawR", new CheckBox("Draw R Range"));

            Chat.Print("StarBuddy - Lux by KarmaPanda", System.Drawing.Color.DeepPink);

            Game.OnUpdate += Game_OnUpdate;
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
            Drawing.OnDraw += Drawing_OnDraw;
        }
        
        /// <summary>
        /// Called when a object gets created.
        /// </summary>
        /// <param name="sender">The Object</param>
        /// <param name="args">The Args</param>
        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Lux_Base_E_mis.troy")
            {
                LuxEObject = sender;
            }
        }

        /// <summary>
        /// Called when a object gets deleted.
        /// </summary>
        /// <param name="sender">The Object</param>
        /// <param name="args">The Args</param>
        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Lux_Base_E_tar_nova.troy")
            {
                LuxEObject = null;
            }
        }

        /// <summary>
        /// Called whenever the Game Draws
        /// </summary>
        /// <param name="args">The Args</param>
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (DrawingMenu["drawQ"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(!Q.IsReady() ? Color.Red : Color.LightGreen, Q.Range, Player.Instance.Position);
            }
            if (DrawingMenu["drawW"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(!W.IsReady() ? Color.Red : Color.LightGreen, W.Range, Player.Instance.Position);
            }
            if (DrawingMenu["drawE"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(!E.IsReady() ? Color.Red : Color.LightGreen, E.Range, Player.Instance.Position);
            }
            if (DrawingMenu["drawR"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(!R.IsReady() ? Color.Red : Color.LightGreen, R.Range, Player.Instance.Position);
            }
        }

        /// <summary>
        /// Called whenever the Game Ticks
        /// </summary>
        /// <param name="args">The Args</param>
        private static void Game_OnUpdate(EventArgs args)
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) && LuxEObject == null)
            {
                Q = new Spell.Skillshot(SpellSlot.Q, 1175, SkillShotType.Linear);
                E = new Spell.Skillshot(SpellSlot.E, 1200, SkillShotType.Circular);
                R = new Spell.Skillshot(SpellSlot.R, 3300, SkillShotType.Linear);
            }

            if (KillStealMenu["useQ"].Cast<CheckBox>().CurrentValue
                || KillStealMenu["useE"].Cast<CheckBox>().CurrentValue
                || KillStealMenu["useR"].Cast<CheckBox>().CurrentValue)
            {
                KillSteal();
            }

            if (MiscMenu["useW"].Cast<CheckBox>().CurrentValue)
            {
                if (MiscMenu["useM"].Cast<CheckBox>().CurrentValue)
                {
                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)
                        || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)
                        || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear)
                        || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
                    {
                        AutoW();
                    }
                }
                else if (!MiscMenu["useM"].Cast<CheckBox>().CurrentValue)
                {
                    AutoW();
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                LaneClear();
            }
        }

        /// <summary>
        /// Does Combo
        /// </summary>
        private static void Combo()
        {
            var useQ = ComboMenu["useQ"].Cast<CheckBox>().CurrentValue;
            var useE = ComboMenu["useE"].Cast<CheckBox>().CurrentValue;
            var useR = ComboMenu["useR"].Cast<CheckBox>().CurrentValue;
            var sliderR = ComboMenu["sliderR"].Cast<Slider>().CurrentValue;

            if (useQ && Q.IsReady() && ManaManager(SpellSlot.Q))
            {
                var t = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

                if (t != null)
                {
                    var pred = Q.GetPrediction(t);

                    if (pred != null && t.IsValidTarget() && pred.HitChancePercent >= ComboMenu["qSlider"].Cast<Slider>().CurrentValue)
                    {
                        Q.Cast(pred.CastPosition);
                    }
                }
            }

            if (useE && E.IsReady() && ManaManager(SpellSlot.E) && LuxEObject == null)
            {
                var t = TargetSelector.GetTarget(E.Range, DamageType.Magical);

                if (t != null)
                {
                    var pred = E.GetPrediction(t);

                    if (pred != null && t.IsValidTarget() && pred.HitChancePercent >= ComboMenu["eSlider"].Cast<Slider>().CurrentValue)
                    {
                        E.Cast(pred.CastPosition);
                    }
                }
            }

            if (LuxEObject != null && E.IsReady() && useE)
            {
                var target =
                    EntityManager.Heroes.Enemies.Where(
                        t => t.IsValidTarget() && t.Distance(LuxEObject.Position) <= LuxEObject.BoundingRadius);

                if (target.Any() && LuxEObject != null)
                {
                    E2.Cast();
                }
            }
            
            if (!useR || !R.IsReady() || !ManaManager(SpellSlot.R))
            {
                return;
            }
            var rTarget = TargetSelector.GetTarget(R.Range, DamageType.Magical);

            if (rTarget == null)
            {
                return;
            }

            var rPrediction = R.GetPrediction(rTarget);

            if (rPrediction != null && rTarget.IsValidTarget() && rPrediction.HitChancePercent >= ComboMenu["rSlider"].Cast<Slider>().CurrentValue
                && rPrediction.GetCollisionObjects<AIHeroClient>().Count() >= sliderR)
            {
                R.Cast(rPrediction.CastPosition);
            }
        }

        /// <summary>
        /// Does Harass
        /// </summary>
        private static void Harass()
        {
            var useQ = HarassMenu["useQ"].Cast<CheckBox>().CurrentValue;
            var useE = HarassMenu["useE"].Cast<CheckBox>().CurrentValue;

            if (useQ && Q.IsReady() && ManaManager(SpellSlot.Q))
            {
                var t = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

                if (t != null)
                {
                    var pred = Q.GetPrediction(t);

                    if (pred != null && t.IsValidTarget() && pred.HitChancePercent >= HarassMenu["qSlider"].Cast<Slider>().CurrentValue)
                    {
                        Q.Cast(pred.CastPosition);
                    }
                }
            }

            if (useE && E.IsReady() && ManaManager(SpellSlot.E) && LuxEObject == null)
            {
                var t = TargetSelector.GetTarget(E.Range, DamageType.Magical);

                if (t != null)
                {
                    var pred = E.GetPrediction(t);

                    if (pred != null && t.IsValidTarget() && pred.HitChancePercent >= HarassMenu["eSlider"].Cast<Slider>().CurrentValue)
                    {
                        E.Cast(pred.CastPosition);
                    }
                }
            }

            if (useE && E.IsReady() && LuxEObject != null)
            {
                var target =
                    EntityManager.Heroes.Enemies.Where(
                        t => t.IsValidTarget() && t.Distance(LuxEObject.Position) <= LuxEObject.BoundingRadius);

                if (target.Any() && LuxEObject != null)
                {
                    E2.Cast();
                }
            }
        }

        /// <summary>
        /// Does LaneClear
        /// </summary>
        private static void LaneClear()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 1175, SkillShotType.Linear, 250, 85, 1150);
            E = new Spell.Skillshot(SpellSlot.E, 1200, SkillShotType.Circular, 250, 280, 950);
            R = new Spell.Skillshot(SpellSlot.R, 3300, SkillShotType.Linear, 1000, 190, int.MaxValue);

            var useQ = LaneClearMenu["useQ"].Cast<CheckBox>().CurrentValue;
            var useE = LaneClearMenu["useE"].Cast<CheckBox>().CurrentValue;
            var useR = LaneClearMenu["useR"].Cast<CheckBox>().CurrentValue;
            var useQs = LaneClearMenu["useQs"].Cast<Slider>().CurrentValue;
            var useEs = LaneClearMenu["useEs"].Cast<Slider>().CurrentValue;
            var useRs = LaneClearMenu["useRs"].Cast<Slider>().CurrentValue;

            if (useQ && Q.IsReady() && ManaManager(SpellSlot.Q))
            {
                var target = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsValidTarget() && Q.IsInRange(t));
                var pred = EntityManager.MinionsAndMonsters.GetLineFarmLocation(target, Q.Width, (int)Q.Range);

                if (pred.HitNumber >= useQs)
                {
                    Q.Cast(pred.CastPosition);
                }
            }

            if (useE && E.IsReady() && ManaManager(SpellSlot.E) && LuxEObject == null)
            {
                var target = EntityManager.MinionsAndMonsters.GetLaneMinions(
                    EntityManager.UnitTeam.Enemy,
                    Player.Instance.ServerPosition,
                    E.Radius);

                var pred = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(target, E.Width, (int)E.Range);

                if (pred.HitNumber >= useEs)
                {
                    E.Cast(pred.CastPosition);
                }
            }

            if (useE && E.IsReady() && LuxEObject != null)
            {
                var target =
                    EntityManager.MinionsAndMonsters.EnemyMinions.Where(
                        t => t.IsValidTarget() && t.Distance(LuxEObject.Position) <= LuxEObject.BoundingRadius);

                if (target.Any() && LuxEObject != null)
                {
                    E2.Cast();
                }
            }

            if (!useR || !R.IsReady() || !ManaManager(SpellSlot.R))
            {
                return;
            }
            var rTarget = EntityManager.MinionsAndMonsters.GetLaneMinions(
                EntityManager.UnitTeam.Enemy,
                Player.Instance.ServerPosition,
                R.Radius);

            if (rTarget == null)
            {
                return;
            }
            var rPrediction = EntityManager.MinionsAndMonsters.GetLineFarmLocation(rTarget, R.Width, (int)R.Range);

            if (rPrediction.HitNumber >= useRs)
            {
                R.Cast(rPrediction.CastPosition);
            }
        }

        /// <summary>
        /// Does KillSteal
        /// </summary>
        private static void KillSteal()
        {
            if (KillStealMenu["useQ"].Cast<CheckBox>().CurrentValue)
            {
                if (Q.IsReady())
                {
                    return;
                }

                var enemies =
                    EntityManager.Heroes.Enemies.Where(
                        t =>
                        t.IsValidTarget() && Q.IsInRange(t) && Player.Instance.GetSpellDamage(t, SpellSlot.Q) >= t.Health);
                
                // Resharper op
                foreach (var pred in enemies.Select(t => Q.GetPrediction(t)).Where(pred => pred != null).Where(pred => pred.HitChancePercent >= KillStealMenu["qSlider"].Cast<Slider>().CurrentValue))
                {
                    Q.Cast(pred.CastPosition);
                }
            }
            if (KillStealMenu["useE"].Cast<CheckBox>().CurrentValue)
            {
                if (E.IsReady() && LuxEObject == null)
                {
                    var enemies =
                        EntityManager.Heroes.Enemies.Where(
                            t =>
                            t.IsValidTarget() && E.IsInRange(t)
                            && Player.Instance.GetSpellDamage(t, SpellSlot.E) >= t.Health);

                    foreach (var pred in enemies.Select(t => E.GetPrediction(t)).Where(pred => pred != null).Where(pred => pred.HitChancePercent >= KillStealMenu["eSlider"].Cast<Slider>().CurrentValue))
                    {
                        E.Cast(pred.CastPosition);
                    }                    
                }
                else if (!E.IsReady() && LuxEObject != null)
                {
                    var enemies =
                        EntityManager.Heroes.Enemies.Where(
                            t =>
                            t.IsValidTarget() && t.Distance(LuxEObject.Position) <= LuxEObject.BoundingRadius
                            && Player.Instance.GetSpellDamage(t, SpellSlot.E) >= t.Health);

                    if (enemies.Any() && LuxEObject != null)
                    {
                        E2.Cast();
                    }
                }
            }
            if (KillStealMenu["useR"].Cast<CheckBox>().CurrentValue)
            {
                if (!R.IsReady())
                {
                    return;
                }

                var enemies =
                    EntityManager.Heroes.Enemies.Where(
                        t =>
                        t.IsValidTarget() && R.IsInRange(t)
                        && Player.Instance.GetSpellDamage(t, SpellSlot.R) + LuxPassiveDamage(t) >= t.Health);

                foreach (var pred in enemies.Select(t => R.GetPrediction(t)).Where(pred => pred != null).Where(pred => pred.HitChancePercent >= KillStealMenu["rSlider"].Cast<Slider>().CurrentValue))
                {
                    R.Cast(pred.CastPosition);
                }
            }
        }

        /// <summary>
        /// Does Auto W
        /// </summary>
        private static void AutoW()
        {
            if (!W.IsReady())
            {
                return;
            }

            var allies =
                EntityManager.Heroes.Allies.Where(
                    t =>
                    MiscMenu["autoW_" + t.BaseSkinName].Cast<CheckBox>().CurrentValue
                    && MiscMenu["hpW"].Cast<Slider>().CurrentValue >= t.HealthPercent && t.IsValidTarget()
                    && W.IsInRange(t)).ToList();

            if (!allies.Any())
            {
                return;
            }

            var pred =
                Prediction.Position.PredictLinearMissile(
                    allies.OrderByDescending(t => t.HealthPercent).FirstOrDefault(),
                    W.Range,
                    W.Radius,
                    W.CastDelay,
                    W.Speed,
                    2,
                    Player.Instance.ServerPosition);

            if (pred.HitChance >= HitChance.High)
            {
                W.Cast(pred.CastPosition);
            }
        }
    }
}
