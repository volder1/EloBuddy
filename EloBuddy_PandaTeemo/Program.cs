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
        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        /// <summary>
        /// Gets Q Damage. Credit to Fluxy.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static float QDamage(Obj_AI_Base target)
        {
            return Player.CalculateDamageOnUnit(target, DamageType.Magical,
                (float)(new[] { 80, 125, 170, 215, 260 }[Program.Q.Level] + 0.8 * Player.FlatMagicDamageMod));
        }

        public static float DynamicQRange()
        {
            if (Q.IsReady())
            {
                return Q.Range;
            }
            return Player.GetAutoAttackRange();
        }

        /// <summary>
        /// Gets Teemo E Damage
        /// </summary>
        /// <param name="minion">The Minion that is being attacked</param>
        /// <returns>The Damage Done to the unit.</returns>
        public static double TeemoE(Obj_AI_Base target)
        {
            { return Player.GetSpellDamage(target, SpellSlot.E); }
        }

        /*/// <summary>
        /// Gets the R Range.
        /// </summary>
        public static float RRange
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
            if (Player.BaseSkinName != ChampionName)
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
            KillStealMenu.Add("KSAA", new CheckBox("KillSteal with AutoAttack", true));

            // Flee Menu
            FleeMenu = PandaTeemo.AddSubMenu("Flee Menu", "Flee");
            FleeMenu.AddGroupLabel("Flee Settings");
            FleeMenu.AddSeparator();
            FleeMenu.Add("fleetoggle", new KeyBind("Flee Toggle", false, KeyBind.BindTypes.HoldActive, 65));
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
            Game.OnTick += Game_OnUpdate;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            Gapcloser.OnGapCloser += Gapcloser_OnGapCloser;
            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
            EloBuddy.Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;

            EloBuddy.Chat.Print("<font color = '#01DF3A'>PandaTeemo EloBuddy Edition Loaded by KarmaPanda</font>");

            // Loads ShroomPosition
            fileHandler = new FileHandler();
            shroomPositions = new ShroomTables();
        }

        /// <summary>
        /// Before Attack Equivalent
        /// </summary>
        /// <param name="target">The Target</param>
        /// <param name="args">Before Attack Arg</param>
        static void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                foreach (var minion in ObjectManager.Get<Obj_AI_Base>().Where(t => t.IsEnemy && Player.IsInAutoAttackRange(t)).OrderBy(t => t.Health))
                {
                    if (minion.Health <= ObjectManager.Player.GetAutoAttackDamage(minion) + TeemoE(minion))
                    {
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                var enemy = HeroManager.Enemies.OrderBy(t => t.Health).FirstOrDefault();
                var minion = ObjectManager.Get<Obj_AI_Base>().Where(t => t.IsEnemy && Player.IsInAutoAttackRange(t)).OrderBy(t => t.Health).FirstOrDefault();

                if (minion == null)
                {
                    if (enemy != null
                        && Player.IsInAutoAttackRange(enemy))
                    {
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, enemy);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    if (enemy != null
                        && minion.Health > ObjectManager.Player.GetAutoAttackDamage(minion) + TeemoE(minion)
                        && Player.IsInAutoAttackRange(enemy))
                    {
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, enemy);
                    }
                    else if (minion.Health <= ObjectManager.Player.GetAutoAttackDamage(minion) + TeemoE(minion))
                    {
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else
            {
                args.Process = true;
            }
        }

        /// <summary>
        /// After Attack Equivalent.
        /// </summary>
        /// <param name="target">The Target to Attack</param>
        /// <param name="args">The Attack Arg</param>
        static void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            var useQCombo = ComboMenu["qcombo"].Cast<CheckBox>().CurrentValue;
            var useQHarass = ComboMenu["qharass"].Cast<CheckBox>().CurrentValue;
            var targetAdc = ComboMenu["useqADC"].Cast<CheckBox>().CurrentValue;
            var checkAa = ComboMenu["checkAA"].Cast<CheckBox>().CurrentValue;
            var checkaaRange = (float)ComboMenu["checkaaRange"].Cast<Slider>().CurrentValue;
            var t = target as Obj_AI_Base;

            if (target != null && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (checkAa)
                {
                    if (targetAdc)
                    {
                        foreach (var adc in Marksman)
                        {
                            if (t.Name == adc && useQCombo && Q.IsReady() && (t.Distance(Player) <= DynamicQRange() - checkaaRange))
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
                        if (useQCombo && Q.IsReady() && (t.Distance(Player) <= DynamicQRange() - checkaaRange))
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

            if (target != null && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                if (checkAa)
                {
                    if (useQHarass && Q.IsReady() && Q.IsInRange(target.Position - checkaaRange))
                    {
                        Q.Cast(t);
                    }
                }
                else
                {
                    if (useQHarass && Q.IsReady() && Q.IsInRange(target.Position))
                    {
                        Q.Cast(t);
                    }
                }
            }
        }

        /// <summary>
        /// Gapcloses whenever possible.
        /// </summary>
        /// <param name="sender">Enemy</param>
        /// <param name="e">The Arguments</param>
        static void Gapcloser_OnGapCloser(AIHeroClient sender, Gapcloser.GapCloserEventArgs e)
        {
            var gapR = InterruptMenu["gapR"].Cast<CheckBox>().CurrentValue;

            if (gapR && sender.IsValidTarget() && sender.IsFacing(Player) && sender.IsTargetable)
            {
                R.Cast(sender.Position);
            }
        }

        /// <summary>
        /// Interrupts interruptable spell
        /// </summary>
        /// <param name="sender">Enemy</param>
        /// <param name="e">The Arguments</param>
        static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, InterruptableSpellEventArgs e)
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

            if (checkCamo && Player.HasBuff("CamouflageStealth"))
            {
                return;
            }

            var enemies = HeroManager.Enemies.FirstOrDefault(t => t.IsValidTarget() && Player.IsInAutoAttackRange(t));
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

            if (enemies == null)
            {
                return;
            }

            if (useW && wCombat)
            {
                if (W.IsReady())
                {
                    W.Cast();
                }
            }

            var predictionR = R.GetPrediction(rtarget);

            if (R.IsReady() && useR && R.IsInRange(rtarget) && rCharge <= rCount && rtarget.IsValidTarget() && !IsShroomed(predictionR.CastPosition))
            {
                R.Cast(predictionR.CastPosition);
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
            var ksaa = KillStealMenu["KSAA"].Cast<CheckBox>().CurrentValue;

            if (ksaa)
            {
                var aatarget = HeroManager.Enemies.Where(t =>
                    t.IsValidTarget()
                    && Player.IsInAutoAttackRange(t)
                    && Player.GetAutoAttackDamage(t) + TeemoE(t) >= t.Health).OrderBy(t => t.Health).FirstOrDefault();

                if (aatarget != null)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, aatarget);
                }
                else
                {
                    return;
                }
            }

            if (ksq)
            {
                var target = HeroManager.Enemies.Where(t => t.IsValidTarget()
                    && Q.IsInRange(t)
                    && Player.GetSpellDamage(t, SpellSlot.R) >= t.Health).OrderBy(t => t.Health).FirstOrDefault();

                if (target != null && Q.IsReady())
                {
                    Q.Cast(target);
                }
                else
                {
                    return;
                }
            }

            if (ksr)
            {
                var target = HeroManager.Enemies.Where(t => t.IsValidTarget()
                    && R.IsInRange(t)
                    && Player.GetSpellDamage(t, SpellSlot.R) >= t.Health).OrderBy(t => t.Health).FirstOrDefault();

                if (target != null && R.IsReady())
                {
                    R.Cast(target);
                }
            }
        }

        /// <summary>
        /// Moved Q LaneClear to Here.
        /// </summary>
        private static void LaneClear()
        {
            var qClear = LaneClearMenu["qclear"].Cast<CheckBox>().CurrentValue;
            var qMinion = ObjectManager.Get<Obj_AI_Base>().Where(t => Q.IsInRange(t) && t.IsValidTarget() && t.IsMinion && t.IsEnemy).OrderBy(t => t.Health).FirstOrDefault();
            var qManaManager = LaneClearMenu["qManaManager"].Cast<Slider>().CurrentValue;

            if (qMinion != null && Q.IsReady() && qClear && qMinion.Health <= QDamage(qMinion) && qManaManager <= (int)Player.ManaPercent)
            {
                Q.Cast(qMinion);
            }

            var allMinionsR = ObjectManager.Get<Obj_AI_Base>().Where(t => R.IsInRange(t) && t.IsValidTarget() && t.IsMinion && t.IsEnemy).OrderBy(t => t.Health);
            var rangedMinionsR = ObjectManager.Get<Obj_AI_Base>().Where(t => R.IsInRange(t) && t.IsValidTarget() && t.IsMinion && t.IsEnemy && t.IsRanged).OrderBy(t => t.Health);
            var rLocation = R.GetPrediction(allMinionsR.FirstOrDefault());
            var r2Location = R.GetPrediction(rangedMinionsR.FirstOrDefault());
            var useR = LaneClearMenu["rclear"].Cast<CheckBox>().CurrentValue;
            var userKill = LaneClearMenu["userKill"].Cast<CheckBox>().CurrentValue;
            var minionR = LaneClearMenu["minionR"].Cast<Slider>().CurrentValue;

            if (minionR <= rLocation.CollisionObjects.Count() && useR
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
                    if (Q.IsReady() && qManaManager <= (int)Player.ManaPercent)
                    {
                        Q.Cast(jungleMobQ);
                    }
                }

                if (useR && jungleMobR != null)
                {
                    if (R.IsReady() && ammoR >= 1)
                    {
                        R.Cast(jungleMobR.Position);
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

            if (!R.IsReady() || Player.HasBuff("Recall") || autoRPanic)
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
                    foreach (var place in shroomPositions.SummonersRift.Where(pos => pos.Distance(ObjectManager.Player.Position) <= R.Range && !IsShroomed(pos)))
                    {
                        if (rCharge <= rCount && Environment.TickCount - lastR > 5000)
                        {
                            R.Cast(place);
                        }
                    }
                }
                else if (Game.MapId == EloBuddy.GameMapId.HowlingAbyss)
                {
                    foreach (var place in shroomPositions.HowlingAbyss.Where(pos => pos.Distance(ObjectManager.Player.Position) <= R.Range && !IsShroomed(pos)))
                    {
                        if (rCharge <= rCount && Environment.TickCount - lastR > 5000)
                        {
                            R.Cast(place);
                        }
                    }
                }
                else if (Game.MapId == EloBuddy.GameMapId.CrystalScar)
                {
                    foreach (var place in shroomPositions.CrystalScar.Where(pos => pos.Distance(ObjectManager.Player.Position) <= R.Range && !IsShroomed(pos)))
                    {
                        if (rCharge <= rCount && Environment.TickCount - lastR > 5000)
                        {
                            R.Cast(place);
                        }
                    }
                }
                else if (Game.MapId == EloBuddy.GameMapId.TwistedTreeline)
                {
                    foreach (var place in shroomPositions.TwistedTreeline.Where(pos => pos.Distance(ObjectManager.Player.Position) <= R.Range && !IsShroomed(pos)))
                    {
                        if (rCharge <= rCount && Environment.TickCount - lastR > 5000)
                        {
                            R.Cast(place);
                        }
                    }
                }
                else if (Game.MapId.ToString() == "Unknown")
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
                W.Cast(Player);
            }

            // Uses R if avaliable and if toggle is on
            if (useR && R.IsReady() && rCharge <= ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Ammo)
            {
                R.Cast(Player.Position);
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

            if (!Q.IsReady())
            {
                return;
            }

            if (Q.IsReady() && 1 <= allMinionsQ.Count())
            {
                foreach (var minion in allMinionsQ)
                {
                    if (minion.Health <= QDamage(minion) && Q.IsInRange(minion))
                    {
                        Q.Cast(minion);
                    }
                }
            }
            else if (Q.IsReady() && Q.IsInRange(target) && target.IsValidTarget())
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
                W.Cast(Player);
            }
        }

        /// <summary>
        /// Auto Q and W
        /// </summary>
        private static void AutoQw()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            var allMinionsQ = ObjectManager.Get<Obj_AI_Base>().Where(t => t.IsEnemy && Q.IsInRange(t)).OrderBy(t => t.Health);

            if (!W.IsReady() || !Q.IsReady())
            {
                return;
            }

            if (W.IsReady())
            {
                W.Cast();
            }

            if (target == null)
            {
                return;
            }

            if (Q.IsReady() && 1 <= allMinionsQ.Count())
            {
                foreach (var minion in allMinionsQ)
                {
                    if (minion.Health <= QDamage(minion) && Q.IsInRange(minion))
                    {
                        Q.Cast(minion);
                    }
                }
            }
            else if (Q.IsReady() && Q.IsInRange(target) && target.IsValidTarget() && 25 <= Player.ManaPercent)
            {
                Q.Cast(target);
            }
        }

        /// <summary>
        /// Called when Game Updates.
        /// </summary>
        /// <param name="args"></param>
        private static void Game_OnUpdate(EventArgs args)
        {
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
                
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None))
            {
                // Flee Menu
                if (FleeMenu["fleetoggle"].Cast<KeyBind>().CurrentValue)
                {
                    Flee();
                }

                // Auto Shroom
                if (MiscMenu["autoR"].Cast<CheckBox>().CurrentValue)
                {
                    AutoShroom();
                }

                // KillSteal
                if (KillStealMenu["KSAA"].Cast<CheckBox>().CurrentValue
                    || KillStealMenu["KSQ"].Cast<CheckBox>().CurrentValue
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
        /// TODO The args.
        /// </param>
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (debug["debugdraw"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawText(
                    (float)debug["x"].Cast<Slider>().CurrentValue,
                    (float)debug["x"].Cast<Slider>().CurrentValue,
                    System.Drawing.Color.Red,
                    Player.Position.ToString());
            }

            var drawQ = DrawingMenu["drawQ"].Cast<CheckBox>().CurrentValue;
            var drawR = DrawingMenu["drawR"].Cast<CheckBox>().CurrentValue;
            var colorBlind = DrawingMenu["colorBlind"].Cast<CheckBox>().CurrentValue;
            var player = ObjectManager.Player.Position;

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
                if (shroomPositions.SummonersRift.Count() > 0)
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
                if (shroomPositions.CrystalScar.Count() > 0)
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
                if (shroomPositions.HowlingAbyss.Count() > 0)
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
                if (shroomPositions.TwistedTreeline.Count() > 0)
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
                if (shroomPositions.ButcherBridge.Count() > 0)
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