namespace EloBuddy_PandaTeemo
{
    using System;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using SharpDX;

    /// <summary>
    /// Made by KarmaPanda. Ported from LeagueSharp.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Teemo's Name
        /// </summary>
        private const string ChampionName = "Teemo";

        /// <summary>
        /// Array of ADC Names
        /// </summary>
        private static readonly string[] Marksman = { "Ashe", "Caitlyn", "Corki", "Draven", "Ezreal", "Jinx", "Kalista", "KogMaw", "Lucian", "MissFortune", "Quinn", "Sivir", "Teemo", "Tristana", "Twitch", "Urgot", "Varus", "Vayne" };

        /// <summary>
        /// Spell Q
        /// </summary>
        public static Spell.Targeted Q;

        /// <summary>
        /// Spell W
        /// </summary>
        public static Spell.Active W;

        /// <summary>
        /// Spell E
        /// </summary>
        public static Spell.Active E;

        /// <summary>
        /// Spell R
        /// </summary>
        public static Spell.Skillshot R;

        /// <summary>
        /// Last time R was Used.
        /// </summary>
        public static int lastR;

        /// <summary>
        /// Initializes Shroom Positions
        /// </summary>
        private static ShroomTables shroomPositions;

        /// <summary>
        /// Initializes FileHandler
        /// </summary>
        private static FileHandler fileHandler;

        /// <summary>
        /// Initializes the Menu
        /// </summary>
        public static Menu PandaTeemo, ComboMenu, HarassMenu, LaneClearMenu, JungleClearMenu, KillStealMenu, FleeMenu, DrawingMenu, InterruptMenu, MiscMenu, debug;

        /// <summary>
        /// Gets the player.
        /// </summary>
        private static AIHeroClient PlayerInstance
        {
            get { return EloBuddy.Player.Instance; }
        }

        /// <summary>
        /// Gets Q Damage. Credit to Fluxy.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static float QDamage(Obj_AI_Base target)
        {
            return PlayerInstance.CalculateDamageOnUnit(target, DamageType.Magical,
                (float)(new[] { 80, 125, 170, 215, 260 }[Program.Q.Level] + 0.8 * PlayerInstance.FlatMagicDamageMod));
        }

        public static float DynamicQRange()
        {
            if (Q.IsReady())
            {
                return Q.Range;
            }
            return PlayerInstance.GetAutoAttackRange();
        }

        /// <summary>
        /// Gets Teemo E Damage
        /// </summary>
        /// <param name="minion">The Minion that is being attacked</param>
        /// <returns>The Damage Done to the unit.</returns>
        public static double TeemoE(Obj_AI_Base target)
        {
            { return PlayerInstance.GetSpellDamage(target, SpellSlot.E); }
        }

        /// <summary>
        /// Gets the R Range.
        /// </summary>
        /*public static int RRange
        {
            get { return 300 * R.Level; }
        }*/

        /// <summary>
        /// Called when program starts
        /// </summary>
        private static void Main()
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        /// <summary>
        /// Load when game starts.
        /// </summary>
        /// <param name="args"></param>
        static void Loading_OnLoadingComplete(EventArgs args)
        {
            // Checks if Player is Teemo
            if (PlayerInstance.BaseSkinName != ChampionName)
            {
                return;
            }

            Bootstrap.Init(null);

            Q = new Spell.Targeted(SpellSlot.Q, 680);
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Active(SpellSlot.E);
            R = new Spell.Skillshot(SpellSlot.R, 300, SkillShotType.Circular, 500, 1000, 120);

            // Menu
            PandaTeemo = MainMenu.AddMenu("PandaTeemo", "PandaTeemo");

            // Combo Menu
            ComboMenu = PandaTeemo.AddSubMenu("Combo", "Combo");
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.AddSeparator();
            ComboMenu.Add("qcombo", new CheckBox("Use Q in Combo", true));
            ComboMenu.Add("wcombo", new CheckBox("Use W in Combo", true));
            ComboMenu.Add("rcombo", new CheckBox("Kite with R in Combo", true));
            ComboMenu.Add("useqADC", new CheckBox("Use Q only on ADC during Combo", false));
            ComboMenu.Add("wCombat", new CheckBox("Use W if enemy is in range only", true));
            ComboMenu.Add("rCharge", new Slider("Charges of R before using R", 2, 1, 3));
            ComboMenu.Add("checkCamo", new CheckBox("Prevents combo being activated while stealth in brush", false));

            // Harass Menu
            HarassMenu = PandaTeemo.AddSubMenu("Harass", "Harass");
            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.AddSeparator();
            HarassMenu.Add("qharass", new CheckBox("Harass with Q", true));

            // LaneClear Menu
            LaneClearMenu = PandaTeemo.AddSubMenu("LaneClear", "LaneClear");
            LaneClearMenu.AddGroupLabel("LaneClear Settings");
            LaneClearMenu.AddSeparator();
            LaneClearMenu.Add("qclear", new CheckBox("LaneClear with Q", true));
            LaneClearMenu.Add("qManaManager", new Slider("Q Mana Manager", 75, 0, 100));
            LaneClearMenu.Add("attackTurret", new CheckBox("Attack Turret", true));
            LaneClearMenu.Add("attackWard", new CheckBox("Attack Ward", true));
            LaneClearMenu.Add("rclear", new CheckBox("LaneClear with R", true));
            LaneClearMenu.Add("userKill", new CheckBox("Use R only if Killable", false));
            LaneClearMenu.Add("minionR", new Slider("Minion for R", 3, 1, 4));

            // JungleClear Menu
            JungleClearMenu = PandaTeemo.AddSubMenu("JungleClear", "JungleClear");
            JungleClearMenu.AddGroupLabel("JungleClear Settings");
            JungleClearMenu.AddSeparator();
            JungleClearMenu.Add("qclear", new CheckBox("JungleClear with Q", true));
            JungleClearMenu.Add("rclear", new CheckBox("JungleClear with R", true));

            // Interrupter && Gapcloser
            InterruptMenu = PandaTeemo.AddSubMenu("Interrupt / Gapcloser", "Interrupt");
            InterruptMenu.AddGroupLabel("Interruptter and Gapcloser Setting");
            InterruptMenu.AddSeparator();
            InterruptMenu.Add("intq", new CheckBox("Interrupt with Q", true));
            InterruptMenu.Add("gapR", new CheckBox("Gapclose with R", true));

            // KillSteal Menu
            KillStealMenu = PandaTeemo.AddSubMenu("KillSteal", "KSMenu");
            KillStealMenu.AddGroupLabel("KillSteal Settings");
            KillStealMenu.AddSeparator();
            KillStealMenu.Add("KSQ", new CheckBox("KillSteal with Q", true));
            KillStealMenu.Add("KSR", new CheckBox("KillSteal with R", true));
            //KillStealMenu.Add("KSAA", new CheckBox("KillSteal with AutoAttack", true));

            // Flee Menu
            FleeMenu = PandaTeemo.AddSubMenu("Flee Menu", "Flee");
            FleeMenu.AddGroupLabel("Flee Settings");
            FleeMenu.AddSeparator();
            FleeMenu.Add("w", new CheckBox("Use W while Flee", true));
            FleeMenu.Add("r", new CheckBox("Use R while Flee", true));
            FleeMenu.Add("rCharge", new Slider("Charges of R before using R", 2, 1, 3));

            // Drawing Menu
            DrawingMenu = PandaTeemo.AddSubMenu("Drawing", "Drawing");
            DrawingMenu.AddGroupLabel("Drawing Settings");
            DrawingMenu.AddSeparator();
            DrawingMenu.Add("drawQ", new CheckBox("Draw Q Range", true));
            DrawingMenu.Add("drawR", new CheckBox("Draw R Range", true));
            DrawingMenu.Add("colorBlind", new CheckBox("Colorblind Mode", false));
            DrawingMenu.Add("drawautoR", new CheckBox("Draw Important Shroom Areas", true));
            DrawingMenu.Add("DrawVision", new Slider("Shroom Vision", 1500, 2500, 1000));

            // Debug Menu
            debug = PandaTeemo.AddSubMenu("Debug", "debug");
            debug.Add("debugdraw", new CheckBox("Draw Coords", false));
            debug.Add("x", new Slider("Where to draw X", 500, 0, 1920));
            debug.Add("y", new Slider("Where to draw Y", 500, 0, 1080));
            debug.Add("debugpos", new CheckBox("Draw Custom Shroom Locations Coordinates", true));

            // Misc
            MiscMenu = PandaTeemo.AddSubMenu("Misc", "Misc");
            MiscMenu.AddGroupLabel("Misc Settings");
            MiscMenu.AddSeparator();
            MiscMenu.Add("autoQ", new CheckBox("Automatic Q", false));
            MiscMenu.Add("autoW", new CheckBox("Automatic W", false));
            MiscMenu.Add("autoR", new CheckBox("Auto Place Shrooms in Important Places", true));
            MiscMenu.Add("rCharge", new Slider("Charges of R before using R in AutoShroom", 2, 1, 3));
            MiscMenu.Add("autoRPanic", new KeyBind("Panic Key for Auto R", false, KeyBind.BindTypes.HoldActive, 84));
            MiscMenu.Add("customLocation", new CheckBox("Use Custom Location for Auto Shroom (Requires Reload)", true));
            MiscMenu.AddSeparator();
            MiscMenu.Add("checkAA", new CheckBox("Subtract Range for Q (checkAA)", true));
            MiscMenu.Add("checkaaRange", new Slider("How many to subtract from Q Range (checkAA)", 100, 0, 180));

            // Events
            Game.OnTick += Game_OnTick;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            EloBuddy.Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;

            EloBuddy.Chat.Print("<font color = '#01DF3A'>PandaTeemo EloBuddy Edition Loaded by KarmaPanda</font>");

            // Loads ShroomPosition
            fileHandler = new FileHandler();
            shroomPositions = new ShroomTables();
        }

        /// <summary>
        /// Interrupts interruptable spell
        /// </summary>
        /// <param name="sender">Enemy</param>
        /// <param name="e">The Arguments</param>
        static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            var intq = InterruptMenu["intq"].Cast<CheckBox>().CurrentValue;

            if (intq && Q.IsReady())
            {
                if (sender != null)
                {
                    Q.Cast(sender);
                }
            }
        }

        /// <summary>
        /// Gapcloses whenever possible.
        /// </summary>
        /// <param name="sender">Enemy</param>
        /// <param name="e">The Arguments</param>
        static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            var gapR = InterruptMenu["gapR"].Cast<CheckBox>().CurrentValue;

            if (gapR && sender.IsValidTarget() && sender.IsFacing(PlayerInstance))
            {
                var pred = R.GetPrediction(sender);
                
                if (pred.HitChance == HitChance.High)
                {
                    R.Cast(sender.Position);
                }
            }
        }

        /// <summary>
        /// After Attack
        /// </summary>
        /// <param name="target">The Target that was attacked</param>
        /// <param name="args">The Args</param>
        static void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            var t = target as AIHeroClient;
            var checkAa = MiscMenu["checkAA"].Cast<CheckBox>().CurrentValue;
            var checkaaRange = MiscMenu["checkaaRange"].Cast<Slider>().CurrentValue;

            if (t != null && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                var useQCombo = ComboMenu["qcombo"].Cast<CheckBox>().CurrentValue;
                var targetAdc = ComboMenu["useqADC"].Cast<CheckBox>().CurrentValue;
                
                #region Check AA
                
                if (checkAa)
                {
                    if (t != null)
                    {
                        if (targetAdc)
                        {
                            foreach (var adc in Marksman)
                            {
                                if (t.Name == adc && useQCombo && Q.IsReady() && PlayerInstance.Distance(target) < Q.Range - checkaaRange)
                                {
                                    Q.Cast(t);
                                }
                                else
                                {
                                    return;
                                }
                            }
                        }
                        else
                        {
                            if (useQCombo && Q.IsReady() && PlayerInstance.Distance(target) < Q.Range - checkaaRange)
                            {
                                Q.Cast(t);
                            }
                            else
                            {
                                return;
                            }
                        }
                        
                    }
                }

                #endregion

                #region No Check AA

                else
                {
                    if (t != null)
                    {
                        if (targetAdc)
                        {
                            foreach (var adc in Marksman)
                            {
                                if (t.Name == adc && useQCombo && Q.IsReady() && Q.IsInRange(t))
                                {
                                    Q.Cast(t);
                                }
                                else
                                {
                                    return;
                                }
                            }
                        }
                        else
                        {
                            if (useQCombo && Q.IsReady() && Q.IsInRange(t))
                            {
                                Q.Cast(t);
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                }

                #endregion
            }
            if (t != null && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                var useQHarass = HarassMenu["qharass"].Cast<CheckBox>().CurrentValue;

                #region Q Cast

                if (checkAa)
                {
                    if (t != null)
                    {
                        if (useQHarass && Q.IsReady() && PlayerInstance.Distance(t) < Q.Range - checkaaRange)
                        {
                            Q.Cast(t);
                        }
                    }
                }
                else
                {
                    if (useQHarass && Q.IsReady() && PlayerInstance.Distance(t) < Q.Range)
                    {
                        Q.Cast(t);
                    }
                }

                #endregion
            }
        }

        /// <summary>
        /// Before Attack Equivalent
        /// </summary>
        /// <param name="target">The Target</param>
        /// <param name="args">Before Attack Arg</param>
        static void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            var t = target as AIHeroClient;
            var m = target as Obj_AI_Base;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                foreach (var minion in ObjectManager.Get<Obj_AI_Base>().Where(creep => creep.IsMinion && creep.IsEnemy && PlayerInstance.IsInAutoAttackRange(creep)).OrderBy(creep => creep.Health))
                {
                    if (minion != null)
                    {
                        if (minion.Health <= ObjectManager.Player.GetAutoAttackDamage(minion) + TeemoE(minion))
                        {
                            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                        }
                    }
                }
            }

            else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                var enemy = HeroManager.Enemies.Where(hero => PlayerInstance.IsInAutoAttackRange(hero)).OrderBy(hero => hero.Health).FirstOrDefault();
                var minion = ObjectManager.Get<Obj_AI_Base>().Where(unit => unit.IsMinion && unit.IsEnemy && PlayerInstance.IsInAutoAttackRange(unit)).OrderBy(unit => unit.Health).FirstOrDefault();
                
                #region Auto Attack

                if (minion == null)
                {
                    if (enemy != null)
                    {
                        if (PlayerInstance.IsInAutoAttackRange(enemy))
                        {
                            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, enemy);
                        }
                    }
                }
                else
                {
                    if (enemy != null
                        && minion.Health > ObjectManager.Player.GetAutoAttackDamage(minion) + TeemoE(minion))
                    {
                        if (PlayerInstance.IsInAutoAttackRange(enemy))
                        {
                            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, enemy);
                        }
                    }
                    else if (minion.Health <= ObjectManager.Player.GetAutoAttackDamage(minion) + TeemoE(minion))
                    {
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                    }
                }

                #endregion
            }
            else
            {
                args.Process = true;
            }
        }

        /// <summary>
        /// Whenever a Spell gets Casted
        /// </summary>
        /// <param name="sender">The Player</param>
        /// <param name="args">The Spell</param>
        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                //Game.PrintChat(args.SData.Name.ToLower());
                if (args.SData.Name.ToLower() == "teemorcast")
                {
                    lastR = Environment.TickCount;
                }
            }
        }

        /// <summary>
        /// Checks if there is shroom in location
        /// </summary>
        /// <param name="position">The location of check</param>
        /// <returns>If that location has a shroom.</returns>
        private static bool IsShroomed(Vector3 position)
        {
            return ObjectManager.Get<Obj_AI_Base>().Where(obj => obj.Name == "Noxious Trap").Any(obj => position.Distance(obj.Position) <= 250);
        }

        /// <summary>
        /// Does the Combo
        /// </summary>
        private static void Combo()
        {
            var checkCamo = ComboMenu["checkCamo"].Cast<CheckBox>().CurrentValue;

            if (checkCamo && PlayerInstance.HasBuff("CamouflageStealth"))
            {
                return;
            }

            var enemies = HeroManager.Enemies.FirstOrDefault(t => t.IsValidTarget() && PlayerInstance.IsInAutoAttackRange(t) && t.IsEnemy);
            var rtarget = TargetSelector.GetTarget(R.Range, DamageType.Magical);
            var useW = ComboMenu["wcombo"].Cast<CheckBox>().CurrentValue;
            var useR = ComboMenu["rcombo"].Cast<CheckBox>().CurrentValue;
            var wCombat = ComboMenu["wCombat"].Cast<CheckBox>().CurrentValue;
            var rCount = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Ammo;
            var rCharge = ComboMenu["rCharge"].Cast<Slider>().CurrentValue;

            if (W.IsReady() && useW && !wCombat)
            {
                W.Cast();
            }

            if (useW && wCombat)
            {
                if (W.IsReady() && enemies != null)
                {
                    W.Cast();
                }
            }

            if (rtarget != null)
            {
                var predictionR = R.GetPrediction(rtarget);

                if (R.IsReady() 
                    && useR 
                    && R.IsInRange(rtarget)
                    && rCharge <= rCount 
                    && rtarget.IsValidTarget() 
                    && !IsShroomed(predictionR.CastPosition))
                {
                    if (predictionR.HitChance == HitChance.High)
                    {
                        R.Cast(predictionR.CastPosition);
                    }
                }
            }
            
            // Temporarily Removed Double Shroom Method

            /*else if (R.IsReady() && useR && rCharge <= rCount)
            {
                var shroom = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(t => t.Name == "Noxious Trap");

                if (shroom != null)
                {
                    var shroomPosition = shroom.Position;
                    var predictionPosition = shroomPosition.Extend(rtarget.Position, Player.CharData.SelectionRadius * R.Level + 2);

                    if (R.IsInRange(rtarget) && IsShroomed(shroomPosition))
                    {
                        R.Cast(predictionPosition);
                    }
                }
            }*/
        }

        /// <summary>
        /// Kill Steal
        /// </summary>
        private static void KillSteal()
        {
            var ksq = KillStealMenu["KSQ"].Cast<CheckBox>().CurrentValue;
            var ksr = KillStealMenu["KSR"].Cast<CheckBox>().CurrentValue;
            //var ksaa = KillStealMenu["KSAA"].Cast<CheckBox>().CurrentValue;

            if (ksq)
            {
                var target = HeroManager.Enemies.Where(t => t.IsValidTarget()
                    && Q.IsInRange(t)
                    && PlayerInstance.GetSpellDamage(t, SpellSlot.R) >= t.Health).OrderBy(t => t.Health).FirstOrDefault();

                if (target != null && Q.IsReady())
                {
                    Q.Cast(target);
                }
            }

            if (ksr)
            {
                var target = HeroManager.Enemies.Where(t => t.IsValidTarget()
                    && R.IsInRange(t)
                    && PlayerInstance.GetSpellDamage(t, SpellSlot.R) >= t.Health).OrderBy(t => t.Health).FirstOrDefault();

                if (target != null && R.IsReady())
                {
                    var pred = R.GetPrediction(target);
                    
                    if (pred.HitChance == HitChance.High)
                    {
                        R.Cast(pred.CastPosition);
                    }
                }
            }

            /*if (ksaa)
            {
                var aatarget = HeroManager.Enemies.Where(t =>
                    t.IsValidTarget()
                    && PlayerInstance.IsInAutoAttackRange(t)
                    && PlayerInstance.GetAutoAttackDamage(t) + TeemoE(t) >= t.Health).OrderBy(t => t.Health).FirstOrDefault();

                if (aatarget != null)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, aatarget);
                }
            }*/
        }

        /// <summary>
        /// Lane Clear (Still waiting for GetCircularFarmLocation)
        /// </summary>
        private static void LaneClear()
        {
            var qClear = LaneClearMenu["qclear"].Cast<CheckBox>().CurrentValue;
            var qManaManager = LaneClearMenu["qManaManager"].Cast<Slider>().CurrentValue;
            var qMinion = ObjectManager.Get<Obj_AI_Base>().Where(t => Q.IsInRange(t) && t.IsValidTarget() && t.IsMinion && t.IsEnemy).OrderBy(t => t.Health).FirstOrDefault();

            if (qMinion != null)
            {
                if (Q.IsReady()
                && qClear
                && qMinion.Health < QDamage(qMinion) 
                && qManaManager <= (int)PlayerInstance.ManaPercent)
                {
                    Q.Cast(qMinion);
                }
            }

            var allMinionsR = ObjectManager.Get<Obj_AI_Base>().Where(t => R.IsInRange(t) && t.IsValidTarget() && t.IsMinion && t.IsEnemy).OrderBy(t => t.Health);
            //var rangedMinionsR = ObjectManager.Get<Obj_AI_Base>().Where(t => R.IsInRange(t) && t.IsValidTarget() && t.IsMinion && t.IsEnemy && t.IsRanged).OrderBy(t => t.Health);
            if (allMinionsR == null)
            {
                return;
            }
            var rLocation = Prediction.Position.PredictCircularMissileAoe(allMinionsR.ToArray(), R.Range, R.Radius, R.CastDelay, R.Speed, PlayerInstance.Position);//R.GetPrediction(allMinionsR.FirstOrDefault());
            //var r2Location = Prediction.Position.PredictCircularMissileAoe(rangedMinionsR.ToArray(), R.Range, R.Radius, R.CastDelay, R.Speed, PlayerInstance.Position);//R.GetPrediction(rangedMinionsR.FirstOrDefault());
            var useR = LaneClearMenu["rclear"].Cast<CheckBox>().CurrentValue;
            var userKill = LaneClearMenu["userKill"].Cast<CheckBox>().CurrentValue;
            var minionR = LaneClearMenu["minionR"].Cast<Slider>().CurrentValue;

            if (useR)
            {
                if (rLocation == null)
                {
                    return;
                }
                else
                {
                    if (userKill)
                    {
                        foreach (var pred in rLocation)
                        {
                            if (pred.CollisionObjects.Count() >= minionR)
                            {
                                if (R.IsReady() && R.IsInRange(pred.CastPosition))
                                {
                                    R.Cast(pred.CastPosition);
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var pred in rLocation)
                        {
                            if (pred.CollisionObjects.Count() >= minionR)
                            {
                                if (R.IsReady() && R.IsInRange(pred.CastPosition))
                                {
                                    R.Cast(pred.CastPosition);
                                }
                            }
                        }
                    }
                }


                #region Old Logic

                /*if (minionR <= rLocation.CollisionObjects.Count() && useR 
                    || minionR <= r2Location.CollisionObjects.Count() && useR 
                    || minionR <= rLocation.CollisionObjects.Count() + r2Location.CollisionObjects.Count() && useR)
                {
                    if (userKill)
                    {
                        foreach (var minion in allMinionsR)
                        {
                            if (minion.Health <= ObjectManager.Player.GetSpellDamage(minion, SpellSlot.R)
                                && R.IsReady()
                                && R.IsInRange(rLocation.CastPosition)
                                && !IsShroomed(rLocation.CastPosition)
                                && minionR <= rLocation.CollisionObjects.Count())
                            {
                                R.Cast(rLocation.CastPosition);
                                return;
                            }

                            if (minion.Health <= ObjectManager.Player.GetSpellDamage(minion, SpellSlot.R)
                                && R.IsReady()
                                && R.IsInRange(r2Location.CastPosition)
                                && !IsShroomed(r2Location.CastPosition)
                                && minionR <= r2Location.CollisionObjects.Count())
                            {
                                R.Cast(r2Location.CastPosition);
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (R.IsReady()
                            && R.IsInRange(rLocation.CastPosition)
                            && !IsShroomed(rLocation.CastPosition)
                            && minionR <= rLocation.CollisionObjects.Count())
                        {
                            R.Cast(rLocation.CastPosition);
                        }
                        else if (R.IsReady()
                            && R.IsInRange(r2Location.CastPosition)
                            && !IsShroomed(r2Location.CastPosition)
                            && minionR <= r2Location.CollisionObjects.Count())
                        {
                            R.Cast(r2Location.CastPosition);
                        }
                    }
                }*/

                #endregion

            }
        }

        /// <summary>
        /// Does the JungleClear
        /// </summary>
        private static void JungleClear()
        {
            var useQ = JungleClearMenu["qclear"].Cast<CheckBox>().CurrentValue;
            var useR = JungleClearMenu["rclear"].Cast<CheckBox>().CurrentValue;
            var ammoR = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Ammo;
            var qManaManager = JungleClearMenu["qManaManager"].Cast<Slider>().CurrentValue;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                var jungleMobQ = ObjectManager.Get<Obj_AI_Base>().Where(t => Q.IsInRange(t) && t.Team == GameObjectTeam.Neutral && t.IsValidTarget()).OrderBy(t => t.MaxHealth).FirstOrDefault();
                var jungleMobR = ObjectManager.Get<Obj_AI_Base>().Where(t => R.IsInRange(t) && t.Team == GameObjectTeam.Neutral && t.IsValidTarget()).OrderBy(t => t.MaxHealth).FirstOrDefault();

                if (useQ && jungleMobQ != null)
                {
                    if (Q.IsReady() && qManaManager <= (int)PlayerInstance.ManaPercent)
                    {
                        Q.Cast(jungleMobQ);
                    }
                }

                if (useR && jungleMobR != null)
                {
                    if (R.IsReady() && ammoR >= 1)
                    {
                        var pred = R.GetPrediction(jungleMobR);

                        if (pred.HitChance == HitChance.High)
                        {
                            R.Cast(pred.CastPosition);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Does the AutoShroom
        /// </summary>
        private static void AutoShroom()
        {
            var autoRPanic = MiscMenu["autoRPanic"].Cast<KeyBind>().CurrentValue;

            if (autoRPanic)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }

            if (!R.IsReady() || PlayerInstance.HasBuff("Recall") || autoRPanic)
            {
                return;
            }

            var target = HeroManager.Enemies.FirstOrDefault(t => R.IsInRange(t) && t.IsValidTarget());

            if (target != null)
            {
                if (target.HasBuff("zhonyasringshield") && R.IsReady() && R.IsInRange(target))
                {
                    R.Cast(target.Position);
                }
            }
            else
            {
                var rCharge = MiscMenu["rCharge"].Cast<Slider>().CurrentValue;
                var rCount = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Ammo;

                if (Game.MapId == EloBuddy.GameMapId.SummonersRift)
                {
                    if (shroomPositions.SummonersRift.Any())
                    {
                        foreach (var place in shroomPositions.SummonersRift.Where(pos => pos.Distance(ObjectManager.Player.Position) <= R.Range && !IsShroomed(pos)))
                        {
                            if (rCharge <= rCount && Environment.TickCount - lastR > 5000)
                            {
                                R.Cast(place);
                            }
                        }
                    }
                }
                else if (Game.MapId == EloBuddy.GameMapId.HowlingAbyss)
                {
                    if (shroomPositions.HowlingAbyss.Any())
                    {
                        foreach (var place in shroomPositions.HowlingAbyss.Where(pos => pos.Distance(ObjectManager.Player.Position) <= R.Range && !IsShroomed(pos)))
                        {
                            if (rCharge <= rCount && Environment.TickCount - lastR > 5000)
                            {
                                R.Cast(place);
                            }
                        }
                    }
                }
                else if (Game.MapId == EloBuddy.GameMapId.CrystalScar)
                {
                    if (shroomPositions.CrystalScar.Any())
                    {
                        foreach (var place in shroomPositions.CrystalScar.Where(pos => pos.Distance(ObjectManager.Player.Position) <= R.Range && !IsShroomed(pos)))
                        {
                            if (rCharge <= rCount && Environment.TickCount - lastR > 5000)
                            {
                                R.Cast(place);
                            }
                        }
                    }
                }
                else if (Game.MapId == EloBuddy.GameMapId.TwistedTreeline)
                {
                    if (shroomPositions.TwistedTreeline.Any())
                    {
                        foreach (var place in shroomPositions.TwistedTreeline.Where(pos => pos.Distance(ObjectManager.Player.Position) <= R.Range && !IsShroomed(pos)))
                        {
                            if (rCharge <= rCount && Environment.TickCount - lastR > 5000)
                            {
                                R.Cast(place);
                            }
                        }
                    }
                }
                else if (Game.MapId.ToString() == "Unknown")
                {
                    if (shroomPositions.ButcherBridge.Any())
                    {
                        foreach (var place in shroomPositions.ButcherBridge.Where(pos => pos.Distance(ObjectManager.Player.Position) <= R.Range && !IsShroomed(pos)))
                        {
                            if (rCharge <= rCount && Environment.TickCount - lastR > 5000)
                            {
                                R.Cast(place);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Does the Flee
        /// </summary>
        private static void Flee()
        {
            // Checks if toggle is on
            var useW = FleeMenu["w"].Cast<CheckBox>().CurrentValue;
            var useR = FleeMenu["r"].Cast<CheckBox>().CurrentValue;
            var rCharge = FleeMenu["rCharge"].Cast<Slider>().CurrentValue;

            // Force move to player's mouse cursor
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            // Uses W if avaliable and if toggle is on
            if (useW && W.IsReady())
            {
                W.Cast(PlayerInstance);
            }

            // Uses R if avaliable and if toggle is on
            if (useR && R.IsReady() && rCharge <= ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Ammo)
            {
                R.Cast(PlayerInstance.Position);
            }
        }

        /// <summary>
        /// Auto Q
        /// </summary>
        private static void AutoQ()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            var allMinionsQ = ObjectManager.Get<Obj_AI_Base>().Where(t => t.IsEnemy && Q.IsInRange(t)).OrderBy(t => t.Health);

            if (target == null)
            {
                return;
            }

            if (Q.IsReady() && allMinionsQ.Count() > 0)
            {
                foreach (var minion in allMinionsQ)
                {
                    if (minion.Health < QDamage(minion) && Q.IsInRange(minion))
                    {
                        Q.Cast(minion);
                    }
                }
            }
            else if (Q.IsReady() && target.IsValidTarget(Q.Range) && PlayerInstance.ManaPercent >= 25)
            {
                Q.Cast(target);
            }
        }

        /// <summary>
        /// Auto W
        /// </summary>
        private static void AutoW()
        {
            if (!W.IsReady())
            {
                return;
            }

            if (W.IsReady())
            {
                W.Cast();
            }
        }

        /// <summary>
        /// Auto Q and W
        /// </summary>
        private static void AutoQw()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            var allMinionsQ = ObjectManager.Get<Obj_AI_Base>().Where(t => t.IsEnemy && Q.IsInRange(t)).OrderBy(t => t.Health);

            if (W.IsReady())
            {
                W.Cast();
            }

            if (target == null)
            {
                return;
            }

            if (Q.IsReady() && allMinionsQ.Count() > 0)
            {
                foreach (var minion in allMinionsQ)
                {
                    if (minion.Health < QDamage(minion) && Q.IsInRange(minion))
                    {
                        Q.Cast(minion);
                    }
                }
            }
            else if (Q.IsReady() && target.IsValidTarget(Q.Range) && PlayerInstance.ManaPercent >= 25)
            {
                Q.Cast(target);
            }
        }

        /// <summary>
        /// Called when Game Updates.
        /// </summary>
        /// <param name="args"></param>
        private static void Game_OnTick(EventArgs args)
        {
            R = new Spell.Skillshot(SpellSlot.R, (uint)(300 * R.Level), SkillShotType.Circular, 500, 1000, 120);

            var autoQ = MiscMenu["autoQ"].Cast<CheckBox>().CurrentValue;
            var autoW = MiscMenu["autoW"].Cast<CheckBox>().CurrentValue;

            if (autoQ && autoW)
            {
                AutoQw();
            }
            else if (autoQ)
            {
                AutoQ();
            }
            else if (autoW)
            {
                AutoW();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                LaneClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                JungleClear();
            }
            
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                Flee();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None))
            {
                // Auto Shroom
                if (MiscMenu["autoR"].Cast<CheckBox>().CurrentValue)
                {
                    AutoShroom();
                }

                // KillSteal
                if (KillStealMenu["KSQ"].Cast<CheckBox>().CurrentValue
                    || KillStealMenu["KSR"].Cast<CheckBox>().CurrentValue)
                {
                    KillSteal();
                }
            }
        }

        /// <summary>
        /// Called when Game Draws
        /// </summary>
        /// <param name="args">
        /// The Args
        /// </param>
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (debug["debugdraw"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawText(
                    (float)debug["x"].Cast<Slider>().CurrentValue,
                    (float)debug["y"].Cast<Slider>().CurrentValue,
                    System.Drawing.Color.Red,
                    PlayerInstance.Position.ToString());
            }

            var drawQ = DrawingMenu["drawQ"].Cast<CheckBox>().CurrentValue;
            var drawR = DrawingMenu["drawR"].Cast<CheckBox>().CurrentValue;
            var colorBlind = DrawingMenu["colorBlind"].Cast<CheckBox>().CurrentValue;
            var player = PlayerInstance.Position;

            if (drawQ && colorBlind)
            {
                EloBuddy.SDK.Rendering.Circle.Draw(Q.IsReady() ? Color.YellowGreen : Color.Red, Q.Range, player);
            }

            if (drawQ && !colorBlind)
            {
                EloBuddy.SDK.Rendering.Circle.Draw(Q.IsReady() ? Color.LightGreen : Color.Red, Q.Range, player);
            }

            if (drawR && colorBlind)
            {
                EloBuddy.SDK.Rendering.Circle.Draw(R.IsReady() ? Color.YellowGreen : Color.Red, R.Range, player);
            }

            if (drawR && !colorBlind)
            {
                EloBuddy.SDK.Rendering.Circle.Draw(R.IsReady() ? Color.LightGreen : Color.Red, R.Range, player);
            }

            var drawautoR = DrawingMenu["drawautoR"].Cast<CheckBox>().CurrentValue;

            if (drawautoR && Game.MapId == EloBuddy.GameMapId.SummonersRift)
            {
                if (shroomPositions.SummonersRift.Any())
                {
                    foreach (var place in shroomPositions.SummonersRift.Where(pos => pos.Distance(ObjectManager.Player.Position) <= DrawingMenu["DrawVision"].Cast<Slider>().CurrentValue))
                    {
                        if (colorBlind)
                        {
                            EloBuddy.SDK.Rendering.Circle.Draw(IsShroomed(place) ? Color.Red : Color.YellowGreen, 100, place);
                        }
                        else
                        {
                            EloBuddy.SDK.Rendering.Circle.Draw(IsShroomed(place) ? Color.Red : Color.LightGreen, 100, place);
                        }
                    }
                }
            }
            else if (drawautoR && Game.MapId == EloBuddy.GameMapId.CrystalScar)
            {
                if (shroomPositions.CrystalScar.Any())
                {
                    foreach (var place in shroomPositions.CrystalScar.Where(pos => pos.Distance(ObjectManager.Player.Position) <= DrawingMenu["DrawVision"].Cast<Slider>().CurrentValue))
                    {
                        if (colorBlind)
                        {
                            EloBuddy.SDK.Rendering.Circle.Draw(IsShroomed(place) ? Color.Red : Color.YellowGreen, 100, place);
                        }
                        else
                        {
                            EloBuddy.SDK.Rendering.Circle.Draw(IsShroomed(place) ? Color.Red : Color.LightGreen, 100, place);
                        }
                    }
                }
            }
            else if (drawautoR && Game.MapId == EloBuddy.GameMapId.HowlingAbyss)
            {
                if (shroomPositions.HowlingAbyss.Any())
                {
                    foreach (var place in shroomPositions.HowlingAbyss.Where(pos => pos.Distance(ObjectManager.Player.Position) <= DrawingMenu["DrawVision"].Cast<Slider>().CurrentValue))
                    {
                        if (colorBlind)
                        {
                            EloBuddy.SDK.Rendering.Circle.Draw(IsShroomed(place) ? Color.Red : Color.YellowGreen, 100, place);
                        }
                        else
                        {
                            EloBuddy.SDK.Rendering.Circle.Draw(IsShroomed(place) ? Color.Red : Color.LightGreen, 100, place);
                        }
                    }
                }
            }
            else if (drawautoR && Game.MapId == EloBuddy.GameMapId.TwistedTreeline)
            {
                if (shroomPositions.TwistedTreeline.Any())
                {
                    foreach (var place in shroomPositions.TwistedTreeline.Where(pos => pos.Distance(ObjectManager.Player.Position) <= DrawingMenu["DrawVision"].Cast<Slider>().CurrentValue))
                    {
                        if (colorBlind)
                        {
                            EloBuddy.SDK.Rendering.Circle.Draw(IsShroomed(place) ? Color.Red : Color.YellowGreen, 100, place);
                        }
                        else
                        {
                            EloBuddy.SDK.Rendering.Circle.Draw(IsShroomed(place) ? Color.Red : Color.LightGreen, 100, place);
                        }
                    }
                }
            }
            else if (drawautoR)
            {
                if (shroomPositions.ButcherBridge.Any())
                {
                    foreach (var place in shroomPositions.ButcherBridge.Where(pos => pos.Distance(ObjectManager.Player.Position) <= DrawingMenu["DrawVision"].Cast<Slider>().CurrentValue))
                    {
                        if (colorBlind)
                        {
                            EloBuddy.SDK.Rendering.Circle.Draw(IsShroomed(place) ? Color.Red : Color.YellowGreen, 100, place);
                        }
                        else
                        {
                            EloBuddy.SDK.Rendering.Circle.Draw(IsShroomed(place) ? Color.Red : Color.LightGreen, 100, place);
                        }
                    }
                }
            }
        }
    }
}